using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transmitter.Tool;

namespace Transmitter.Manager
{
    public class CursorModule:Singleton<CursorModule>
    {
        Dictionary<string, List<Action<List<string>>>> callbackPairTable = new Dictionary<string, List<Action<List<string>>>>();

        object cursorLocker;

        List<string> commandCaches = new List<string>();
        int lastCommandIndex;
        string readText = "";
        Thread readInputThread;

        public CursorModule()
        {
            this.cursorLocker = new object();

            readInputThread = new Thread(ReadInput);
            readInputThread.Start();
        }

        public void BindKeyWordCallback(string key, Action<List<string>> callback)
        {
            List<Action<List<string>>> callbacks;

            if (!callbackPairTable.TryGetValue(key, out callbacks))
            {
                callbacks = new List<Action<List<string>>>();
                callbackPairTable.Add(key, callbacks);
            }

            callbacks.Add(callback);
        }

        public void UnBindKeyWordCallback(string key, Action<List<string>> callback)
        {
            List<Action<List<string>>> callbacks;

            if (!callbackPairTable.TryGetValue(key, out callbacks))
            {
                throw new Exception("找不到對應的key");
            }

            bool result = callbacks.Remove(callback);

            if (!result)
                throw new Exception("找不到對應的事件緩存");
        }

        void ReadInput()
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.Backspace && readText.Length > 0)
                {
                    DeletOneChar();
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    MoveToPrevCmd();
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    MoveToNextCmd();
                }
                else if (key.Key == ConsoleKey.Enter && readText.Length > 0)
                {
                    FlushCmdLine();
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    ExitApplication();
                }
                else if (char.IsControl(key.KeyChar) == false)
                {
                    WriteOneChar(key);
                }

            }
        }

        void DeletOneChar()
        {
            lock (cursorLocker)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(readText + "\b \b");
                readText = readText.Substring(0, readText.Length - 1);
            }
        }

        void MoveToPrevCmd()
        {
            if (commandCaches.Count > 0 && lastCommandIndex > 0)
            {
                readText = commandCaches[--lastCommandIndex];
                lock (cursorLocker)
                {
                    int currentTop = Console.CursorTop;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, currentTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentTop);
                    Console.Write(readText);
                }
            }
            else
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }

        void MoveToNextCmd()
        {
            if (commandCaches.Count > 0 && lastCommandIndex < commandCaches.Count - 1)
            {
                readText = commandCaches[++lastCommandIndex];
                lock (cursorLocker)
                {
                    int currentTop = Console.CursorTop;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, currentTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentTop);
                    Console.Write(readText);
                }
            }
            else
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }

        void FlushCmdLine()
        {
            lock (cursorLocker)
            {
                Console.WriteLine(readText);
            }

            bool newOne = commandCaches.CheckAdd(readText);

            if (newOne)
            {
                lastCommandIndex = commandCaches.Count;
            }

            ReceiveMessage(readText);

            readText = "";
        }

        void ExitApplication()
        {
            Environment.Exit(Environment.ExitCode);
        }

        void WriteOneChar(ConsoleKeyInfo key)
        {
            readText += key.KeyChar;

            lock (cursorLocker)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(readText);
            }
        }

        void ReceiveMessage(string message)
        {
            string[] spiltStr = message.Split(new string[] { " : ", ", " }, StringSplitOptions.None);

            string eventName = spiltStr[0];
            List<string> callbackPars = new List<string>();

            //長度超過1代表有帶參數
            if (spiltStr.Length > 1)
            {
                for (int i = 1; i < spiltStr.Length; i++)
                {
                    callbackPars.Add(spiltStr[i]);
                }
            }

            List<Action<List<string>>> callbacks;

            if (callbackPairTable.TryGetValue(eventName, out callbacks))
            {
                callbacks.ForEach(callback => callback.Invoke(callbackPars));
            }
            else
            {
                lock (cursorLocker)
                {
                    Console.WriteLine($"找不到對應的 事件緩存 {message} -> {eventName}");
                }
            }
        }
    }
}
