using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Net.Model;

namespace Transmitter.Net
{
	class EventNamePairDelegats :Dictionary<string,ChannelBindCacheData>{};
	
	public class MessageAdapter{

		Thread unityThread;

		MessageProcesser messageProcesser;

		bool IsMainThread
		{
			get
			{
				if (unityThread != null) 
				{
					return unityThread.Equals (Thread.CurrentThread);
				}
				else
				{
					throw new UnityException ("尚未進行初始化");
				}
			}
		}

		object waitSendMessageLocker;

		List<byte[]> waitSendMessages = new List<byte[]> ();
	
		List<Channel> channelTable = new List<Channel>();

		Dictionary<string,EventNamePairDelegats> callbackTable = new Dictionary<string, EventNamePairDelegats>();

		/// <summary>
		/// 只能在主線程呼叫Init以便記錄unity thread主線程是誰
		/// </summary>
		public MessageAdapter()
		{
			waitSendMessageLocker = new object ();
			waitSendMessages = new List<byte[]> ();

			unityThread = Thread.CurrentThread;
			messageProcesser = new MessageProcesser (this);
		}


		/// <summary>
		/// 讓外部傳入轉換成封包的byte[]
		/// </summary>
		/// <param name="messages">Messages.</param>
		public void AddProcessedSendMessage(byte[] messages)
		{
			lock (waitSendMessageLocker) 
			{
				try
				{
					waitSendMessages.Add(messages);
				}
				catch(Exception e) 
				{
					Debug.LogError (e.Message);
				}
			}
		}

		public void Update()
		{
			messageProcesser.Update ();	
		}

		#region trigger in unity life circly
		public List<byte[]> PopAllWaitSendMessages()
		{
			List<byte[]> _waitSendMessage = null;


			lock (waitSendMessageLocker) 
			{
				try
				{
					if(waitSendMessages.Count>0)
					{
						_waitSendMessage = new List<byte[]>(waitSendMessages);
					}

					waitSendMessages.Clear();
				}
				catch(Exception e) 
				{
					Debug.LogError (e.Message);
				}
			}

			return _waitSendMessage;
		}

		#endregion

		public void ReceiveMessage(byte[] buffer)
		{
			messageProcesser?.AddReceiveMessage (buffer);
		}

		//接收處理完成的封包 並找到對應的callback進行觸發
		public void ReceiveProcessMessage(MessageData data)
		{
			EventNamePairDelegats eventNamePairDelegats;

			if(callbackTable.TryGetValue(data.ChannelName,out eventNamePairDelegats))
			{
				ChannelBindCacheData bindCacheData;

				if (eventNamePairDelegats.TryGetValue (data.EventName, out bindCacheData))
				{
					bindCacheData.Trigger (data.Objs);
				}
				else
				{
					throw new UnityException ("指定的event name不存在 -> " + data.EventName);
				}
			}
			else
			{
				throw new UnityException ("指定的channel名稱不存在  -> " + data.ChannelName);
			}
		}

		#region Invoke by Channel

		#region Generic Bind
		public void Bind(string channelName,string eventName,Action callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}

			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		public void Bind(string channelName,string eventName,Action<object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}

			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		public void Bind(string channelName,string eventName,Action<object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}

			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		public void Bind(string channelName,string eventName,Action<object,object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}

			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		public void Bind(string channelName,string eventName,Action<object,object,object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}

			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		public void Bind(string channelName,string eventName,Action<object,object,object,object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}

			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}
		#endregion

		#region Generic UnBind
		public void UnBind(string channelName,string eventName,Action callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}


			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		public void UnBind(string channelName,string eventName,Action<object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}


			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		public void UnBind(string channelName,string eventName,Action<object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}


			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		public void UnBind(string channelName,string eventName,Action<object,object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}


			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		public void UnBind(string channelName,string eventName,Action<object,object,object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}


			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		public void UnBind(string channelName,string eventName,Action<object,object,object,object,object> callback)
		{
			if (!IsMainThread) 
			{
				throw new UnityEngine.UnityException ("請只在unity thread進行bind");
			}


			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}
		#endregion

		/// <summary>
		/// 同樣key的已經生成過了就取得 沒有生成過就生成一個新的 然後註冊進內部
		/// </summary>
		/// <returns>The bind cache data.</returns>
		/// <param name="channelName">Channel name.</param>
		/// <param name="eventName">Event name.</param>
		ChannelBindCacheData GetBindCacheData(string channelName,string eventName)
		{
			EventNamePairDelegats eventNamePairDelegats;

			if(!callbackTable.TryGetValue(channelName,out eventNamePairDelegats))
			{
				eventNamePairDelegats = new EventNamePairDelegats ();
				callbackTable.Add (channelName,eventNamePairDelegats);
			}

			ChannelBindCacheData bindCacheData;

			if (!eventNamePairDelegats.TryGetValue (eventName, out bindCacheData)) 
			{
				bindCacheData = new ChannelBindCacheData ();

				eventNamePairDelegats.Add (eventName, bindCacheData);
			}

			return bindCacheData;
		}

		public void Send(string channelName,string eventName,params System.Object[] objs)
		{
			messageProcesser.AddSendMessage (channelName, eventName, objs);
		}
		#endregion

		#region Factory
		public Channel BindChannel(string channelKey)
		{
			Channel channel = Channel.ChannelFactory (channelKey, this);
			channelTable.Add (channel);
			return channel;
		}
		#endregion

		public void Close()
		{
			messageProcesser?.Close ();
		}
	}
}
