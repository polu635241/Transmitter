using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Transmitter.Net.Model
{
	[Serializable]
	public class MessageData
	{
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

		object[] objs;

		public object[] Objs
		{
			get
			{
				return objs;
			}
		}

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

		/// <summary>
		/// 透過實體物件建構
		/// </summary>
		public static MessageData Create(string channelName,string eventName,params object[] objs)
		{
			
			MessageData messageData = new MessageData ();
			messageData.channelName = channelName;
			messageData.eventName = eventName;
			messageData.objs = objs;

			return messageData;
		}

		/// <summary>
		/// 內部作為格式化之用 由外部傳入將單一object轉換成byte[]方法
		/// </summary>
		/// <returns>The buffer.</returns>
		/// <param name="serializeObjToBuffer">Serialize object to buffer.</param>
		public byte[] GetBuffer(Func<string,object,byte[]> serializeObjToBuffer)
		{
			byte[] buffer = null;
			MemoryStream memoryStream = null;
			BinaryWriter binaryWriter = null;

			try
			{
				memoryStream = new MemoryStream();
				binaryWriter = new BinaryWriter(memoryStream);

				//寫入頻道名稱
				binaryWriter.Write(channelName);

				//寫入事件名稱
				binaryWriter.Write(eventName);

				if(objs==null)
				{
					binaryWriter.Write((short)-1);
				}
				else
				{
					binaryWriter.Write((short)objs.Length);

					for (int i = 0; i < objs.Length; i++) 
					{
						object _object = objs[i];
						string fullName = _object.GetType().FullName;
						binaryWriter.Write(fullName);
						byte[] objBuffer = serializeObjToBuffer(fullName, objs[i]);
						binaryWriter.Write((ushort)objBuffer.Length);
						binaryWriter.Write(objBuffer);
					}
				}

				binaryWriter.Flush();
				buffer = memoryStream.ToArray();

			}
			catch (Exception e) 
			{
				Debug.LogError (e.Message);
			}
			finally
			{
				memoryStream?.Dispose ();
				binaryWriter?.Dispose ();
			}
			return buffer;
		}

		/// <summary>
		/// 透過封包建構 外部傳入將單一段byte[]轉換成object的方法 作為格式化的依據
		/// </summary>
		/// <param name="byteData">Byte data.</param>
		public static MessageData CreateByMsg(Func<string,byte[],object> deserializeToObject ,byte[] byteData)
		{
			MessageData messageData = new MessageData ();
			
			MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;

			try
			{
				memoryStream = new MemoryStream(byteData);
				binaryReader = new BinaryReader(memoryStream);

				messageData.channelName = binaryReader.ReadString();

				messageData.eventName = binaryReader.ReadString();

				short parsCount = binaryReader.ReadInt16 ();

				if (parsCount >= 0) 
				{
					List<object> parTable = new List<object> ();

					for (int i = 0; i < parsCount; i++) 
					{
						string objectTypeFullName = binaryReader.ReadString();
						ushort parBufferLen = binaryReader.ReadUInt16 ();
						byte[] parBuffer = binaryReader.ReadBytes (parBufferLen);

						object obj = deserializeToObject(objectTypeFullName,parBuffer);

						parTable.Add(obj);
					}

					messageData.objs = parTable.ToArray ();
				}
				else
				{
					//長度為負數 代表值為 null
					messageData.objs = null;
				}
			}
			catch(Exception e) 
			{
				Debug.LogError (e.Message);
			}
			finally
			{
				memoryStream?.Dispose ();
				binaryReader?.Dispose ();
			}

			return messageData;
		}
	}	
}