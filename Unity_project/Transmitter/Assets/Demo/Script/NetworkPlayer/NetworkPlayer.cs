using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Net;
using Transmitter.Tool;
using Transmitter.Model;
using Transmitter.Demo.UI;

namespace Transmitter.Demo
{
	public class NetworkPlayer : MonoBehaviour 
	{
		const string DefaultPlayerNameFormat = "Player{0}";
		
		[SerializeField][ReadOnly]
		Transmitter_Client client;

		[SerializeField][ReadOnly]
		RefBinder refBinder;

		NetworkMapper networkMapper;
		NetworkPlayerData networkPlayerData;

		/// <summary>
		/// 公開群聊
		/// </summary>
		Channel public_Channel;

		UserData Owner
		{
			get
			{
				return client.LobbyController.Owner;
			}
		}

		[SerializeField][ReadOnly]
		UIController uiController;

		// Use this for initialization
		void Awake () 
		{
			InitController ();
			InitNeworkEvent ();
			//Token不是帶入玩家名稱的 是用來辨別身分的 
			client.Connect ("127.0.0.1", 9487, DemoConsts.Tokens.DefaultPlayer);
		}
		
		void InitController()
		{
			GameObject m_Go = this.gameObject;
			client = m_Go.AddComponent<Transmitter_Client> ();
			refBinder = this.GetComponent<RefBinder> ();

			networkMapper = new NetworkMapper ();

			uiController = new UIController (this);
			uiController.SetupRef (refBinder);
		}

		void InitNeworkEvent()
		{
			public_Channel = client.BindChinnel (DemoConsts.Channels.Player);
			client.RegeistedOnJoinLobby (OnJoinLobby);
			client.RegeistedOnUserAdd (OnUserAdd);
			client.RegeistedOnUserRemove (OnUserRemove);
			public_Channel.Bind<string,ushort> (DemoConsts.Events.Rename, ReceiveRename);
		}

		void OnJoinLobby(List<UserData> others, UserData owner)
		{
			ushort udid = owner.Udid;

			networkPlayerData = new NetworkPlayerData (udid);

			string defaultName = string.Format (DefaultPlayerNameFormat, udid);
			uiController.CreateOwnerPlayerField (defaultName, udid);
			networkMapper.SetPlayerNamePair (defaultName, udid);

			others.ForEach ((userData)=>
				{
					OnUserAdd(userData);
				});
		}

		void OnUserAdd(UserData userData)
		{
			ushort udid = userData.Udid;
			string defaultName = string.Format (DefaultPlayerNameFormat, udid);

			uiController.CreateOtherPlayerField (defaultName, udid);
			networkMapper.SetPlayerNamePair (defaultName, udid);

			//玩家進來後 會透過UDID生成預設的人物ID 如果改過名字 就把新的名字傳給別人
			if (networkPlayerData.HasModifyPlayerName) 
			{
				string currentName = networkPlayerData.PlayerName;
				public_Channel.SendAssign (udid, DemoConsts.Events.Rename, currentName, Owner.Udid);
			}
		}

		void OnUserRemove(UserData userData)
		{
			uiController.RemoveOtherPlayerNameField (userData.Udid);
		}

		void SendMessage(string message)
		{
			public_Channel.Send (DemoConsts.Events.SendMessage, Owner.Udid, message);
		}

		public void SendRenameMessage(string newName)
		{
			//本地端先緩存修改紀錄 UI依然透過封包修改 保持一致性
			networkPlayerData.PlayerName = newName;
			public_Channel.Send (DemoConsts.Events.Rename, newName, Owner.Udid);
		}

		void ReceiveRename(string newName, ushort udid)
		{
			networkMapper.SetPlayerNamePair (newName, udid);

			if (udid == Owner.Udid) 
			{
				uiController.SetOwnerPlayerName (newName);
			}
			else
			{
				uiController.SetOtherPlayerName (newName, udid);
			}
		}
	}
}
