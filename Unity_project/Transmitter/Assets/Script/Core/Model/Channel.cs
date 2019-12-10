using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Net;

namespace Transmitter.Model
{
	public class Channel
	{
		MessageAdapter messageAdapter;

		internal Channel (string channelkey, MessageAdapter messageAdapter)
		{
			this.channelkey = channelkey;
			this.messageAdapter = messageAdapter;
		}

		string channelkey= "";

		public string ChannelKey
		{
			get
			{
				return channelkey;
			}
		}

		#region Generic Bind
		public void Bind (string key, Action callback)
		{
			Action processAction = this.GetProcessAction (callback);
			messageAdapter.Bind (channelkey, key, processAction);
		}

		public void Bind<T>(string key,Action<T> callback)
		{
			Action<object> processAction = this.GetProcessAction (callback);
			messageAdapter.Bind (channelkey, key, processAction);
		}

		public void Bind<T1,T2> (string key, Action<T1,T2> callback)
		{
			Action<object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.Bind (channelkey, key, processAction);
		}

		public void Bind<T1,T2,T3> (string key, Action<T1,T2,T3> callback)
		{
			Action<object,object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.Bind (channelkey, key, processAction);
		}

		public void Bind<T1,T2,T3,T4> (string key, Action<T1,T2,T3,T4> callback)
		{
			Action<object,object,object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.Bind (channelkey, key, processAction);
		}

		public void Bind<T1,T2,T3,T4,T5> (string key, Action<T1,T2,T3,T4,T5> callback)
		{
			Action<object,object,object,object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.Bind (channelkey, key, processAction);
		}
		#endregion


		#region Generic UnBind

		public void UnBind (string key, Action callback)
		{
			Action processAction = this.GetProcessAction (callback);
			messageAdapter.UnBind (channelkey, key, processAction);
		}

		public void UnBind<T> (string key, Action<T> callback)
		{
			Action<object> processAction = this.GetProcessAction (callback);
			messageAdapter.UnBind (channelkey, key, processAction);
		}

		public void UnBind<T1,T2> (string key, Action<T1,T2> callback)
		{
			Action<object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.UnBind (channelkey, key, processAction);
		}

		public void UnBind<T1,T2,T3> (string key, Action<T1,T2,T3> callback)
		{
			Action<object,object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.UnBind (channelkey, key, processAction);
		}

		public void UnBind<T1,T2,T3,T4> (string key, Action<T1,T2,T3,T4> callback)
		{
			Action<object,object,object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.UnBind (channelkey, key, processAction);
		}

		public void UnBind<T1,T2,T3,T4,T5> (string key, Action<T1,T2,T3,T4,T5> callback)
		{
			Action<object,object,object,object,object> processAction = this.GetProcessAction (callback);
			messageAdapter.UnBind (channelkey, key, processAction);
		}

		#endregion

		/// <summary>
		/// 全域發送
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="objs">Objects.</param>
		public void Send(string eventName,params System.Object[] objs)
		{
			messageAdapter.SendGameMessage ((short)-1, this.channelkey, eventName, objs);
		}

		/// <summary>
		/// 發送給指定使用者
		/// </summary>
		/// <param name="assignUdid">Assign udid.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="objs">Objects.</param>
		public void SendAssign (short assignUdid, string eventName, params System.Object[] objs)
		{
			messageAdapter.SendGameMessage (assignUdid, this.channelkey, eventName, objs);
		}
	}
}