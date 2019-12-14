using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Net;
using Transmitter.Tool;

namespace Transmitter.Demo
{
	public class NetworkMapper 
	{
		Dictionary<ushort,string> udidPairPlayerName = new Dictionary<ushort, string>();

		public void SetPlayerNamePair (string playerName, ushort udid)
		{
			string oldName;

			if (!udidPairPlayerName.TryGetValue (udid, out oldName)) 
			{
				udidPairPlayerName.Add (udid, playerName);
			}
			else
			{
				udidPairPlayerName [udid] = playerName;
			}
		}

		public bool TryGetPlayerName(ushort udid, out string playerName)
		{
			return udidPairPlayerName.TryGetValue (udid, out playerName);
		}
	}
}
