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

		[SerializeField][ReadOnly]
		string serverIP;
		int port;

		LobbylBindCacheData lobbylBindCacheData;

		public void Init()
		{
			messageAdapter = new MessageAdapter ();
			lobbylBindCacheData = new LobbylBindCacheData ();
		}

		/// <summary>
		/// 只能在主線程呼叫Init以便記錄unity thread主線程是誰
		/// CallBack的用法舉例來說 等到連線成功 才切換場景之類的
		/// </summary>
		public void Connect (string serverIP,int port) 
		{
			//just cache
			this.serverIP = serverIP;
			this.port = port;
			
			DontDestroyOnLoad (this.gameObject);
			socketController = new SocketController (serverIP, port);
		}

		public Channel BindChinnel(string channelNamel)
		{
			return messageAdapter.BindChannel (channelNamel);
		}

		#region LobbyEvent

		/// <summary>
		/// 進入大廳時會回傳先前已經進入者的 UserData
		/// </summary>
		public void RegeistedOnJoinLobby(Action<List<UserData>> onJoinLobby)
		{
			//只有這個是交握後才觸發 不是由大廳事件列管
			this.onJoinLobby = onJoinLobby;
		}

		event Action<List<UserData>> onJoinLobby;

		//基於跨版本的相容性 unity與vs的溝通 透過Json傳遞 client 與 client的溝通 才會完全的序列化成byte[]

		public void RegeistedOnUserAdd(Action<UserData> onUserAdd)
		{
			Action<string> processOnUserAdd = (jsonStr) => 
			{
				UserData userData = JsonUtility.FromJson<UserData>(jsonStr);
				onUserAdd.Invoke(userData);
			};

			lobbylBindCacheData.AddMemberModifyCacheData (onUserAdd, processOnUserAdd);

			messageAdapter.BindLobbyEvent (Consts.NetworkEvents.AddUser, processOnUserAdd);
		}

		public void RegeistedOnUserRemove(Action<UserData> onUserRemove)
		{
			Action<string> processOnUserRemove = (jsonStr) => 
			{
				UserData userData = JsonUtility.FromJson<UserData>(jsonStr);
				onUserRemove.Invoke(userData);
			};

			lobbylBindCacheData.AddMemberModifyCacheData (onUserRemove, processOnUserRemove);
			messageAdapter.BindLobbyEvent (Consts.NetworkEvents.AddUser, processOnUserRemove);
		}

		#endregion

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