using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Serialize.Tool;

namespace Transmitter.Serialize
{
	public class ObjectSerialize_Base 
	{
		public virtual void Init()
		{
			serializeFunctionTable = new Dictionary<string, Func<object, byte[]>> ();
			serializeFunctionTable.Add ("System.Boolean", BoolConvertToBuffer);
			serializeFunctionTable.Add ("System.Byte", ByteConvertToBuffer);
			serializeFunctionTable.Add ("System.SByte", SbyteConvertToBuffer);
			serializeFunctionTable.Add ("System.Char", CharConvertToBuffer);
			serializeFunctionTable.Add ("System.Decimal", DecimalConvertToBuffer);
			serializeFunctionTable.Add ("System.Double", DoubleConvertToBuffer);
			serializeFunctionTable.Add ("System.Single", FloatConvertToBuffer);
			serializeFunctionTable.Add ("System.Int32", IntConvertToBuffer);
			serializeFunctionTable.Add ("System.UInt32", UintConvertToBuffer);
			serializeFunctionTable.Add ("System.Int64", LongConvertToBuffer);
			serializeFunctionTable.Add ("System.Int16", ShortConvertToBuffer);
			serializeFunctionTable.Add ("System.UInt16", UShortConvertToBuffer);
			serializeFunctionTable.Add ("System.String", StringConvertToBuffer);
        }

		public byte[] SerializeToBuffer (string fullTypeName, System.Object msg)
		{
			Func<object,byte[]> processMessageToObject = null;

			//如果map內找不到 就表示這個type是使用者自己創的type 不是透過生產器生的
			if (serializeFunctionTable.TryGetValue (fullTypeName, out processMessageToObject)) 
			{
				return processMessageToObject.Invoke (msg);
			}
			else
			{
				return MessageUtility.GeneralObjectToByteArray (msg);
			}
		}

		byte[] BoolConvertToBuffer(System.Object _msg)
		{
			bool msg = (bool)_msg;
			
			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_bool(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
		}

		byte[] ByteConvertToBuffer(System.Object _msg)
		{
			byte msg = (byte)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_byte(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] SbyteConvertToBuffer(System.Object _msg)
		{
			sbyte msg = (sbyte)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_sbyte(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] CharConvertToBuffer(System.Object _msg)
		{
			char msg = (char)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_char(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] DecimalConvertToBuffer(System.Object _msg)
		{
			decimal msg = (decimal)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_decimal(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] DoubleConvertToBuffer(System.Object _msg)
		{
			double msg = (double)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_double(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] FloatConvertToBuffer(System.Object _msg)
		{
			float msg = (float)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_float(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] IntConvertToBuffer(System.Object _msg)
		{
			int msg = (int)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_int(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] UintConvertToBuffer(System.Object _msg)
		{
			uint msg = (uint)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_uint(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] LongConvertToBuffer(System.Object _msg)
		{
			long msg = (long)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_long(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] ShortConvertToBuffer(System.Object _msg)
		{
			short msg = (short)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_short(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] UShortConvertToBuffer(System.Object _msg)
		{
			ushort msg = (ushort)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_ushort(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		byte[] StringConvertToBuffer(System.Object _msg)
		{
			string msg = (string)_msg;

			byte[] result = null;
			ObjectSerilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectSerilizeBuffer();
				result = buffer.Write_string(msg);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally 
			{
				buffer?.Close ();
			}

			return result;
        }

		protected Dictionary<string,Func<object,byte[]>> serializeFunctionTable;
	}

	public class ObjectSerilizeBuffer
	{
		MemoryStream memoryStream = null;
		BinaryWriter binaryWriter = null;

		public ObjectSerilizeBuffer()
		{
			memoryStream = new MemoryStream ();
			binaryWriter = new BinaryWriter (memoryStream);
        }

		public byte[] Write_bool(bool msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_byte(byte msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_sbyte(sbyte msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_char(char msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_decimal(decimal msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_double(double msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_float(float msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_int(int msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_uint(uint msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_long(long msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_short(short msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_ushort(ushort msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public byte[] Write_string(string msg)
		{
			binaryWriter.Write (msg);
			return memoryStream.GetBuffer ();
		}

		public void Close()
		{
			memoryStream?.Close ();
			binaryWriter?.Close ();
		}
	}
}