using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Transmitter.Tool
{
    public class TransmitterUtility
    {
        public Byte[] GetToClientMsg(string channelName, string eventName, string msg)
        {
            MsgParseData msgParseData = new MsgParseData(channelName, eventName, msg);

            return msgParseData.GetBuffer();
        }

        class MsgParseData
        {
            #region 合成公式
            // int (頻道長度)
            // string (頻道)

            // int (事件長度)
            // string (事件)

            // int (有幾個參數)

            // Loop :
            // 參數型態
            // 參數長度
            // 參數
            #endregion

            string channelName;
            string eventName;
            string msg = "";

            public MsgParseData(string channelName, string eventName, string msg)
            {
                this.channelName = channelName;
                this.eventName = eventName;
                this.msg = msg;
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
                    byte[] channelbytes = Encoding.UTF8.GetBytes(channelName);
                    binaryWriter.Write((ushort)channelbytes.Length);
                    binaryWriter.Write(channelbytes);

                    //寫入事件名稱
                    byte[] eventbytes = Encoding.UTF8.GetBytes(eventName);
                    binaryWriter.Write((ushort)eventbytes.Length);
                    binaryWriter.Write(eventbytes);

                    //寫入傳遞給client的事件
                    binaryWriter.Write((ushort)1);
                    string fullName = typeof(string).FullName;
                    binaryWriter.Write(fullName);
                    byte[] objBuffer = ParseStringToBytes(msg);
                    binaryWriter.Write((ushort)objBuffer.Length);
                    binaryWriter.Write(objBuffer);

                    binaryWriter.Flush();
                    buffer = memoryStream.ToArray();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    memoryStream?.Dispose();
                    binaryWriter?.Dispose();
                }
                return buffer;
            }
            Byte[] ParseStringToBytes(string msg)
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(msg);
                return memoryStream.GetBuffer();
            }
        }
    }
}
