using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Facebook.MiniJSON;

namespace Transmitter.Tool
{
	public static class BuiltInTypeUtility
	{
		public static class Serialize
		{
			public static byte[] BoolConvertToBuffer(System.Object _msg)
			{
				return BoolConvertToBuffer ((bool)_msg);
			}
			
			public static byte[] BoolConvertToBuffer(bool msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_bool(msg);
					result = buffer.GetBuffer();
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

			public static byte[] ByteConvertToBuffer(System.Object _msg)
			{
				return ByteConvertToBuffer ((byte)_msg);
			}

			public static byte[] ByteConvertToBuffer(byte msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_byte(msg);
					result = buffer.GetBuffer();
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

			public static byte[] SbyteConvertToBuffer(System.Object _msg)
			{
				return SbyteConvertToBuffer ((sbyte)_msg);
			}

			public static byte[] SbyteConvertToBuffer(sbyte msg)
			{

				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_sbyte(msg);
					result = buffer.GetBuffer();
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

			public static byte[] CharConvertToBuffer(System.Object _msg)
			{
				return CharConvertToBuffer ((char)_msg);
			}

			public static byte[] CharConvertToBuffer(char msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_char(msg);
					result = buffer.GetBuffer();
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

			public static byte[] DecimalConvertToBuffer(System.Object _msg)
			{
				return DecimalConvertToBuffer ((decimal)_msg);
			}

			public static byte[] DecimalConvertToBuffer(decimal msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_decimal(msg);
					result = buffer.GetBuffer();
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

			public static byte[] DoubleConvertToBuffer(System.Object _msg)
			{
				return DoubleConvertToBuffer ((double)_msg);
			}

			public static byte[] DoubleConvertToBuffer(double msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_double(msg);
					result = buffer.GetBuffer();
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

			public static byte[] FloatConvertToBuffer(System.Object _msg)
			{
				return FloatConvertToBuffer ((float)_msg);
			}

			public static byte[] FloatConvertToBuffer(float msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_float(msg);
					result = buffer.GetBuffer();
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

			public static byte[] IntConvertToBuffer(System.Object _msg)
			{
				return IntConvertToBuffer ((int)_msg);
			}

			public static byte[] IntConvertToBuffer(int msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_int(msg);
					result = buffer.GetBuffer();
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

			public static byte[] UintConvertToBuffer(System.Object _msg)
			{
				return UintConvertToBuffer ((uint)_msg);
			}

			public static byte[] UintConvertToBuffer(uint msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_uint(msg);
					result = buffer.GetBuffer();
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

			public static byte[] LongConvertToBuffer(System.Object _msg)
			{
				return LongConvertToBuffer ((long)_msg);
			}

			public static byte[] LongConvertToBuffer(long msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_long(msg);
					result = buffer.GetBuffer();
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

			public static byte[] ShortConvertToBuffer(System.Object _msg)
			{
				return ShortConvertToBuffer ((short)_msg);
			}

			public static byte[] ShortConvertToBuffer(short msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_short(msg);
					result = buffer.GetBuffer();
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

			public static byte[] UShortConvertToBuffer(System.Object _msg)
			{
				return UShortConvertToBuffer ((ushort)_msg);
			}

			public static byte[] UShortConvertToBuffer(ushort msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_ushort(msg);
					result = buffer.GetBuffer();
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
				
			public static byte[] StringConvertToBuffer(System.Object _msg)
			{
				return StringConvertToBuffer ((string)_msg);
			}

			public static byte[] StringConvertToBuffer(string msg)
			{
				byte[] result = null;
				ObjectSerilizeBuffer buffer = null;

				try
				{
					buffer = new ObjectSerilizeBuffer();
					buffer.Write_string(msg);
					result = buffer.GetBuffer();
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
		}

		public static class Deserilize
		{
			public static bool BufferConvertToBool(byte[] msg)
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

			public static byte BufferConvertToByte(byte[] msg)
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

			public static sbyte BufferConvertToSByte(byte[] msg)
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

			public static char BufferConvertToChar(byte[] msg)
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

			public static decimal BufferConvertToDecimal(byte[] msg)
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

			public static double BufferConvertToDouble(byte[] msg)
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

			public static float BufferConvertToFloat(byte[] msg)
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

			public static int BufferConvertToInt(byte[] msg)
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

			public static uint BufferConvertToUInt(byte[] msg)
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

			public static long BufferConvertToLong(byte[] msg)
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

			public static short BufferConvertToShort(byte[] msg)
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

			public static ushort BufferConvertToUShort(byte[] msg)
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

			public static string BufferConvertToString(byte[] msg)
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
		}
	}
}
