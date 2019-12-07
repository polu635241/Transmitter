using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Net;
using Transmitter.Tool;
using Transmitter.Model;

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

		/// <summary>
		/// 公開群聊
		/// </summary>
		Channel public_Channel;

		[SerializeField][ReadOnly]
		UserData owner;

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

			uiController = new UIController ();
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
			this.owner = owner;
			ushort udid = owner.Udid;
			string defaultName = string.Format (DefaultPlayerNameFormat, udid);
			uiController.CreateOwnerPlayerField (defaultName, udid);
			networkMapper.SetPlayerNamePair (defaultName, udid);
		}

		void OnUserAdd(UserData userData)
		{

		}

		void OnUserRemove(UserData userData)
		{

		}

		void SendMessage(string message)
		{
			public_Channel.Send (DemoConsts.Events.SendMessage, owner.Udid, message);
		}

		void SendRename(string newName)
		{
			public_Channel.Send (DemoConsts.Events.Rename, newName, owner.Udid);
		}

		void ReceiveRename(string newName, ushort udid)
		{
			networkMapper.SetPlayerNamePair (newName, udid);

			if (udid == owner.Udid) 
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
