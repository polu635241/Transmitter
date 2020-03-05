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
	public class Transmitter_Client : MonoBehaviour {

		SocketController socketController;

		MessageAdapter messageAdapter;

		public LobbyController LobbyController
		{
			get
			{
				return lobbyController;
			}
		}

		LobbyController lobbyController;

		public void Awake()
		{
			messageAdapter = new MessageAdapter (this);
			socketController = new SocketController (messageAdapter, this);
			lobbyController = new LobbyController (messageAdapter);
		}

		/// <summary>
		/// 只能在主線程呼叫Init以便記錄unity thread主線程是誰
		/// </summary>
		public void Connect (string serverIP, int port, string token)
		{
			
			DontDestroyOnLoad (this.gameObject);
			socketController.ConnectionToServer (serverIP, port, token);
		}

		public Channel BindChinnel(string channelNamel)
		{
			return messageAdapter.BindChannel (channelNamel);
		}

		// Update is called once per frame
		void Update () 
		{
			socketController?.Update ();
			
			List<byte[]> newReceiveMessages = socketController?.PopAllReceiveMessages ();

			newReceiveMessages?.ForEach (message => messageAdapter.ReceiveMessage (message));

			messageAdapter.Update ();

			List<byte[]> newWaitSendMessages = messageAdapter?.PopAllWaitSendMessages ();

			newWaitSendMessages?.ForEach (message => 
				{
					socketController.SendMessageToServer (message);
				}
			);
		}

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserDatas 以及自己的UserData
		/// </summary>
		public void RegeistedOnJoinLobby(Action<List<UserData>,UserData> onJoinLobby)
		{
			lobbyController.RegeistedOnJoinLobby (onJoinLobby);
		}

		public void RegeistedOnUserAdd(Action<UserData> onUserAddCallback)
		{
			lobbyController.RegeistedOnUserAdd (onUserAddCallback);
		}

		public void RegeistedOnUserRemove(Action<UserData> onUserRemoveCallback)
		{
			lobbyController.RegeistedOnUserRemove (onUserRemoveCallback);
		}

		internal void OnJoinLobby (List<UserData> otherMembers, UserData owner)
		{
			lobbyController.OnJoinLobby (otherMembers, owner);
		}

		#if UNITY_EDITOR
		void OnDestroy()
		{
			Close ();
		}
		#else
		void OnApplicationQuit()
		{
			Close ();
		}
		#endif

		void Close()
		{
			socketController?.Close ();
			messageAdapter?.Close ();
		}
	}
}