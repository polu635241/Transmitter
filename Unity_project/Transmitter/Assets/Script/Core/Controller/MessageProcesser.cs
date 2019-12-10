using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Transmitter.Model;
using Transmitter.Serialize;
using Transmitter.DataStruct;

namespace Transmitter.Net
{
	public class MessageProcesser {

		ObjectDeserialize_Base deserializeProcess;

		internal ObjectDeserialize_Base DeserializeProcess
		{
			get
			{
				return deserializeProcess;
			}
		}

		ObjectSerialize_Base serializeProcess;

		internal ObjectSerialize_Base SerializeProcess
		{
			get
			{
				return serializeProcess;
			}
		}

		object waitReceiveLocker;
		List<byte[]> waitReceivePool = new List<byte[]>();

		#region Game
		object waitInvokeGameMessagesLocker;
		List<GameMessageData> receiveGameMessageDatas = new List<GameMessageData>();

		object waitSendGameMessageLocker;
		List<GameMessageData> waitSendGameMessageDatas = new List<GameMessageData> ();
		#endregion

		#region Lobby
		object waitInvokeLobbyMessagesLocker;
		List<LobbyMessageData> receiveLobbyMessageDatas = new List<LobbyMessageData>();

		object waitSendLobbyMessageLocker;
		List<LobbyMessageData> waitSendLobbyMessageDatas = new List<LobbyMessageData> ();
		#endregion

		Thread processSendMessageThread;

		MessageAdapter messageAdapter;

		Thread processMessageThread;

		internal MessageProcesser(MessageAdapter messageRouter)
		{
			waitReceiveLocker = new object ();
			waitInvokeGameMessagesLocker = new object ();
			waitSendGameMessageLocker = new object ();

			waitInvokeLobbyMessagesLocker = new object ();
			waitSendLobbyMessageLocker = new object ();

			deserializeProcess = new ObjectDeserialize_CustomType();
			deserializeProcess.Init ();

			serializeProcess = new ObjectSerialize_CustomType ();
			serializeProcess.Init ();

			this.messageAdapter = messageRouter;

			processMessageThread = new Thread (ProcessMessage);
			processMessageThread.Start ();

			processSendMessageThread = new Thread (ProcessSendMessage);
			processSendMessageThread.Start ();
		}

		//call in main thread
		internal void Update()
		{
			#region Game
			List<GameMessageData> gameMessageDatas = new List<GameMessageData> ();
			
			lock(waitInvokeGameMessagesLocker)
			{
				gameMessageDatas = new List<GameMessageData>(receiveGameMessageDatas);
				receiveGameMessageDatas.Clear();
			}

			gameMessageDatas.ForEach (data=>
				{
					messageAdapter.ReceiveProcessGameMessage(data);
				});
			#endregion

			#region Lobby

			List<LobbyMessageData> lobbyMessageDatas = new List<LobbyMessageData>();

			lock(waitInvokeLobbyMessagesLocker)
			{
				lobbyMessageDatas = new List<LobbyMessageData>(receiveLobbyMessageDatas);
				receiveLobbyMessageDatas.Clear();
			}

			lobbyMessageDatas.ForEach(data=>
				{
					messageAdapter.ReceiveProcessLobbyMessage(data);
				});

			#endregion
		}
			
		//call in sub thread
		internal void ProcessMessage()
		{
			while (true) 
			{
				List<byte[]> receiveDatas = new List<byte[]> ();

				lock (waitReceiveLocker) 
				{
					try
					{
						if(waitReceivePool.Count>0)
						{
							receiveDatas = new List<byte[]>(waitReceivePool);
							waitReceivePool.Clear();
						}
						
					}
					catch (Exception e) 
					{
						Debug.LogError (e.Message);
					}
				}


				if (receiveDatas.Count == 0)
					//只是一個離開的依據 並不是進入的條件 所以上方還要再lock一次
					SpinWait.SpinUntil (() => {
						return waitReceivePool.Count > 0;
					});
				else
				{
					receiveDatas.ForEach ((byteData)=>
						{
							try
							{
								MemoryStream memoryStream = new MemoryStream(byteData);
								BinaryReader binaryReader = new BinaryReader(memoryStream);
								ushort header = binaryReader.ReadUInt16();
								int contentBufferLength = (int)binaryReader.ReadUInt16();
								byte[] contentBuffer = binaryReader.ReadBytes(contentBufferLength);

								if(header == Consts.NetworkEvents.GameMessage)
								{
									GameMessageData messageData = GameMessageData.CreateByMsg(this.DeserializeProcess.DeserializeToObject, contentBuffer);

									lock(waitInvokeGameMessagesLocker)
									{
										receiveGameMessageDatas.Add(messageData);
									}
								}
								else
								{
									LobbyMessageData messageData = LobbyMessageData.CreateByMsg(header, contentBuffer);

									lock(waitInvokeLobbyMessagesLocker)
									{
										receiveLobbyMessageDatas.Add(messageData);
									}
								}

							}
							catch (Exception e)
							{
								Debug.LogError(e.Message);
							}
						});
				}
			}
		}

		internal void ProcessSendMessage()
		{
			while(true)
			{
				List<GameMessageData> _waitSendGameMessageDatas = null;
				List<LobbyMessageData> _waitSendLobbyMessageData = null;

				lock(waitSendGameMessageLocker)
				{
					_waitSendGameMessageDatas = new List<GameMessageData> (waitSendGameMessageDatas);
					waitSendGameMessageDatas.Clear ();
				}

				lock(waitSendLobbyMessageLocker)
				{
					_waitSendLobbyMessageData = new List<LobbyMessageData> (waitSendLobbyMessageDatas);
					waitSendLobbyMessageDatas.Clear ();
				}

				if (_waitSendGameMessageDatas.Count == 0 && _waitSendLobbyMessageData.Count == 0)
				{
					//只是一個離開的依據 並不是進入的條件 所以上方還要再lock一次
					SpinWait.SpinUntil (() => {
						return waitSendGameMessageDatas.Count > 0|| waitSendLobbyMessageDatas.Count > 0;
					});
				}
				else
				{
					_waitSendGameMessageDatas.ForEach (cache => {
						try 
						{
							byte[] message = cache.GetBuffer(this.serializeProcess.SerializeToBuffer);
							messageAdapter.AddProcessedSendMessage(message);
						}
						catch (Exception e) 
						{
							Debug.LogError (e.Message);
						}
					});

					_waitSendLobbyMessageData.ForEach (cache=>{
						try
						{
							byte[] message = cache.GetBuffer();
							messageAdapter.AddProcessedSendMessage(message);
						}
						catch(Exception e)
						{
							Debug.Log(e.Message);
						}
					});
				}
			}
		}

		#region Add Data
		internal void AddReceiveMessage(byte[] message)
		{
			lock (waitReceiveLocker) 
			{
				waitReceivePool.Add (message);
			}
		}

		internal void AddSendGameMessage (short assignUdid, string channelName, string eventName, System.Object[] objs)
		{
			GameMessageData data = GameMessageData.Create (assignUdid, channelName, eventName, objs);

			lock(waitSendGameMessageLocker)
			{
				waitSendGameMessageDatas.Add (data);
			}
		}

		internal void AddSendLobbyMessage (ushort header, object content)
		{
			LobbyMessageData data = LobbyMessageData.Create (header, content);

			lock(waitSendLobbyMessageLocker)
			{
				waitSendLobbyMessageDatas.Add (data);
			}
		}

		#endregion

		internal void Close()
		{
			processMessageThread?.Abort ();
			processSendMessageThread?.Abort ();
		}
	}
}