using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Net.Model;

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

		[SerializeField][ReadOnly]
		string serverIP;
		int port;

		Channel playerStatusChannel = null;

		public void Init()
		{
			messageAdapter = new MessageAdapter ();
			playerStatusChannel = messageAdapter.BindChannel ($"{Presesrve_Start_Word}PlayerStatus");
		}

		/// <summary>
		/// 只能在主線程呼叫Init以便記錄unity thread主線程是誰
		/// CallBack的用法舉例來說 等到連線成功 才切換場景之類的
		/// </summary>
		public void Connect (string serverIP,int port,Action onConnectionCallback) 
		{
			//just cache
			this.serverIP = serverIP;
			this.port = port;
			
			DontDestroyOnLoad (this.gameObject);
			socketController = new SocketController (serverIP, port,onConnectionCallback);
		}

		const string Presesrve_Start_Word = "Presesrve-";

		public Channel BindChinnel(string channelNamel)
		{
			if (!channelNamel.StartsWith (Presesrve_Start_Word)) 
			{
				return messageAdapter.BindChannel (channelNamel);
			}
			else
			{
				throw new UnityException ($"{Presesrve_Start_Word} 開頭的頻道 是系統保留頻道 請勿使用");
			}

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