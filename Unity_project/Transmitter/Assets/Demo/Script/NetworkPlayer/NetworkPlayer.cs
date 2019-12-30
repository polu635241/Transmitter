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
			string defaultName = string.Format (DemoConsts.Formats.DefaultPlayerNameFormat, udid);

			networkPlayerData = new NetworkPlayerData (udid, defaultName);

			uiController.CreateOwnerPlayerField (defaultName, udid);
			uiController.ShowJoinLobbyMsg (defaultName);

			networkMapper.SetPlayerNamePair (defaultName, udid);

			StartCoroutine (PlayerSharkHandCoroutine (udid));
		}

		IEnumerator PlayerSharkHandCoroutine(ushort ownerUdid)
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
			}

			otherPlayerNamePair.ForEach (pair=>
				{
					uiController.CreateOtherPlayerField(pair.value, pair.key);
				});

			uiController.UnLockInputControllers ();

			public_Channel.UnBind<string,ushort> (DemoConsts.Events.Rename, UpdateName);

			BindEventAfterSharkHand ();
		}

		void BindEventAfterSharkHand()
		{
			client.RegeistedOnUserAdd (OnUserAdd);
			client.RegeistedOnUserRemove (OnUserRemove);

			//玩家與玩家間的交握完畢後 才會一次性生成玩家名條
			public_Channel.Bind<string,ushort> (DemoConsts.Events.Rename, ReceiveRenameMessage);
			public_Channel.Bind<string,ushort> (DemoConsts.Events.SendMessage, ReceiveTalkMessage);
		}

		void OnUserAdd(UserData userData)
		{
			ushort udid = userData.Udid;
			string defaultName = string.Format (DemoConsts.Formats.DefaultPlayerNameFormat, udid);

			uiController.CreateOtherPlayerField (defaultName, udid);
			uiController.ShowOnAddPlayerMsg (defaultName);
			networkMapper.SetPlayerNamePair (defaultName, udid);

			string currentName = networkPlayerData.PlayerName;
			public_Channel.SendAssign (udid, DemoConsts.Events.Rename, currentName, Owner.Udid);
		}

		void OnUserRemove(UserData userData)
		{
			uiController.RemoveOtherPlayerNameField (userData.Udid);

			string onRemovePlayerName = "";

			if (networkMapper.TryGetPlayerName (userData.Udid, out onRemovePlayerName)) 
			{
				uiController.ShowOnRemovePlayerMsg (onRemovePlayerName);
			}
			else
			{
				Debug.LogError ($"找不到對應的玩家名稱 -> {userData.Udid}");
			}
		}

		public void SendTalkMessage(string message)
		{
			public_Channel.Send (DemoConsts.Events.SendMessage, message, Owner.Udid);
		}

		void ReceiveTalkMessage(string message, ushort udid)
		{
			string playerName = "";

			if (networkMapper.TryGetPlayerName (udid, out playerName))
			{
				uiController.ReceiveTalkMessage (playerName, message);
			}
			else
			{
				Debug.LogError ($"找不到對應的玩家名稱緩存 {udid}");
			}
		}

		public void SendRenameMessage(string newName)
		{
			//本地端先緩存修改紀錄 UI依然透過封包修改 保持一致性
			networkPlayerData.PlayerName = newName;
			public_Channel.Send (DemoConsts.Events.Rename, newName, Owner.Udid);
		}

		void ReceiveRenameMessage(string newName, ushort udid)
		{
			string oldName = "";

			if (networkMapper.TryGetPlayerName (udid, out oldName))
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
				
				uiController.ReceiveRenameMessage (oldName, newName);
			}
			else
			{
				Debug.LogError ($"找不到對應的舊有玩家名稱緩存 {udid}");
			}
		}

		void UpdateName(string newName, ushort udid)
		{
			networkMapper.SetPlayerNamePair (newName, udid);
		}
	}
}
