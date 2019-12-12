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

		[SerializeField][ReadOnly]
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
			public_Channel.Bind<string,ushort> (DemoConsts.Events.Rename, UpdateName);
		}

		void OnJoinLobby(List<UserData> others, UserData owner)
		{
			ushort udid = owner.Udid;
			string defaultName = string.Format (DefaultPlayerNameFormat, udid);

			networkPlayerData = new NetworkPlayerData (udid, defaultName);

			uiController.CreateOwnerPlayerField (defaultName, udid);
			networkMapper.SetPlayerNamePair (defaultName, udid);

			StartCoroutine (PlayerSharkHandCoroutine ());
		}

		IEnumerator PlayerSharkHandCoroutine()
		{
			bool recevieAllPlayerName = false;

			List<RefKeyValuePair<ushort,string>> otherPlayerNamePair = new List<RefKeyValuePair<ushort, string>> ();

			while (!recevieAllPlayerName) 
			{
				List<UserData> userDatas = client.LobbyController.OtherMembers;

				recevieAllPlayerName = userDatas.TrueForAll (userData => 
					{
						string playerName = string.Empty;
						
						if(networkMapper.TryGetPlayerName(userData.Udid, out playerName))
						{
							otherPlayerNamePair.Add(new RefKeyValuePair<ushort, string>(userData.Udid,playerName));
							return true;
						}
						else
						{
							return false;
						}
					});

				if (!recevieAllPlayerName) 
				{
					//每一偵玩家都可能增減 所以每次失敗都要清空
					otherPlayerNamePair.Clear ();
					yield return null;
				}

				otherPlayerNamePair.ForEach (pair=>
					{
						uiController.CreateOtherPlayerField(pair.value, pair.key);
					});
			}

			public_Channel.UnBind<string,ushort> (DemoConsts.Events.Rename, UpdateName);

			BindEventAfterSharkHand ();
		}

		void BindEventAfterSharkHand()
		{
			client.RegeistedOnUserAdd (OnUserAdd);
			client.RegeistedOnUserRemove (OnUserRemove);

			//玩家與玩家間的交握完畢後 才會一次性生成玩家名條
			public_Channel.Bind<string,ushort> (DemoConsts.Events.Rename, ReceiveRename);
		}

		void OnUserAdd(UserData userData)
		{
			ushort udid = userData.Udid;
			string defaultName = string.Format (DefaultPlayerNameFormat, udid);

			uiController.CreateOtherPlayerField (defaultName, udid);
			networkMapper.SetPlayerNamePair (defaultName, udid);

			string currentName = networkPlayerData.PlayerName;
			public_Channel.SendAssign (udid, DemoConsts.Events.Rename, currentName, Owner.Udid);
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
			UpdateName (newName, udid);

			if (udid == Owner.Udid) 
			{
				uiController.SetOwnerPlayerName (newName);
			}
			else
			{
				uiController.SetOtherPlayerName (newName, udid);
			}
		}

		void UpdateName(string newName, ushort udid)
		{
			networkMapper.SetPlayerNamePair (newName, udid);
		}
	}
}
