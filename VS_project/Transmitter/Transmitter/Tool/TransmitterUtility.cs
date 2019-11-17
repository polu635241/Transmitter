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
        public static Byte[] GetToClientGameMsg(string channelName, string eventName, string msg)
        {
            ushort msgHeader = Consts.NetworkEvents.GameMessage;

            MsgParseData msgParseData = MsgParseData.Create(channelName, eventName, msg);

            byte[] msgParseDataBuffer = msgParseData.GetBuffer();

            return GetToClientMsg(msgHeader, msgParseDataBuffer);
        }

        public static Byte[] GetToClientMsg(ushort msgHeader, string jsonMsg)
        {
            byte[] msg = ParseStringToBuffer(jsonMsg);

            return GetToClientMsg(msgHeader, jsonMsg);
        }

        public static Byte[] GetToClientMsg(ushort msgHeader, byte[] msg)
        {
            byte[] msgHeaderBuffer = BitConverter.GetBytes(msgHeader);

            byte[] fullMsg = Tool.Combine(msgHeaderBuffer, msg);

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

        public class MsgParseData
        {
            #region 合成公式
            // int (頻道長度)
            // string (頻道)

            // int (事件長度)
            // string (事件)

            //ushort (長度)
            //byte[] (msg)

            #endregion

            string channelName;

            public string ChannelName
            {
                get
                {
                    return channelName;
                }
            }

            string eventName;

            public string EventName
            {
                get
                {
                    return eventName;
                }
            }

            string msg;

            /// <summary>
            /// 基於 版本相容性的問題 unity 對 unity傳遞才會透過 byte封裝 asp 對 unity的封裝透過json傳遞
            /// </summary>
            public string Msg
            {
                get
                {
                    return msg;
                }
            }

            MsgParseData()
            {

            }

            public static MsgParseData Create(string channelName, string eventName, string msg)
            {
                MsgParseData msgParseData = new MsgParseData();

                msgParseData.channelName = channelName;
                msgParseData.eventName = eventName;
                msgParseData.msg = msg;

                return msgParseData;
            }

            public static MsgParseData CreateFromMsg(byte[] msg)
            {
                MsgParseData msgParseData = new MsgParseData();

                MemoryStream memoryStream = null;
                BinaryReader binaryReader = null;

                try
                {
                    memoryStream = new MemoryStream(msg);
                    binaryReader = new BinaryReader(memoryStream);

                    msgParseData.channelName = binaryReader.ReadString();

                    msgParseData.eventName = binaryReader.ReadString();

                    msgParseData.msg = binaryReader.ReadString();
                }
                catch (Exception e)
                {
                    CursorModule.Instance.WriteLine(e.Message);
                }
                finally
                {
                    memoryStream?.Dispose();
                    binaryReader?.Dispose();
                }

                return msgParseData;
            }

            public byte[] GetBuffer()
            {
                byte[] buffer = null;
                MemoryStream memoryStream = null;
                BinaryWriter binaryWriter = null;

                try
                {
                    memoryStream = new MemoryStream();
                    binaryWriter = new BinaryWriter(memoryStream);

                    //寫入頻道名稱
                    byte[] channelbytes = ParseStringToBuffer(channelName);
                    binaryWriter.Write((ushort)channelbytes.Length);
                    binaryWriter.Write(channelbytes);

                    //寫入事件名稱
                    byte[] eventbytes = ParseStringToBuffer(eventName);
                    binaryWriter.Write((ushort)eventbytes.Length);
                    binaryWriter.Write(eventbytes);

                    //寫入傳遞給client的訊息
                    binaryWriter.Write(msg);

                    binaryWriter.Flush();
                    buffer = memoryStream.ToArray();

                }
                catch (Exception e)
                {
                    CursorModule.Instance.WriteLine(e.Message);
                }
                finally
                {
                    memoryStream?.Dispose();
                    binaryWriter?.Dispose();
                }
                return buffer;
            }
        }
    }
}
