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

			Func<byte[], object> ConvertToBoolProcess = (Buffer) => {
				return (Object)BuiltInTypeUtility.Deserilize.BufferConvertToBool(Buffer);
			};

			deserializeFunctionTable.Add ("System.Boolean", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToBool(msg);
				});
			
			deserializeFunctionTable.Add ("System.Byte", (msg)=>
				{ 
					return BuiltInTypeUtility.Deserilize.BufferConvertToByte(msg);
				});
			
			deserializeFunctionTable.Add ("System.SByte", (msg)=>
				{ 
					return BuiltInTypeUtility.Deserilize.BufferConvertToSByte(msg);
				});

			deserializeFunctionTable.Add ("System.Char", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToChar(msg);
				});
			
			deserializeFunctionTable.Add ("System.Decimal", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToDecimal(msg);
				});
			
			deserializeFunctionTable.Add ("System.Double", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToDouble(msg);
				});
			
			deserializeFunctionTable.Add ("System.Single", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToFloat(msg);
				});
			
			deserializeFunctionTable.Add ("System.Int32", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToInt(msg);
				});
			
			deserializeFunctionTable.Add ("System.UInt32", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToUInt(msg);
				});
			
			deserializeFunctionTable.Add ("System.Int64", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToLong(msg);
				});
			
			deserializeFunctionTable.Add ("System.Int16", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToShort(msg);
				});
			
			deserializeFunctionTable.Add ("System.UInt16", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToUShort(msg);
				});
			
			deserializeFunctionTable.Add ("System.String", (msg)=>
				{
					return BuiltInTypeUtility.Deserilize.BufferConvertToString(msg);
				});
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

		protected Dictionary<string,Func<byte[],object>> deserializeFunctionTable;
	}
}