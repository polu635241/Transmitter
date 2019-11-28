using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Transmitter.Serialize
{
	public class MessageUtility {

		public static byte[] ProcessLengthInTitle(byte[] message)
		{
			List<byte> results = new List<byte> ();
			Byte[] titleBytes = BitConverter.GetBytes ((ushort)message.Length);
			results.AddRange (titleBytes);
			results.AddRange (message);
			return results.ToArray ();
		}

		public static object ByteArrayToGeneralObject(byte[] bytes)
		{
			MemoryStream memoryStream = null;
			object result = null;
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				memoryStream = new MemoryStream (bytes);
				result = binaryFormatter.Deserialize (memoryStream);
			}
			catch(Exception e) 
			{
				Debug.LogError (e.Message);

			}
			finally 
			{
				memoryStream?.Dispose ();
			}
			return result;
		}

		public static byte[] GeneralObjectToByteArray (object obj)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			byte[] buffer = null;
			try
			{
				binaryFormatter.Serialize(memoryStream, obj);
				buffer = memoryStream.GetBuffer ();
			}
			catch (Exception e)
			{
				Debug.LogError (e.Message);
			}
			finally
			{
				memoryStream?.Dispose ();
			}
			return buffer;
		}
	}
}
