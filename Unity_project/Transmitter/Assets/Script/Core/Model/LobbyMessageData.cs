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
using Transmitter.DataStruct;

namespace Transmitter.Net.Model
{
	[Serializable]
	public class LobbyMessageData
	{
		ushort header;

		public ushort Header
		{
			get
			{
				return header;
			}
		}

		byte[] msg;

		public byte[] Msg
		{
			get
			{
				return msg;
			}
		}

		#region 合成公式
		// ushort (標頭)

		// ushort (內容長度)
		// byte[] (內容)

		// int (有幾個參數)

		// Loop :
		// 參數型態
		// 參數長度
		// 參數
		#endregion

		/// <summary>
		/// 透過實體物件建構
		/// </summary>
		public static LobbyMessageData Create(ushort header, byte[] msg)
		{
			
			LobbyMessageData messageData = new LobbyMessageData ();
			messageData.header = header;
			messageData.msg = msg;

			return messageData;
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

				binaryWriter.Write(header);

				ushort msgLength = (ushort)msg.Length;

				binaryWriter.Write(msgLength);
				binaryWriter.Write(msg);

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
		public static LobbyMessageData CreateByMsg(byte[] msg)
		{
			LobbyMessageData messageData = new LobbyMessageData ();
			
			MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;

			try
			{
				memoryStream = new MemoryStream(msg);
				binaryReader = new BinaryReader(memoryStream);

				messageData.header = binaryReader.ReadUInt16();

				int msgBufferLength = (int)binaryReader.ReadUInt16();

				msg = binaryReader.ReadBytes(msgBufferLength);

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