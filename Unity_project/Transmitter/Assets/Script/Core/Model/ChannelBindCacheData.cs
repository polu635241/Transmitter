using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.Model
{
	public class ChannelBindCacheData
	{
		#region Generic Cache
		List<Action> noVarActions = new List<Action> ();

		List<Action<object>> oneVarActions = new List<Action<object>>();

		List<Action<object,object>> twoVarActions = 
			new List<Action<object,object>>();

		List<Action<object,object,object>> threeVarActions = 
			new List<Action<object,object,object>>();

		List<Action<object,object,object,object>> fourVarActions = 
			new List<Action<object,object,object,object>>();

		List<Action<object,object,object,object,object>> fiveVarActions = 
			new List<Action<object,object,object,object,object>>();
		#endregion

		#region Generic Bind
		public void BindAction(Action callback)
		{
			noVarActions.Add (callback);
		}

		public void BindAction(Action<object> callback)
		{
			oneVarActions.Add (callback);
		}

		public void BindAction(Action<object,object> callback)
		{
			twoVarActions.Add (callback);
		}

		public void BindAction(Action<object,object,object> callback)
		{
			threeVarActions.Add (callback);
		}

		public void BindAction(Action<object,object,object,object> callback)
		{
			fourVarActions.Add (callback);
		}

		public void BindAction(Action<object,object,object,object,object> callback)
		{
			fiveVarActions.Add (callback);
		}
		#endregion

		#region Generic UnBind
		public void UnBindAction(Action callback)
		{
			bool success = noVarActions.Remove (callback);

			if (!success)
				Debug.LogError ("UnBind Fail");
		}

		public void UnBindAction(Action<object> callback)
		{
			bool success = oneVarActions.Remove (callback);

			if (!success)
				Debug.LogError ("UnBind Fail");
		}

		public void UnBindAction(Action<object,object> callback)
		{
			bool success = twoVarActions.Remove (callback);

			if (!success)
				Debug.LogError ("UnBind Fail");
		}

		public void UnBindAction(Action<object,object,object> callback)
		{
			bool success = threeVarActions.Remove (callback);

			if (!success)
				Debug.LogError ("UnBind Fail");
		}

		public void UnBindAction(Action<object,object,object,object> callback)
		{
			bool success = fourVarActions.Remove (callback);

			if (!success)
				Debug.LogError ("UnBind Fail");
		}

		public void UnBindAction(Action<object,object,object,object,object> callback)
		{
			bool success = fiveVarActions.Remove (callback);

			if (!success)
				Debug.LogError ("UnBind Fail");
		}
		#endregion

		public void Trigger(params object[] objs)
		{
			int objLength = 0;

			if (objs != null) 
			{
				objLength = objs.Length;
			}
			else
			{
				objLength = 0;
			}

			switch(objs.Length)
			{
			case 0:
				{
					List<Action> _noVarActions = new List<Action> (noVarActions);

					_noVarActions.ForEach (callback => callback.Invoke ());

					break;
				}

			case 1:
				{
					List<Action<object>> _oneVarActions = new List<Action<object>> (oneVarActions);

					_oneVarActions.ForEach (callback => callback.Invoke (objs [0]));
					break;
				}

			case 2:
				{
					List<Action<object,object>> _twoVarActions = new List<Action<object, object>> (twoVarActions);
					
					_twoVarActions.ForEach (callback => 
						callback.Invoke (objs [0], objs [1]));
					break;
				}

			case 3:
				{
					List<Action<object,object,object>> _threeVarActions = new List<Action<object, object, object>> (threeVarActions);
					
					_threeVarActions.ForEach (callback => 
						callback.Invoke (objs [0], objs [1], objs [2]));
					break;
				}

			case 4:
				{
					List<Action<object,object,object,object>> _fourVarActions = new List<Action<object, object, object, object>> (fourVarActions);
					
					_fourVarActions.ForEach (callback => 
						callback.Invoke (objs [0], objs [1], objs [2], objs [3]));
					break;
				}

			case 5:
				{
					List<Action<object,object,object,object,object>> _fiveVarActions = new List<Action<object, object, object, object, object>> (fiveVarActions);
					
					_fiveVarActions.ForEach (callback => 
						callback.Invoke (objs [0], objs [1], objs [2], objs [3], objs [4]));
					break;
				}
			}
		}
	}
}