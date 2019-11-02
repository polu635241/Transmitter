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

		object parseDatasLocker;
		List<MessageData> parseDatas = new List<MessageData>();

		object waitSendLocker;
		List<MessageData> waitSendPool = new List<MessageData> ();
		Thread processSendMessageThread;

		MessageAdapter messageAdapter;

		Thread processMessageThread;

		public MessageProcesser(MessageAdapter messageRouter)
		{
			waitReceiveLocker = new object ();
			parseDatasLocker = new object ();
			waitSendLocker = new object ();

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
			List<MessageData> cacheDatas = new List<MessageData> ();
			
			lock(parseDatasLocker)
			{
				try
				{
					if(parseDatas.Count>0)
					{
						cacheDatas = new List<MessageData>(parseDatas);
						parseDatas.Clear();
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
								MessageData parseData = MessageData.CreateByMsg(this.DeserializeProcess.DeserializeToObject ,byteData);

								lock(parseDatasLocker)
								{
									try
									{
										parseDatas.Add(parseData);
									}
									catch (Exception e)
									{
										Debug.LogError(e.Message);
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
				List<MessageData> waitSendCaches = null;

				lock(waitSendLocker)
				{
					waitSendCaches = new List<MessageData> (waitSendPool);
					waitSendPool.Clear ();
				}

				if (waitSendCaches.Count==0) 
				{
					//只是一個離開的依據 並不是進入的條件 所以上方還要再lock一次
					SpinWait.SpinUntil (() => {
						return waitSendPool.Count > 0;
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
			MessageData data = MessageData.CreateByDefaultFormat (channelName, eventName, objs);

			lock(waitSendLocker)
			{
				waitSendPool.Add (data);
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