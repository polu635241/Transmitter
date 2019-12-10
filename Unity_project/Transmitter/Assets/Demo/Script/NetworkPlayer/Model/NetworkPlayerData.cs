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
	public class NetworkPlayerData
	{
		public NetworkPlayerData(ushort udid)
		{
			this.udid = udid;
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

		public bool HasModifyPlayerName
		{
			get
			{
				return hasModifyPlayerName;
			}
		}

		[SerializeField][ReadOnly]
		bool hasModifyPlayerName;

		public string PlayerName
		{
			get
			{
				return playerName;
			}

			set
			{
				playerName = value;
				hasModifyPlayerName = true;
			}
		}

		[SerializeField][ReadOnly]
		string playerName;
	}
}