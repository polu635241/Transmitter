using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.Net.Model
{
	public class UserIdentity
	{
		public string IP
		{
			get
			{
				return ip;
			}
		}

		string ip;

		public string Token
		{
			get
			{
				return token;
			}
		}

		string token;

		UserIdentity(){}

		public static UserIdentity CreateByMsg(byte[] msg)
		{
			UserIdentity userIdentity = new UserIdentity ();
			
            MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;

            try
            {
				memoryStream = new MemoryStream(msg);
				binaryReader = new BinaryReader(memoryStream);

                //讀取IP
				userIdentity.ip = binaryReader.ReadString();

                //讀取Token
				userIdentity.token = binaryReader.ReadString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                memoryStream?.Dispose();
                binaryReader?.Dispose();
            }

			return userIdentity;
		}

		public static List<UserIdentity> CreateTableByMsg(byte[] msg)
		{
			List<UserIdentity> userIdentitys = new List<UserIdentity> ();

			MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;

			try
			{
				memoryStream = new MemoryStream(msg);
				binaryReader = new BinaryReader(memoryStream);

				int tableCount = (int)binaryReader.ReadUInt16();

				for (int i = 0; i < tableCount; i++) 
				{
					int bufferLength = (int)binaryReader.ReadUInt16();
					byte[] itemBuffer = binaryReader.ReadBytes(bufferLength);

					UserIdentity userIdentity = UserIdentity.CreateByMsg(itemBuffer);

					userIdentitys.Add(userIdentity);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				memoryStream?.Dispose();
				binaryReader?.Dispose();
			}

			return userIdentitys;
		}
	}
}