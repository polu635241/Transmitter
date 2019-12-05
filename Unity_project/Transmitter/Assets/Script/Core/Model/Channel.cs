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
		static List<string> usedChannelKeys = new List<string>();
		MessageAdapter messageAdapter;

		Channel (string channelkey, MessageAdapter messageAdapter)
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

		public static Channel ChannelFactory (string channelKey, MessageAdapter messageController)
		{
			if (!usedChannelKeys.Contains (channelKey)) 
			{
				usedChannelKeys.Add (channelKey);

				return new Channel (channelKey, messageController);
			}
			else 
			{
				throw new UnityException (string.Format ("已存在相同key 的Channel -> {0}", channelKey));
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

		public void Send(string eventName,params System.Object[] objs)
		{
			messageAdapter.SendGameMessage (this.channelkey,eventName,objs);
		}
	}
}