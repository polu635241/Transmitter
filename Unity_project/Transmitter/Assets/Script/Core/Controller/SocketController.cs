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
	public class SocketController {

		#region display in Inspector
		
		public bool IsConnected
		{
			get
			{
				return tcpClient.Client.Connected;
			}
		}

		[SerializeField][ReadOnly]
		int port;

		[SerializeField][ReadOnly]
		string serverIP;

		[SerializeField][ReadOnly]
		string token;

		#endregion	

		#region cache
//		Socket toServerSocket;
		TcpClient tcpClient;
		IAsyncResult IAsyncResult;

		Thread receiveThread;

		Thread sharkThread;
		#endregion

		object receiveMessageLocker;
		List<byte[]> receiveMessages = new List<byte[]> ();

		const int reconnectTime = 10;

		Action recursivelyConnect = null;

		MessageAdapter messageAdapter;
		Transmitter_Client transmitter_Client;

		//由於coroutine只能在主線程跑 所以透過hook通知
		bool runSharkHandHook;

		bool connected;

		object runSharkHandHookLocker;

		public SocketController (MessageAdapter messageAdapter, Transmitter_Client transmitter_Client)
		{
			receiveMessageLocker = new object ();
			receiveMessages = new List<byte[]> ();
			this.messageAdapter = messageAdapter;
			this.transmitter_Client = transmitter_Client;

			runSharkHandHookLocker = new object ();
            runSharkHandHook = false;
            connected = false;

        }

		/// <summary>
		/// Coroutine需要有代理人代為觸發
		/// </summary>
		/// <param name="serverIP">Server I.</param>
		/// <param name="port">Port.</param>
		/// <param name="token">Token.</param>
		/// <param name="proxy">Proxy.</param>
		public void ConnectionToServer (string serverIP, int port, string token)
		{
			this.port = port;
			this.serverIP = serverIP;
			this.token = token;

			Debug.Log($"開始嘗試連線 ip -> {serverIP}, port -> {port}");
			
			tcpClient = new TcpClient ();

			recursivelyConnect = () => 
			{
				IAsyncResult = tcpClient.BeginConnect (serverIP, port, (ar) => {
					if (tcpClient.Connected) 
					{
						Thread thread = new Thread(SharkHand);
						thread.Start();
						receiveThread = new Thread (RecieveServerMessage);
						receiveThread.Start ();
					}
					else 
					{
						Debug.LogError ($"連線失敗 等待{reconnectTime}秒後重新連線");
						Thread.Sleep (reconnectTime*1000);

						recursivelyConnect?.Invoke();
					}
				}, tcpClient);
			};

			recursivelyConnect.Invoke ();
		}

		void RecieveServerMessage() 
		{
			while(IsConnected) 
			{
				try 
				{
					byte[] buffer = new byte[2048];
					int receiveLength = tcpClient.Client.Receive(buffer);

					lock(receiveMessageLocker)
					{
						try
						{
							receiveMessages.Add(buffer);
						}
						catch (Exception e)
						{
							Debug.Log(e.Message);
						}
					}

				} 
				catch(SocketException e)
				{
					Debug.Log (e.Message);
				}
				catch(Exception e) 
				{
					Debug.Log(e.Message); 
				}
			}
		}

		public void SendMessageToServer(byte[] buffer)
		{
			if (IsConnected) 
			{
				tcpClient.Client.Send (buffer);
			}
		}

		#region unity life circly

		public List<byte[]> PopAllReceiveMessages()
		{
			List<byte[]> cache = null;

			lock(receiveMessageLocker)
			{
				try
				{
					if(receiveMessages.Count>0)
					{
						cache = new List<byte[]>(receiveMessages);
						receiveMessages.Clear();
					}
				}
				catch (Exception e)
				{
					Debug.Log(e.Message);
				}
			}

			return cache;
		}

		public void Close()
		{
			recursivelyConnect = null;
			tcpClient.Close ();
			tcpClient.Dispose ();
			receiveThread?.Abort ();

			if (sharkHandCoroutine != null) 
			{
				transmitter_Client.StopCoroutine (sharkHandCoroutine);
			}
		}

		Coroutine sharkHandCoroutine;

		public void Update()
		{
			if (!connected) 
			{
                lock (runSharkHandHookLocker) 
				{
					if (runSharkHandHook) 
					{
						sharkHandCoroutine = transmitter_Client.StartCoroutine (SharkHandIEnumerator ());
                        runSharkHandHook = false;
                        connected = true;
					}
				}
			}
		}

		void SharkHand()
		{
			messageAdapter.BindLobbyEvent (Consts.NetworkEvents.NewUserReq, ReceiveExistMembers);
			Debug.Log ("Bind");
			lock (runSharkHandHookLocker) 
			{
				runSharkHandHook = true;
			}
		}

		IEnumerator SharkHandIEnumerator()
		{
			float beginTime = Time.time;

			float endTime = beginTime + waitNewUserReqTimeout;

			while (newUserReq==null) 
			{
				yield return new WaitForFixedUpdate ();

				if (Time.time > endTime) 
				{
					messageAdapter.UnBindLobbyEvent (Consts.NetworkEvents.NewUserReq, ReceiveExistMembers);
					Debug.LogError ("SharkHand Time out");
					yield break;
				}
			}

			Debug.Log (newUserReq!=null);
			Debug.Log ("UnBind");
			messageAdapter.UnBindLobbyEvent (Consts.NetworkEvents.NewUserReq, ReceiveExistMembers);

			NewUserRes newUserRes = new NewUserRes (){ Token = this.token };
			messageAdapter.SendLobbyMessage (Consts.NetworkEvents.NewUserRes, newUserRes);

			UserData owner = UserData.Create (newUserReq.NewUserUDID, this.token);
			transmitter_Client.LobbyController.OnJoinLobby (newUserReq.UserDatas, owner);
		}

		const float waitNewUserReqTimeout = 5;

		NewUserReq newUserReq = null;

		void ReceiveExistMembers(string msg)
		{
			newUserReq = JsonUtility.FromJson<NewUserReq> (msg);
		}

		#endregion

	}
		
}