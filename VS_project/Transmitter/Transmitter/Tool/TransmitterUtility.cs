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
        public static Byte[] GetToClientMsg(ushort msgHeader, object msg)
        {
            //基於跨版本的相容性 unity與vs的溝通 透過Json傳遞 client 與 client的溝通 才會完全的序列化成byte[]
            string jsonMsg = JsonUtility.ToJson(msg);

            return GetToClientMsg(msgHeader, jsonMsg);
        }

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
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(msg);

            return memoryStream.GetBuffer();
        }

        public static string ParseBufferToString(byte[] msg)
        {
            MemoryStream memoryStream = new MemoryStream(msg);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            return binaryReader.ReadString();
        }
    }
}
