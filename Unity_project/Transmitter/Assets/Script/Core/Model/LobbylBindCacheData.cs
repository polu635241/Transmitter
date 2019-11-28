using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Model;

namespace Transmitter.Net.Model
{
	public class LobbylBindCacheData
	{
		Dictionary<Action<UserData>,Action<string>> memberModifyCacheDatas= new Dictionary<Action<UserData>, Action<string>>();

		/// <summary>
		/// 目前沒有 解除大廳事件的需求 不過如果之後有 移除時才有個參照
		/// </summary>
		/// <param name="originalCallback">Original callback.</param>
		/// <param name="processCallback">Process callback.</param>
		public void AddMemberModifyCacheData(Action<UserData> originalCallback, Action<string> processCallback)
		{
			memberModifyCacheDatas.Add (originalCallback, processCallback);
		}

		public Action<string> GetProcessCallback(Action<UserData> originalCallback)
		{
			Action<string> processCallback = null;

			if(!memberModifyCacheDatas.TryGetValue (originalCallback , out processCallback))
			{
				throw new UnityException ("找不到對應的參照事件");
			}
			else
			{
				return processCallback;
			}
		}
	}
}
