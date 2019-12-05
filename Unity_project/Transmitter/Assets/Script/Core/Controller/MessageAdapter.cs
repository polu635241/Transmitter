using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Model;

namespace Transmitter.Net
{
	class EventNamePairDelegats :Dictionary<string,ChannelBindCacheData>{};
	
	public class MessageAdapter{

		MessageProcesser messageProcesser;

		object waitSendMessageLocker;

		List<byte[]> waitSendMessages = new List<byte[]> ();
	
		List<Channel> channelTable = new List<Channel>();

		Dictionary<string,EventNamePairDelegats> gameCallbackTable = new Dictionary<string, EventNamePairDelegats> ();

		Dictionary<ushort,List<Action<string>>> lobbyCallbackTable = new Dictionary<ushort, List<Action<string>>> ();

		internal void BindLobbyEvent(ushort header, Action<string> callback)
		{
			List<Action<string>> callbacks = null;

			if (!lobbyCallbackTable.TryGetValue (header, out callbacks)) 
			{
				callbacks = new List<Action<string>> ();

				lobbyCallbackTable.Add (header, callbacks);
			}

			callbacks.Add (callback);
		}

		internal void UnBindLobbyEvent(ushort header, Action<string> callback)
		{
			List<Action<string>> callbacks = null;

			if (lobbyCallbackTable.TryGetValue (header, out callbacks)) 
			{
				callbacks.Remove (callback);
			}
			else
			{
				Debug.LogError ("找不到對應的緩存");
			}
		}

		/// <summary>
		/// 只能在主線程呼叫Init以便記錄unity thread主線程是誰
		/// </summary>
		internal MessageAdapter()
		{
			waitSendMessageLocker = new object ();
			waitSendMessages = new List<byte[]> ();
			messageProcesser = new MessageProcesser (this);
		}


		/// <summary>
		/// 讓外部傳入轉換成封包的byte[]
		/// </summary>
		/// <param name="messages">Messages.</param>
		internal void AddProcessedSendMessage(byte[] messages)
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

		internal void Update()
		{
			messageProcesser.Update ();	
		}

		#region trigger in unity life circly
		internal List<byte[]> PopAllWaitSendMessages()
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

		internal void ReceiveMessage(byte[] buffer)
		{
			messageProcesser?.AddReceiveMessage (buffer);
		}

		//接收處理完成的封包 並找到對應的callback進行觸發
		internal void ReceiveProcessGameMessage(GameMessageData data)
		{
			EventNamePairDelegats eventNamePairDelegats;

			if(gameCallbackTable.TryGetValue(data.ChannelName,out eventNamePairDelegats))
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

		//接收處理完成的封包 並找到對應的callback進行觸發
		internal void ReceiveProcessLobbyMessage(LobbyMessageData data)
		{
			EventNamePairDelegats eventNamePairDelegats;

			List<Action<string>> callbacks = null;

			if (lobbyCallbackTable.TryGetValue (data.Header, out callbacks))
			{
				callbacks.ForEach (callback=>
					{
						callback.Invoke(data.Token);
					});
			}
			else
			{
				Debug.LogError ("invoke a not exist callback lobby event -> " + data.Header);
			}
		}

		#region Invoke by Channel

		#region Generic Bind
		internal void Bind(string channelName,string eventName,Action callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		internal void Bind(string channelName,string eventName,Action<object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		internal void Bind(string channelName,string eventName,Action<object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		internal void Bind(string channelName,string eventName,Action<object,object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		internal void Bind(string channelName,string eventName,Action<object,object,object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}

		internal void Bind(string channelName,string eventName,Action<object,object,object,object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.BindAction (callback);
		}
		#endregion

		#region Generic UnBind
		internal void UnBind(string channelName,string eventName,Action callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		internal void UnBind(string channelName,string eventName,Action<object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		internal void UnBind(string channelName,string eventName,Action<object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		internal void UnBind(string channelName,string eventName,Action<object,object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		internal void UnBind(string channelName,string eventName,Action<object,object,object,object> callback)
		{
			ChannelBindCacheData bindCacheData = GetBindCacheData (channelName, eventName);

			bindCacheData.UnBindAction (callback);
		}

		internal void UnBind(string channelName,string eventName,Action<object,object,object,object,object> callback)
		{
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

			if(!gameCallbackTable.TryGetValue(channelName,out eventNamePairDelegats))
			{
				eventNamePairDelegats = new EventNamePairDelegats ();
				gameCallbackTable.Add (channelName,eventNamePairDelegats);
			}

			ChannelBindCacheData bindCacheData;

			if (!eventNamePairDelegats.TryGetValue (eventName, out bindCacheData)) 
			{
				bindCacheData = new ChannelBindCacheData ();

				eventNamePairDelegats.Add (eventName, bindCacheData);
			}

			return bindCacheData;
		}

		internal void SendGameMessage (string channelName, string eventName, params System.Object[] objs)
		{
			messageProcesser.AddSendGameMessage (channelName, eventName, objs);
		}

		internal void SendLobbyMessage (ushort header, object content)
		{
			messageProcesser.AddSendLobbyMessage (header, content);
		}

		#endregion

		#region Factory
		internal Channel BindChannel(string channelKey)
		{
			Channel channel = Channel.ChannelFactory (channelKey, this);
			channelTable.Add (channel);
			return channel;
		}
		#endregion

		internal void Close()
		{
			messageProcesser?.Close ();
		}
	}
}
