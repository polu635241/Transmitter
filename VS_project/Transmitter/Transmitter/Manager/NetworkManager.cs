using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Transmitter.Tool;
using Transmitter.Model;
using Transmitter.Plugin;
using Transmitter.DataStruct;

namespace Transmitter.Manager
{
    public class NetworkManager
    {
        object clientSocketsLocker = null;
        Socket serverSocket;
        List<Socket> clientSockets = new List<Socket>();

        Dictionary<ushort, List<Action<Socket, byte[]>>> networkEventsDict = new Dictionary<ushort, List<Action<Socket, byte[]>>>();
        object networkEventsDictLocker = new object();

        public NetworkManager(int port)
        {
            clientSocketsLocker = new object();

            networkEventsDictLocker = new object();

            #region networkEvent

            //不開出來填的原因是 小黑窗所在的網域就是伺服器本體才對
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            //IPV4
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip和端口  
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(10);
           
            CursorModule.Instance.WriteLine("啟動監聽{0}成功", serverSocket.LocalEndPoint.ToString());

            Thread serverThread = new Thread(ListenClientConnect);
            serverThread.Start();
            #endregion
        }

        void ListenClientConnect()
        {
            while (true)
            {
                //內部有另一個 while(true) 去把程序卡在這裡
                Socket clientSocket = serverSocket.Accept();

                lock (clientSocketsLocker)
                {
                    clientSockets.Add(clientSocket);
                }

                CursorModule.Instance.WriteLine("客戶端 {0} 成功連接", clientSocket.RemoteEndPoint.ToString());

                //把每個客戶端的thread錯開
                Thread clientThead = new Thread(RecieveClientMessage);
                clientThead.Start(clientSocket);
            }
        }

        void RecieveClientMessage(object clientSocket)
        {
            byte[] result = new byte[2048];
            Socket m_ClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveLength = m_ClientSocket.Receive(result);

                    if (receiveLength > 0)
                    {
                        SendMsgForAllClients(result);
                    }
                    else
                    {
                        if (receiveLength == 0)
                        {
                            CursorModule.Instance.WriteLine($"連接已斷開{m_ClientSocket.RemoteEndPoint.ToString()}");

                            RemoveClientSocket(m_ClientSocket);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CursorModule.Instance.WriteLine(ex.Message);

                    RemoveClientSocket(m_ClientSocket);
                    break;
                }
            }
        }


        /// <summary>
        /// 傳送給包含自己在內的所有client
        /// </summary>
        /// <param name="msg"></param>
        void SendMsgForAllClients(byte[] msg)
        {
            lock (clientSocketsLocker)
            {
                foreach (Socket socket in clientSockets)
                {
                    socket.Send(msg);
                }
            }
        }

        void RemoveClientSocket(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            lock (clientSocketsLocker)
            {
                clientSockets.Remove(clientSocket);
            }
        }
        public void BindNetworkEvent(ushort eventHeader, Action<Socket, byte[]> callback)
        {
            lock (networkEventsDictLocker)
            {
                List<Action<Socket, byte[]>> callbacks = null;

                if (!networkEventsDict.TryGetValue(eventHeader, out callbacks))
                {
                    callbacks = new List<Action<Socket, byte[]>>();
                    networkEventsDict.Add(eventHeader, callbacks);
                }

                callbacks.Add(callback);
            }
        }

        public void UnBindNetworkEvent(ushort eventHeader, Action<Socket, byte[]> callback)
        {
            lock (networkEventsDictLocker)
            {
                List<Action<Socket, byte[]>> callbacks = null;

                if (networkEventsDict.TryGetValue(eventHeader, out callbacks))
                {
                    bool removeSuccess = callbacks.Remove(callback);

                    if (!removeSuccess)
                    {
                        CursorModule.Instance.WriteLine($"can't remove event -> {eventHeader}");
                    }
                }
                else
                {
                    CursorModule.Instance.WriteLine($"can't find event -> {eventHeader}");
                }
            }
        }
    }
}
