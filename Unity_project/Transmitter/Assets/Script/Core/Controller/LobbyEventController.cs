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
	public class LobbyEventData
	{
		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		event Action<List<UserIdentity>> OnJoinLobby;

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		public void RegeistedOnJoinLobby(Action<List<UserIdentity>> onJoinLobby)
		{
			this.OnJoinLobby += onJoinLobby;
		}

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		event Action<UserIdentity> OnUserAdd;

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		public void RegeistedOnUserAdd(Action<UserIdentity> onUserAdd)
		{
			this.OnUserAdd += onUserAdd;
		}

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		event Action<UserIdentity> OnUserRemove;

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		public void RegeistedOnUserRemove(Action<UserIdentity> onUserRemove)
		{
			this.OnUserRemove += onUserRemove;
		}
	}	
}