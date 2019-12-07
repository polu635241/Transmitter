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
		const string PlayerMsgFormat = "{0} : {1}";
		const string PlayerJoinMsgFormat = "{0} -> Join room";
		const string PlayerExiyMsgFormat = "{0} -> Exit room";

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

		public string GetPlayerName(ushort udid)
		{
			string playerName;
			
			if (!udidPairPlayerName.TryGetValue (udid, out playerName)) 
			{
				Debug.LogError ("Can't query this playerName");
				playerName = "UnKnuow";
			}

			return playerName;
		}

		public string ConvertPlayerMsg (string msg, ushort udid)
		{
			string playerName = GetPlayerName (udid);
			return string.Format (PlayerMsgFormat, playerName, msg);
		}

	}
}
