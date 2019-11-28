using System;
using System.IO;
using System.Collections.Generic;

namespace Transmitter.Serialize
{
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
