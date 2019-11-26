using System;
using System.IO;
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
        Socket serverSocket;

        object identityCheckLocker = null;
        Dictionary<Socket, UserData> userDataPairSocketTable = new Dictionary<Socket, UserData>();

        object networkEventsDictLocker = new object();
        Dictionary<ushort, List<Action<Socket, string>>> networkEventsDict = new Dictionary<ushort, List<Action<Socket, string>>>();

        Dictionary<Socket, Thread> receiveMessageDict = new Dictionary<Socket, Thread>();

        /// <summary>
        /// 只包含正式握手完畢的client
        /// </summary>
        List<Socket> clientSockets = new List<Socket>();

        Socket waitSharkHandSocket;

        public NetworkManager(int port)
        {
            InitLocker();

            #region networkEvent

            //不開出來填的原因是 小黑窗所在的網域即為伺服器所在的機器才對
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

                CursorModule.Instance.WriteLine("客戶端 {0} 成功連接", clientSocket.RemoteEndPoint.ToString());

                //把每個客戶端的thread錯開
                Thread clientThead = new Thread(RecieveClientMessage);
                clientThead.Start(clientSocket);

                receiveMessageDict.Add(clientSocket, clientThead);

                //基於處理上的簡易化 目前規格要握手結束後 才能接受下一個client的登入
                ClientSharkHand(clientSocket, 5);
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
                        ReceiveNetworkMsg(m_ClientSocket, result);
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

        void ClientSharkHand(Socket clientSocket, int timeout)
        {
            ushort newUserUDID = 0;
            string receiveToken = "";
            bool successReceive = false;
            ushort newUserReqHeader = Consts.NetworkEvents.NewUserReq;
            ushort receiveNewUserReqHeader = Consts.NetworkEvents.NewUserRes;

            Action<Socket, string> receiveNewUserRes = (receiveSocket, msgJson) =>
            {
                if (receiveSocket == clientSocket)
                {
                    NewUserRes newUserRes = JsonUtility.FromJson<NewUserRes>(msgJson);
                    receiveToken = newUserRes.Token;
                    successReceive = true;
                }
            };

            lock (identityCheckLocker)
            {
                //先把自己以外的玩家的資訊收集起來
                List<UserData> userDatas = new List<UserData>();

                foreach (var item in userDataPairSocketTable)
                {
                    userDatas.Add(item.Value);
                }

                //把屬於新的client的UDID 加在最後
                newUserUDID = GetUDID();

                NewUserReq userDataGroup = new NewUserReq() { UserDatas = userDatas, NewUserUDID = newUserUDID };

                byte[] userIdentityGroupMsg = TransmitterUtility.GetToClientMsg(newUserReqHeader, userDataGroup);
                BindNetworkEvent(receiveNewUserReqHeader, receiveNewUserRes);
                clientSocket.Send(userIdentityGroupMsg);

                waitSharkHandSocket = clientSocket;
            }

            //5秒內client沒回應視同斷線
            SpinWait.SpinUntil(() =>
            {
                return successReceive;
            }, timeout * 1000);

            UnBindNetworkEvent(receiveNewUserReqHeader, receiveNewUserRes);

            if (successReceive)
            {
                CursorModule.Instance.WriteLine($"{clientSocket.RemoteEndPoint.ToString()} -> token -> {receiveToken}");

                UserData userData = UserData.Create(newUserUDID, receiveToken);

                lock (identityCheckLocker)
                {
                    userDataPairSocketTable.Add(clientSocket, userData);

                    waitSharkHandSocket = null;

                    //先把自己登入的消息傳給自己以外的人 再把自己加入列表
                    SendAddUserData(userData, clientSockets);
                    clientSockets.Add(clientSocket);
                }
            }
            else
            {
                CursorModule.Instance.WriteLine($"{clientSocket.RemoteEndPoint.ToString()}");
            }
        }

        public void ReceiveNetworkMsg(Socket socket, byte[] msg)
        {
            MemoryStream memoryStream = new MemoryStream(msg);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            ushort header = 0;

            int contentLength = 0;

            byte[] msgContent = null;

            try
            {
                header = binaryReader.ReadUInt16();

                contentLength = (int)binaryReader.ReadUInt16();

                msgContent = binaryReader.ReadBytes(contentLength);
            }
            catch (Exception e)
            {
                CursorModule.Instance.WriteLine(e.Message);
            }
            finally 
            {
                memoryStream.Dispose();
                binaryReader.Dispose();
            }

            //如果是遊戲間溝通的封包 直接轉送給其他玩家
            if (header == Consts.NetworkEvents.GameMessage)
            {
                //因為是保留轉送 所以要保持檔頭直接轉送
                ReceiveGameMessage(msg);
            }
            else
            {
                TriggerNetworkEvent(socket, header, msgContent);
            }
        }

        void TriggerNetworkEvent(Socket socket, ushort header, byte[] msg)
        {
            lock (networkEventsDictLocker)
            {
                List<Action<Socket, string>> caches = null;

                if(networkEventsDict.TryGetValue(header, out caches))
                {
                    caches.ForEach(cache=> 
                    {
                        string msgJson = TransmitterUtility.ParseBufferToString(msg);

                        cache.Invoke(socket, msgJson);
                    });
                }
                else
                {
                    CursorModule.Instance.WriteLine($"找不到對應的註冊事件 header -> {header}");
                }
            }
        }

        /// <summary>
        /// 遊戲端互相溝通的 直接轉送給全部玩家即可
        /// </summary>
        /// <param name="msg"></param>
        void ReceiveGameMessage(byte[] msg)
        {
            lock (identityCheckLocker)
            {
                clientSockets.ForEach(clientSocket =>
                {
                    TrySendMsgToSocket(clientSocket, msg);
                });            
            }

        }

        public void BindNetworkEvent(ushort eventHeader, Action<Socket, string> callback)
        {
            lock (networkEventsDictLocker)
            {
                List<Action<Socket, string>> callbacks = null;

                if (!networkEventsDict.TryGetValue(eventHeader, out callbacks))
                {
                    callbacks = new List<Action<Socket, string>>();
                    networkEventsDict.Add(eventHeader, callbacks);
                }

                callbacks.Add(callback);
            }
        }

        public void UnBindNetworkEvent(ushort eventHeader, Action<Socket, string> callback)
        {
            lock (networkEventsDictLocker)
            {
                List<Action<Socket, string>> callbacks = null;

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

        void RemoveClientSocket(Socket clientSocket)
        {

            lock (identityCheckLocker)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                Thread receiveThread = receiveMessageDict[clientSocket];
                receiveMessageDict.Remove(clientSocket);
                receiveThread.Abort();

                clientSockets.Remove(clientSocket);

                List<Socket> needSendRemoveSockets = new List<Socket>(clientSockets);

                if (waitSharkHandSocket != null)
                {
                    needSendRemoveSockets.Add(waitSharkHandSocket);
                }

                UserData removeUserData = userDataPairSocketTable[clientSocket];
                userDataPairSocketTable.Remove(clientSocket);
                SendRemoveUserData(removeUserData, needSendRemoveSockets);
            }
        }

        //還在locker內 不要再次locker 可能是在發完訊息後 發現訊息發不出去 就直接當作斷線
        void SendRemoveClientSocketInLocker(List<Socket> originSockets, List<Socket> needRemoveScoekts)
        {
            List<Socket> needSendSockets = new List<Socket>(originSockets);
            List<UserData> removeUserDatas = new List<UserData>();

            needRemoveScoekts.ForEach(socket =>
            {
                needSendSockets.Remove(socket);

                UserData removeUserData = userDataPairSocketTable[socket];
                userDataPairSocketTable.Remove(socket);
                removeUserDatas.Add(removeUserData);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                Thread receiveThread = receiveMessageDict[socket];
                receiveMessageDict.Remove(socket);
                receiveThread.Abort();
            });

            this.needRemoveSockets.Clear();

            removeUserDatas.ForEach(userData =>
            {
                SendRemoveUserData(userData, needSendSockets);
            });

            CheckNeedRemoveSockets();
        }

        /// <summary>
        /// 如果還是有新發現的socket訊息發不出去 就依然把它斷線
        /// </summary>
        void CheckNeedRemoveSockets()
        {
            if (needRemoveSockets.Count > 0)
            {
                SendRemoveClientSocketInLocker(clientSockets, needRemoveSockets);
            }
        }

        void SendRemoveUserData(UserData userData, List<Socket> targetSockets)
        {
            byte[] msg = TransmitterUtility.GetToClientMsg(Consts.NetworkEvents.RemoveUser, userData);

            targetSockets.ForEach(socket => TrySendMsgToSocket(socket, msg));
        }

        void SendAddUserData(UserData userData, List<Socket> targetSockets)
        {
            byte[] msg = TransmitterUtility.GetToClientMsg(Consts.NetworkEvents.AddUser, userData);

            targetSockets.ForEach(socket => TrySendMsgToSocket(socket, msg));

            CheckNeedRemoveSockets();
        }

        ushort enquence = 0;

        /// <summary>
        /// 程序開始啟動後 每當client被生成出來 就申請一個流水號
        /// </summary>
        /// <returns></returns>
        ushort GetUDID()
        {
            enquence++;
            return enquence;
        }

        void InitLocker()
        {
            identityCheckLocker = new object();

            networkEventsDictLocker = new object();
        }

        void TrySendMsgToSocket(Socket socket, byte[] buffer)
        {
            try
            {
                socket.Send(buffer);
            }
            catch (Exception e)
            {
                CursorModule.Instance.WriteLine(e.Message);
                //無法對他傳訊息 先當作斷線處理
                needRemoveSockets.Add(socket);
            }
        }

        List<Socket> needRemoveSockets = new List<Socket>();
    }
}
