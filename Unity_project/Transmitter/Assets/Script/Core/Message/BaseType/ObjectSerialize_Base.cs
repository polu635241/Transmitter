using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Tool;

namespace Transmitter.Serialize
{
	public class ObjectSerialize_Base 
	{
		public virtual void Init()
		{
			serializeFunctionTable = new Dictionary<string, Func<object, byte[]>> ();
			serializeFunctionTable.Add ("System.Boolean", BuiltInTypeUtility.Serialize.BoolConvertToBuffer);
			serializeFunctionTable.Add ("System.Byte", BuiltInTypeUtility.Serialize.ByteConvertToBuffer);
			serializeFunctionTable.Add ("System.SByte", BuiltInTypeUtility.Serialize.SbyteConvertToBuffer);
			serializeFunctionTable.Add ("System.Char", BuiltInTypeUtility.Serialize.CharConvertToBuffer);
			serializeFunctionTable.Add ("System.Decimal", BuiltInTypeUtility.Serialize.DecimalConvertToBuffer);
			serializeFunctionTable.Add ("System.Double", BuiltInTypeUtility.Serialize.DoubleConvertToBuffer);
			serializeFunctionTable.Add ("System.Single", BuiltInTypeUtility.Serialize.FloatConvertToBuffer);
			serializeFunctionTable.Add ("System.Int32", BuiltInTypeUtility.Serialize.IntConvertToBuffer);
			serializeFunctionTable.Add ("System.UInt32", BuiltInTypeUtility.Serialize.UintConvertToBuffer);
			serializeFunctionTable.Add ("System.Int64", BuiltInTypeUtility.Serialize.LongConvertToBuffer);
			serializeFunctionTable.Add ("System.Int16", BuiltInTypeUtility.Serialize.ShortConvertToBuffer);
			serializeFunctionTable.Add ("System.UInt16", BuiltInTypeUtility.Serialize.UShortConvertToBuffer);
			serializeFunctionTable.Add ("System.String", BuiltInTypeUtility.Serialize.StringConvertToBuffer);
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

		protected Dictionary<string,Func<object,byte[]>> serializeFunctionTable;
	}
}