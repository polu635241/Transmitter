using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Serialize.Tool;

namespace Transmitter.Serialize
{
	public class ObjectDeserialize_Base 
	{
		public virtual void Init()
		{
			deserializeFunctionTable = new Dictionary<string, Func<byte[], object>> ();
			deserializeFunctionTable.Add ("System.Boolean", ConvertToBool);
			deserializeFunctionTable.Add ("System.Byte", ConvertToByte);
			deserializeFunctionTable.Add ("System.SByte", ConvertToSByte);
			deserializeFunctionTable.Add ("System.Char", ConvertToChar);
			deserializeFunctionTable.Add ("System.Decimal", ConvertToDecimal);
			deserializeFunctionTable.Add ("System.Double", ConvertToDouble);
			deserializeFunctionTable.Add ("System.Single", ConvertToFloat);
			deserializeFunctionTable.Add ("System.Int32", ConvertToInt);
			deserializeFunctionTable.Add ("System.UInt32", ConvertToUInt);
			deserializeFunctionTable.Add ("System.Int64", ConvertToLong);
			deserializeFunctionTable.Add ("System.Int16", ConvertToShort);
			deserializeFunctionTable.Add ("System.UInt16", ConvertToUShort);
			deserializeFunctionTable.Add ("System.String", ConvertToString);
        }

		public object DeserializeToObject (string fullTypeName, byte[] msg)
		{
			Func<byte[], object> processMessageToObject = null;

			//如果map內找不到 就表示這個type是使用者自己創的type 不是透過生產器生的
			if (deserializeFunctionTable.TryGetValue (fullTypeName, out processMessageToObject)) 
			{
				return processMessageToObject.Invoke (msg);
			}
			else
			{
				return MessageUtility.ByteArrayToGeneralObject (msg);
			}
		}

		object ConvertToBool(byte[] msg)
		{
			bool result = false;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_bool ();
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

        object ConvertToByte(byte[] msg)
        {
			byte result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_byte ();
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

        object ConvertToSByte(byte[] msg)
        {
			sbyte result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_sbyte ();
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

        object ConvertToChar(byte[] msg)
        {
			char result = ' ';
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_char ();
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

        object ConvertToDecimal(byte[] msg)
        {
			decimal result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_decimal ();
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

        object ConvertToDouble(byte[] msg)
        {
			double result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_double ();
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

        object ConvertToFloat(byte[] msg)
        {
			float result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_float ();
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

        object ConvertToInt(byte[] msg)
        {
			int result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_int ();
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

        object ConvertToUInt(byte[] msg)
        {
			uint result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_uint ();
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

        object ConvertToLong(byte[] msg)
        {
			long result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_long ();
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

        object ConvertToShort(byte[] msg)
        {
			short result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_short ();
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

        object ConvertToUShort(byte[] msg)
        {
			ushort result = 0;
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_ushort ();
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

        object ConvertToString(byte[] msg)
        {
			string result = "";
			ObjectDeserilizeBuffer buffer = null;

			try
			{
				buffer = new ObjectDeserilizeBuffer (msg);
				result = buffer.Parse_string ();
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

		protected Dictionary<string,Func<byte[],object>> deserializeFunctionTable;
	}

	public class ObjectDeserilizeBuffer
	{
		MemoryStream memoryStream = null;
		BinaryReader binaryReader = null;

		public ObjectDeserilizeBuffer(byte[] msg)
		{
			memoryStream = new MemoryStream (msg);
			binaryReader = new BinaryReader (memoryStream);
        }

		public bool Parse_bool()
		{
			return binaryReader.ReadBoolean ();
		}

		public byte Parse_byte()
		{
			return binaryReader.ReadByte ();
		}

		public sbyte Parse_sbyte()
		{
			return binaryReader.ReadSByte ();
		}

		public char Parse_char()
		{
			return binaryReader.ReadChar ();
		}

		public decimal Parse_decimal()
		{
			return binaryReader.ReadDecimal ();
		}

		public double Parse_double()
		{
			return binaryReader.ReadDouble ();
		}

		public float Parse_float()
		{
			return binaryReader.ReadSingle ();
		}

		public int Parse_int()
		{
			return binaryReader.ReadInt32 ();
		}

		public uint Parse_uint()
		{
			return binaryReader.ReadUInt32 ();
		}

		public long Parse_long()
		{
			return binaryReader.ReadInt64 ();
		}

		public short Parse_short()
		{
			return binaryReader.ReadInt16 ();
		}

		public ushort Parse_ushort()
		{
			return binaryReader.ReadUInt16 ();
		}

		public string Parse_string()
		{
			return binaryReader.ReadString ();
		}

		public void Close()
		{
			memoryStream?.Close ();
			binaryReader?.Close ();
		}
	}
}