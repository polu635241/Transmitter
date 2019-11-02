using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.Net.Model
{
	public static class ChannelExtend
	{
		public static Action GetProcessAction (this Channel channel, Action callback)
		{
			Action processCallback;

			if (!ProxyTable<Action,Action>.TryGetValue (callback, out processCallback))
			{
				processCallback = callback;

				ProxyTable<Action,Action>.Add (callback,processCallback);
			}

			return processCallback;
		}
		
		public static Action<object> GetProcessAction<T> (this Channel channel, Action<T> callback)
		{
			Action<object> processCallback;

			if (!ProxyTable<Action<T>,Action<object>>.TryGetValue (callback, out processCallback))
			{
				processCallback = (obj) => 
				{
					callback.Invoke((T)obj);
				};

				ProxyTable<Action<T>,Action<object>>.Add (callback,processCallback);
			}
			
			return processCallback;
		}

		public static Action<object,object> GetProcessAction<T1,T2> (this Channel channel, Action<T1,T2> callback)
		{
			Action<object,object> processCallback;

			if (!ProxyTable<Action<T1,T2>,Action<object,object>>.TryGetValue (callback, out processCallback))
			{
				processCallback = (obj1,obj2) => 
				{
					callback.Invoke((T1)obj1,(T2)obj2);
				};

				ProxyTable<Action<T1,T2>,Action<object,object>>.Add (callback,processCallback);
			}

			return processCallback;
		}

		public static Action<object,object,object> GetProcessAction<T1,T2,T3> (this Channel channel, Action<T1,T2,T3> callback)
		{
			Action<object,object,object> processCallback;

			if (!ProxyTable<Action<T1,T2,T3>,Action<object,object,object>>.TryGetValue (callback, out processCallback))
			{
				processCallback = (obj1,obj2,obj3) => {
					callback.Invoke((T1)obj1,(T2)obj2,(T3)obj3);
				};

				ProxyTable<Action<T1,T2,T3>,Action<object,object,object>>.Add (callback,processCallback);
			}

			return processCallback;
		}

		public static Action<object,object,object,object> GetProcessAction<T1,T2,T3,T4> (this Channel channel, Action<T1,T2,T3,T4> callback)
		{
			Action<object,object,object,object> processCallback;

			if (!ProxyTable<Action<T1,T2,T3,T4>,Action<object,object,object,object>>.TryGetValue (callback, out processCallback))
			{
				processCallback = (obj1,obj2,obj3,obj4) => {
					callback.Invoke((T1)obj1,(T2)obj2,(T3)obj3,(T4)obj4);
				};

				ProxyTable<Action<T1,T2,T3,T4>,Action<object,object,object,object>>.Add (callback,processCallback);
			}

			return processCallback;
		}

		public static Action<object,object,object,object,object> GetProcessAction<T1,T2,T3,T4,T5> (this Channel channel, Action<T1,T2,T3,T4,T5> callback)
		{
			Action<object,object,object,object,object> processCallback;

			if (!ProxyTable<Action<T1,T2,T3,T4,T5>,Action<object,object,object,object,object>>.TryGetValue (callback, out processCallback))
			{
				processCallback = (obj1,obj2,obj3,obj4,obj5) => {
					callback.Invoke((T1)obj1,(T2)obj2,(T3)obj3,(T4)obj4,(T5)obj5);
				};
			}

			return processCallback;
		}
	}
}