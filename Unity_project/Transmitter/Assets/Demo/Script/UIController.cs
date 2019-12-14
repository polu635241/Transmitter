using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Tool;

namespace Transmitter.Demo.UI
{
	[Serializable]
	public class UIController
	{
		const string FormatRenameMessage = "{0} 更名為 {1}";
		const string FormatSendMessage = "{0} : {1}";
		
		public UIController(NetworkPlayer netWorkPlayer)
		{
			this.netWorkPlayer = netWorkPlayer;
		}
		
		[SerializeField][ReadOnly]
		NetworkPlayer netWorkPlayer;
		
		[SerializeField][ReadOnly]
		DialogBox dialogBox;

		[SerializeField][ReadOnly]
		Transform ownerPlayerRoot;

		[SerializeField][ReadOnly]
		PlayerField ownerPlayerField;

		[SerializeField][ReadOnly]
		Transform otherPlayerFieldsRoot;

		[SerializeField][ReadOnly]
		List<PlayerField> otherPlayerFields;

		[SerializeField][ReadOnly]
		InputController sendRenameMessageController;

		[SerializeField][ReadOnly]
		InputController sendTalkMessageController;

		public void SetupRef(RefBinder refBinder)
		{
			GameObject dialogBoxGo = refBinder.GetGameobject (DemoConsts.AssetKeys.DialogBox);
			dialogBox = dialogBoxGo.AddComponent<DialogBox> ();

			ownerPlayerRoot = refBinder.GetComponent<Transform> (DemoConsts.AssetKeys.OwnerPlayerRoot);
			otherPlayerFieldsRoot = refBinder.GetComponent<Transform> (DemoConsts.AssetKeys.OtherPlayerRoot);

			otherPlayerFields = new List<PlayerField> ();

			InputField renameInputField = refBinder.GetComponent<InputField> (DemoConsts.AssetKeys.RenameInputField);
			Button renameBtn = refBinder.GetComponent<Button> (DemoConsts.AssetKeys.RenameBtn);
			sendRenameMessageController = new InputController (renameInputField, renameBtn, SendRenameMessage);

			InputField talkInputField = refBinder.GetComponent<InputField> (DemoConsts.AssetKeys.TalkInputField);
			Button talkBtn = refBinder.GetComponent<Button> (DemoConsts.AssetKeys.TalkBtn);
			sendTalkMessageController = new InputController (talkInputField, talkBtn, SendTalkMessage);
		}

		public void CreateOwnerPlayerField(string playerName, ushort udid)
		{
			ownerPlayerField = PlayerField.Create (playerName, udid, PlayerField.PlayerFieldStyle.Owner, ownerPlayerRoot);
		}

		public void SetOwnerPlayerName(string playerName)
		{
			ownerPlayerField.SetPlayerName (playerName);
		}

		public void CreateOtherPlayerField(string playerName, ushort udid)
		{
			PlayerField otherPlayerField = PlayerField.Create (playerName, udid, PlayerField.PlayerFieldStyle.Other, otherPlayerFieldsRoot);
			otherPlayerFields.Add (otherPlayerField);
		}

		public void SetOtherPlayerName (string playerName, ushort udid)
		{
			PlayerField findPlayerField = otherPlayerFields.Find (field => field.UDID == udid);

			if (findPlayerField != null) 
			{
				findPlayerField.SetPlayerName (playerName);
			}
			else
			{
				Debug.LogError ($"{udid} 找不到對應的 玩家資料緩存");
			}
		}

		public void RemoveOtherPlayerNameField(ushort udid)
		{
			PlayerField findPlayerField = otherPlayerFields.Find (field => field.UDID == udid);

			if (findPlayerField != null) 
			{
				findPlayerField.Close ();
				otherPlayerFields.Remove (findPlayerField);
			}
			else
			{
				Debug.LogError ($"{udid} 找不到對應的 玩家資料緩存");
			}
		}

		public void SendRenameMessage(string newName)
		{
			netWorkPlayer.SendRenameMessage (newName);
		}

		public void ReceiveRenameMessage (string oldName, string newName)
		{
			string processMessage = string.Format (FormatRenameMessage, oldName, newName);
			dialogBox.Input (processMessage);
		}

		public void SendTalkMessage(string message)
		{
			netWorkPlayer.SendTalkMessage (message);
		}

		public void ReceiveTalkMessage (string playerName, string message)
		{
			string processMessage = string.Format (FormatSendMessage, playerName, message);
			dialogBox.Input (processMessage);
		}
	}
}