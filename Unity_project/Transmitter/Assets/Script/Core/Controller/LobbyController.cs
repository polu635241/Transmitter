using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Model;
using Transmitter.Net.Model;
using Transmitter.DataStruct;

namespace Transmitter.Net
{
	public class LobbyController
	{
		public LobbyController(MessageAdapter messageAdapter)
		{
			this.messageAdapter = messageAdapter;
			InitLobbyInit ();
		}
		
		MessageAdapter messageAdapter;
		
		/// <summary>
		/// 自己
		/// </summary>
		public UserData Owner;
		UserData owner;

		/// <summary>
		/// 所有人 包含自己
		/// </summary>
		/// <value>The members.</value>
		public List<UserData> Members
		{
			get
			{
				return members;
			}
		}

		List<UserData> members = new List<UserData>();

		/// <summary>
		/// 其他玩家 不含自己
		/// </summary>
		/// <value>The other members.</value>
		public List<UserData> OtherMembers
		{
			get
			{
				return otherMembers;
			}
		}

		public List<UserData> otherMembers = new List<UserData>();

		LobbylBindCacheData lobbylBindCacheData = new LobbylBindCacheData();

		#region LobbyEvent

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserDatas 以及自己的UserData
		/// </summary>
		public void RegeistedOnJoinLobby(Action<List<UserData>,UserData> onJoinLobby)
		{
			//只有這個是交握後才觸發 不是由大廳事件列管
			this.onJoinLobby = onJoinLobby;
		}

		event Action<List<UserData>,UserData> onJoinLobby;

		//基於跨版本的相容性 unity與vs的溝通 透過Json傳遞 client 與 client的溝通 才會完全的序列化成byte[]


		void InitLobbyInit()
		{
			messageAdapter.BindLobbyEvent (Consts.NetworkEvents.AddUser, ProcessOnUserAdd);

			messageAdapter.BindLobbyEvent (Consts.NetworkEvents.AddUser, ProcessOnUserRemove);
		}

		event Action<UserData> onUserAdd;

		public void RegeistedOnUserAdd(Action<UserData> onUserAddCallback)
		{
			this.onUserAdd += onUserAddCallback;
		}

		void ProcessOnUserAdd  (string jsonStr) 
		{
			UserData userData = JsonUtility.FromJson<UserData>(jsonStr);

			otherMembers.Add(userData);
			members.Add(userData);

			onUserAdd?.Invoke(userData);
		}

		event Action<UserData> onUserRemove;

		public void RegeistedOnUserRemove(Action<UserData> onUserRemoveCallback)
		{
			this.onUserRemove += onUserRemoveCallback;
		}

		void ProcessOnUserRemove (string jsonStr) 
		{
			UserData userData = JsonUtility.FromJson<UserData>(jsonStr);

			otherMembers.Remove(userData);
			members.Remove(userData);

			onUserRemove.Invoke(userData);
		}

		public void OnJoinLobby (List<UserData> otherMembers, UserData owner)
		{
			this.otherMembers = new List<UserData> (otherMembers);

			//在玩家登入的當下 自己是最後一個玩家 所以在列尾
			this.members = new List<UserData> (otherMembers);
			this.members.Add (owner);

			onJoinLobby.Invoke (otherMembers, owner);
		}

		#endregion
	}
}