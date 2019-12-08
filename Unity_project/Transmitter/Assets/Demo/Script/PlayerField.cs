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
		[SerializeField][ReadOnly]
		GameObject ownerGoRoot;

		[SerializeField][ReadOnly]
		Text playerNameText;

		[SerializeField][ReadOnly]
		Text commentText;

		[SerializeField][ReadOnly]
		ushort udid;

		public ushort UDID
		{
			get
			{
				return udid;
			}
		}

		void SetUpRef(GameObject ownerGoRoot)
		{
			this.ownerGoRoot = ownerGoRoot;

			RefBinder refBinder = ownerGoRoot.GetComponent<RefBinder> ();

			playerNameText = refBinder.GetComponent<Text> (DemoConsts.AssetKeys.PlayerNameText);
			commentText = refBinder.GetComponent<Text> (DemoConsts.AssetKeys.CommentText);
		}

		public void SetPlayerName (string playerName)
		{
			playerNameText.text = playerName;
		}

		//隱藏建構式
		PlayerField(){}

		/// <summary>
		/// 內容一樣 底板樣式與說明不一樣
		/// </summary>
		/// <param name="style">Style.</param>
		public static PlayerField Create(string playerName, ushort udid, PlayerFieldStyle style, Transform root)
		{
			PlayerField playerField = new PlayerField ();
			playerField.InstantiateEntity (style, root);
			playerField.playerNameText.text = playerName;
			playerField.udid = udid;
			return playerField;
		}

		void InstantiateEntity (PlayerFieldStyle style, Transform root)
		{
			string resourceName = GetResourcesName (style);
			GameObject prefab = Resources.Load<GameObject> (resourceName);
			GameObject entity = MonoBehaviour.Instantiate<GameObject> (prefab, root);

			//Other靠layout排序
			if (style == PlayerFieldStyle.Owner) 
			{
				RectTransform entityRectTransform = entity.GetComponent<RectTransform> ();
				entityRectTransform.anchoredPosition = Vector2.zero;
			}

			this.SetUpRef (entity);
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
