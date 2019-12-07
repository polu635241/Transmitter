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
	public class PlayerField
	{
		GameObject ownerGoRoot;

		Text playerNameText;
		Text commentText;

		void SetUpRef(GameObject ownerGoRoot)
		{
			this.ownerGoRoot = ownerGoRoot;

			RefBinder refBinder = ownerGoRoot.GetComponent<RefBinder> ();

			playerNameText = refBinder.GetChildComponent<Text> (DemoConsts.AssetKeys.PlayerNameText);
			commentText = refBinder.GetChildComponent<Text> (DemoConsts.AssetKeys.CommentText);
		}

		public void SetPlayerName(string playerName)
		{
			playerNameText.text = playerName;
		}

		//隱藏建構式
		PlayerField(){}

		/// <summary>
		/// 內容一樣 底板樣式與說明不一樣
		/// </summary>
		/// <param name="style">Style.</param>
		public static PlayerField Create(PlayerFieldStyle style)
		{
			PlayerField playerField = new PlayerField ();
			playerField.InstantiateEntity (style);
			return playerField;
		}

		void InstantiateEntity(PlayerFieldStyle style)
		{
			string resourceName = GetResourcesName (style);
			GameObject prefab = Resources.Load<GameObject> (resourceName);
			this.SetUpRef (prefab);
			this.commentText.text = GetComment (style);
		}

		string GetResourcesName(PlayerFieldStyle style)
		{
			string resourceName = "";

			switch (style) 
			{
			case PlayerFieldStyle.Owner:
				{
					resourceName = DemoConsts.Resources.OwnerPlayerField;
					break;
				}

			case PlayerFieldStyle.Other:
				{
					resourceName = DemoConsts.Resources.OtherPlayerField;
					break;
				}
			}

			return resourceName;
		}

		string GetComment(PlayerFieldStyle style)
		{
			string comment = $"{style} Player";
			return comment;
		}

		public void Close()
		{
			//關閉之前要把它控管的實體遊戲物件刪除
			MonoBehaviour.Destroy (ownerGoRoot);
		}

		public enum PlayerFieldStyle
		{
			Owner,Other
		}
	}
}
