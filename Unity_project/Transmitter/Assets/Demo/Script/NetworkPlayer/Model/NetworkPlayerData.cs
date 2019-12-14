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
	[Serializable]
	public class NetworkPlayerData
	{
		public NetworkPlayerData (ushort udid, string playerName)
		{
			this.udid = udid;
			this.playerName = playerName;
		}
		
		public ushort Udid
		{
			get
			{
				return udid;
			}
		}

		[SerializeField][ReadOnly]
		ushort udid;

		public string PlayerName
		{
			get
			{
				return playerName;
			}

			set
			{
				playerName = value;
			}
		}

		[SerializeField][ReadOnly]
		string playerName;
	}
}