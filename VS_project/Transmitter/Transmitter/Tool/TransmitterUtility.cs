using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Transmitter.Tool;
using Transmitter.DataStruct;
using Transmitter.Plugin;

namespace Transmitter.Tool
{
    public class TransmitterUtility
    {
        public static Byte[] GetToClientMsg(ushort msgHeader, string jsonMsg)
        {
            byte[] msg = ParseStringToBuffer(jsonMsg);

            return GetToClientMsg(msgHeader, msg);
        }

        public static Byte[] GetToClientMsg(ushort msgHeader, byte[] msg)
        {
            byte[] msgHeaderBuffer = BitConverter.GetBytes(msgHeader);

            ushort msgLength = (ushort)msg.Length;
            byte[] msgLengthBuffer = BitConverter.GetBytes(msgLength);


            byte[] fullMsg = Tool.Combine(msgHeaderBuffer, msgLengthBuffer, msg);

            return fullMsg;
        }

        /// <summary>
        /// 封裝用　後續如果需要修改編碼方式　直接更改封裝方法就好
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] ParseStringToBuffer(string msg)
        {
            return Encoding.Default.GetBytes(msg);
        }

        public static string ParseBufferToString(byte[] msg)
        {
            return Encoding.Default.GetString(msg);
        }
    }
}
