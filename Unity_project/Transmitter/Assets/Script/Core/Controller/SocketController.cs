using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		#endregion	

		#region cache
//		Socket toServerSocket;
		TcpClient tcpClient;
		IAsyncResult IAsyncResult;

		Thread receiveThread;
		#endregion

		object receiveMessageLocker;
		List<byte[]> receiveMessages = new List<byte[]> ();

		const int reconnectTime = 10;

		Action recursivelyConnect = null;

		/// <summary>
		/// CallBack的用法舉例來說 等到連線成功 才切換場景之類的
		/// </summary>
		/// <param name="serverIP">Server I.</param>
		/// <param name="port">Port.</param>
		/// <param name="connectedTrigger">Connected trigger.</param>
		/// 
		public SocketController (string serverIP, int port,
			Action onConnectionCallback)
		{
			receiveMessageLocker = new object ();
			receiveMessages = new List<byte[]> ();

			this.port = port;
			this.serverIP = serverIP;

			Debug.Log($"開始嘗試連線 ip -> {serverIP}, port -> {port}");

			ConnectionToServer (serverIP, port, onConnectionCallback);
		}

		void ConnectionToServer (string serverIP,int port, Action onConnectionCallback)
		{
			tcpClient = new TcpClient ();

			recursivelyConnect = () => 
			{
				IAsyncResult = tcpClient.BeginConnect (serverIP, port, (ar) => {
					if (tcpClient.Connected) 
					{
						receiveThread = new Thread (RecieveServerMessage);
						receiveThread.Start ();
						onConnectionCallback.Invoke ();
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
		}

		#endregion

	}
		
}