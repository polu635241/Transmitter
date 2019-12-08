using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Tool;

namespace Transmitter.Demo.UI
{
	public class UIController
	{
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

		InputController renameMessageController;

		public void SetupRef(RefBinder refBinder)
		{
			GameObject dialogBoxGo = refBinder.GetGameobject (DemoConsts.AssetKeys.DialogBox);
			dialogBox = dialogBoxGo.AddComponent<DialogBox> ();

			ownerPlayerRoot = refBinder.GetComponent<Transform> (DemoConsts.AssetKeys.OwnerPlayerRoot);
			otherPlayerFieldsRoot = refBinder.GetComponent<Transform> (DemoConsts.AssetKeys.OtherPlayerRoot);

			otherPlayerFields = new List<PlayerField> ();

			InputField renameInputField = refBinder.GetComponent<InputField> (DemoConsts.AssetKeys.RenameInputField);
			Button renameBtn = refBinder.GetComponent<Button> (DemoConsts.AssetKeys.RenameBtn);
			renameMessageController = new InputController (renameInputField, renameBtn, SendRenameMessage);
		}

		public void CreateOwnerPlayerField(string playerName, ushort udid)
		{
			ownerPlayerField = PlayerField.Create (playerName, udid, PlayerField.PlayerFieldStyle.Owner, ownerPlayerRoot);
		}

		public void SetOwnerPlayerName(string playerName)
		{
			ownerPlayerField.SetPlayerName (playerName);
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

		public void RemoveOtherPlayerName(ushort udid)
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
	}
}