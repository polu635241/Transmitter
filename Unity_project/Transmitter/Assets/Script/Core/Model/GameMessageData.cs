﻿using System;
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

namespace Transmitter.Model
{
	[Serializable]
	public class GameMessageData
	{
		short assignUdid;

		public short AssignUdid
		{
			get
			{
				return assignUdid;
			}
		}
		
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
		// short  發送對象 -1為全域

		// string (頻道)

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
		public static GameMessageData Create(short assignUdid,string channelName,string eventName,params object[] objs)
		{
			GameMessageData messageData = new GameMessageData ();
			messageData.assignUdid = assignUdid;
			messageData.channelName = channelName;
			messageData.eventName = eventName;
			messageData.objs = objs;

			return messageData;
		}

		public byte[] GetBuffer(Func<string,object,byte[]> serializeObjToBuffer)
		{
			byte[] buffer = null;
			MemoryStream memoryStream = null;
			BinaryWriter binaryWriter = null;

			try
			{
				memoryStream = new MemoryStream();
				binaryWriter = new BinaryWriter(memoryStream);

				ushort gameMsgHeader = Consts.NetworkEvents.GameMessage;
				binaryWriter.Write(gameMsgHeader);

				byte[] contentBuffer = GetContentBuffer(serializeObjToBuffer);
				ushort contentBufferLength = (ushort)contentBuffer.Length;

				binaryWriter.Write(contentBufferLength);
				binaryWriter.Write(contentBuffer);

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
		/// 內部作為格式化之用 由外部傳入將單一object轉換成byte[]方法
		/// </summary>
		/// <returns>The buffer.</returns>
		/// <param name="serializeObjToBuffer">Serialize object to buffer.</param>
		byte[] GetContentBuffer(Func<string,object,byte[]> serializeObjToBuffer)
		{
			byte[] buffer = null;
			MemoryStream memoryStream = null;
			BinaryWriter binaryWriter = null;

			try
			{
				memoryStream = new MemoryStream();
				binaryWriter = new BinaryWriter(memoryStream);

				//寫入發送對象
				binaryWriter.Write(assignUdid);

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

						//改為先寫長度 這樣偵測到空物件時 才能直接寫0 空物件抓不到 type name
						if(_object!=null)
						{
							string fullName = _object.GetType().FullName;
							byte[] objBuffer = serializeObjToBuffer(fullName, objs[i]);
							binaryWriter.Write((ushort)objBuffer.Length);
							binaryWriter.Write(fullName);
							binaryWriter.Write(objBuffer);
						}
						else
						{
							binaryWriter.Write((ushort)0);
						}
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
		public static GameMessageData CreateByMsg(Func<string,byte[],object> deserializeToObject ,byte[] byteData)
		{
			GameMessageData messageData = new GameMessageData ();
			
			MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;

			try
			{
				memoryStream = new MemoryStream(byteData);
				binaryReader = new BinaryReader(memoryStream);

				messageData.assignUdid = binaryReader.ReadInt16();

				messageData.channelName = binaryReader.ReadString();

				messageData.eventName = binaryReader.ReadString();

				short parsCount = binaryReader.ReadInt16 ();

				if (parsCount >= 0) 
				{
					List<object> parTable = new List<object> ();

					for (int i = 0; i < parsCount; i++) 
					{
						object obj = null;

						//改為先讀長度 這樣讀到0時 才能直接null 空物件抓不到 type name
						ushort parBufferLen = binaryReader.ReadUInt16 ();

						if(parBufferLen!=0)
						{
							string objectTypeFullName = binaryReader.ReadString();
							byte[] parBuffer = binaryReader.ReadBytes (parBufferLen);
							
							obj = deserializeToObject(objectTypeFullName,parBuffer);
						}
						else
						{
							obj = null;
						}

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