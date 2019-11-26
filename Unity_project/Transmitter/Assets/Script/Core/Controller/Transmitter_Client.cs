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

		public SocketController SocketController
		{
			get
			{
				return SocketController;
			}
		}

		MessageAdapter messageAdapter;

		public MessageAdapter MessageAdapter
		{
			get
			{
				return MessageAdapter;
			}
		}

		public LobbyController LobbyController
		{
			get
			{
				return lobbyController;
			}
		}

		LobbyController lobbyController;

		[SerializeField][ReadOnly]
		string serverIP;
		int port;
		string token;

		public void Init()
		{
			messageAdapter = new MessageAdapter ();
			socketController = new SocketController (messageAdapter, this);
			lobbyController = new LobbyController (messageAdapter);
		}

		/// <summary>
		/// 只能在主線程呼叫Init以便記錄unity thread主線程是誰
		/// </summary>
		public void Connect (string serverIP, int port, string token)
		{
			//just cache
			this.serverIP = serverIP;
			this.port = port;
			this.token = token;
			
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

		void OnApplicationQuit()
		{
			socketController?.Close ();
			messageAdapter?.Close ();
		}
	}
}