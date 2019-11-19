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
using Transmitter.Serialize;
using Transmitter.Net.Model;
using Transmitter.DataStruct;

namespace Transmitter.Net
{
	public class MessageProcesser {

		ObjectDeserialize_Base deserializeProcess;

		public ObjectDeserialize_Base DeserializeProcess
		{
			get
			{
				return deserializeProcess;
			}
		}

		ObjectSerialize_Base serializeProcess;

		public ObjectSerialize_Base SerializeProcess
		{
			get
			{
				return serializeProcess;
			}
		}

		object waitReceiveLocker;
		List<byte[]> waitReceivePool = new List<byte[]>();

		object waitInvokeDatasLocker;
		List<GameMessageData> receiveGameMessageDatas = new List<GameMessageData>();

		object waitSendDataLocker;
		List<GameMessageData> waitSendgameMessageDatas = new List<GameMessageData> ();
		Thread processSendMessageThread;

		MessageAdapter messageAdapter;

		Thread processMessageThread;

		public MessageProcesser(MessageAdapter messageRouter)
		{
			waitReceiveLocker = new object ();
			waitInvokeDatasLocker = new object ();
			waitSendDataLocker = new object ();

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
		public void Update()
		{
			List<GameMessageData> cacheDatas = new List<GameMessageData> ();
			
			lock(waitInvokeDatasLocker)
			{
				try
				{
					if(receiveGameMessageDatas.Count>0)
					{
						cacheDatas = new List<GameMessageData>(receiveGameMessageDatas);
						receiveGameMessageDatas.Clear();
					}
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			}

			cacheDatas.ForEach (data=>
				{
					messageAdapter.ReceiveProcessMessage(data);
				});
		}
			
		//call in sub thread
		public void ProcessMessage()
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
									GameMessageData parseData = GameMessageData.CreateByMsg(this.DeserializeProcess.DeserializeToObject, contentBuffer);

									lock(waitInvokeDatasLocker)
									{
										try
										{
											receiveGameMessageDatas.Add(parseData);
										}
										catch (Exception e)
										{
											Debug.LogError(e.Message);
										}
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

		public void ProcessSendMessage()
		{
			while(true)
			{
				List<GameMessageData> waitSendCaches = null;

				lock(waitSendDataLocker)
				{
					waitSendCaches = new List<GameMessageData> (waitSendgameMessageDatas);
					waitSendgameMessageDatas.Clear ();
				}

				if (waitSendCaches.Count==0) 
				{
					//只是一個離開的依據 並不是進入的條件 所以上方還要再lock一次
					SpinWait.SpinUntil (() => {
						return waitSendgameMessageDatas.Count > 0;
					});
				}
				else
				{
					waitSendCaches.ForEach (cache => {
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
				}
			}
		}

		#region Add Data
		public void AddReceiveMessage(byte[] message)
		{
			lock (waitReceiveLocker) 
			{
				waitReceivePool.Add (message);
			}
		}

		public void AddSendMessage (string channelName, string eventName, System.Object[] objs)
		{
			GameMessageData data = GameMessageData.Create (channelName, eventName, objs);

			lock(waitSendDataLocker)
			{
				waitSendgameMessageDatas.Add (data);
			}
		}
		#endregion

		public void Close()
		{
			processMessageThread?.Abort ();
			processSendMessageThread?.Abort ();
		}
	}
}