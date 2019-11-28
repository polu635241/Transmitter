using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Tool;

namespace Transmitter.Serialize
{
	public class ObjectSerilizeBuffer
	{
		MemoryStream memoryStream = null;
		BinaryWriter binaryWriter = null;

		public byte[] GetBuffer()
		{
			return memoryStream.GetBuffer ();
		}

		public ObjectSerilizeBuffer()
		{
			memoryStream = new MemoryStream ();
			binaryWriter = new BinaryWriter (memoryStream);
		}

		public void Write_bool(bool msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_byte(byte msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_sbyte(sbyte msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_char(char msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_decimal(decimal msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_double(double msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_float(float msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_int(int msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_uint(uint msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_long(long msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_short(short msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_ushort(ushort msg)
		{
			binaryWriter.Write (msg);
		}

		public void Write_string(string msg)
		{
			binaryWriter.Write (msg);
		}

		public void Close()
		{
			memoryStream?.Close ();
			binaryWriter?.Close ();
		}
	}
}
