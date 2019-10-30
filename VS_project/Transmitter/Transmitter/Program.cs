using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using Transmitter.Manager;

namespace Transmitter
{
    class Program
    {
        #region locker 
        static object cursorLocker = new object();
        #endregion

        static KeyInputManager keyInputManager;
        static NetworkManager networkManager;

        const int defaultPort = 9487;

        /// <summary>
        /// port號可以由啟動路徑傳入
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int port = 0;

            if (args.Length != 0)
            {
                int parseResult;
                bool success = int.TryParse(args[0], out parseResult);

                if (success)
                {
                    port = parseResult;
                    Console.WriteLine($"使用傳入值作為port開啟連線 port -> {parseResult}");
                }
                else
                {
                    Console.WriteLine($"轉換失敗 傳入值並非數字 args -> {args[0]} " +
                        $"使用預設port開啟連線 port -> {defaultPort}");
                }
            }
            else
            {
                Console.WriteLine($"使用預設port開啟連線 port -> {defaultPort}");
                port = defaultPort;
            }

            InitManager(port);

            SpinWait.SpinUntil(() => false);
        }

        static void InitManager(int port)
        {
            #region Input Manager
            keyInputManager = new KeyInputManager(cursorLocker);
            keyInputManager.BindKeyWordCallback("Send", SendMessageToClients);
            #endregion

            #region NetWork Manager
            networkManager = new NetworkManager(cursorLocker, port);
            #endregion

        }

        static void SendMessageToClients(List<string> pars)
        {
            try
            {
                string channelName = pars[0];
                string eventName = pars[1];
                string msg = pars[2];

                lock (cursorLocker)
                {
                    Console.WriteLine($"channelName -> {channelName}, eventName -> {eventName}, msg -> {msg}");
                }
            }
            catch (Exception e)
            {
                lock (cursorLocker)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
