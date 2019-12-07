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
	public static class DemoConsts 
	{
		public static class AssetKeys
		{
			public const string DialogBox = "DialogBox";
			public const string WaitSendMsgText = "WaitSendMsgText";
			public const string WaitSendMsgBtn = "WaitSendMsgBtn";

			public const string RenameText = "RenameText";
			public const string RenameBtn = "RenameBtn";

			public const string OwnerPlayerRoot = "OwnerPlayerRoot";
			public const string OtherPlayerRoot = "OtherPlayerRoot";
			public const string PlayerNameText = "PlayerNameText";
			public const string CommentText = "CommentText";
		}

		public static class Resources
		{
			public const string OwnerPlayerField = "OwnerPlayerField";
			public const string OtherPlayerField = "OtherPlayerField";
		}

		public static class Tokens
		{
			public const string DefaultPlayer = "DefaultPlayer";
		}

		public static class Channels
		{
			public const string Player = "Player";
		}

		public static class Events
		{
			public const string Rename = "Rename";
			public const string SendMessage = "SendMessage";
		}
	}
}
