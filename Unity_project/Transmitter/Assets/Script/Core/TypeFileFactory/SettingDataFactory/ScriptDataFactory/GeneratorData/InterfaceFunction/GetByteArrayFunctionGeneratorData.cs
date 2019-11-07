using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class GetByteArrayFunctionGeneratorData:GeneratorData
	{
		TypeSettingData typeSettingData;
		List<string> allEnumNames;

		public GetByteArrayFunctionGeneratorData (TypeSettingData typeSettingData,List<string> allEnumNames) : base (null)
		{
			this.typeSettingData = typeSettingData;
			this.allEnumNames = new List<string> (allEnumNames);

			InitWriteBuildinTypeTable ();
		}

		protected override void GeneratorContent ()
		{
			ProcessAddLine ("public override byte[] GetByteArray()");
			ProcessAddLine ("{");

			//初始化緩存
			ProcessAddLine (@"byte[] buffer = new byte[1024];" , 1);
			ProcessAddLine ("MemoryStream memoryStream = new MemoryStream(buffer);", 1);
			ProcessAddLine ("BinaryWriter binaryWriter = new BinaryWriter(memoryStream);", 1);

			AddTempLine ();

			for (int i = 0; i < typeSettingData.fieldDatas.Count; i++) 
			{
				FieldSettingData fieldData = typeSettingData.fieldDatas [i];
				ProcessFieldData (fieldData);

				if (i <= typeSettingData.fieldDatas.Count - 1) 
				{
					AddTempLine ();
				}
			}

			ProcessAddLine ("byte[] result = memoryStream.ToArray ();", 1);
			AddTempLine ();
			ProcessAddLine ("binaryWriter.Close ();", 1);
			ProcessAddLine ("memoryStream.Close ();", 1);
			AddTempLine ();
			ProcessAddLine ("return result;", 1);
		}

		void ProcessFieldData(FieldSettingData fieldSettingData)
		{
			string fieldName = fieldSettingData.fieldName;
			string typeName = fieldSettingData.typeName;
			
			//repeated的話 先寫入長度 然後依序寫入單體
			if (fieldSettingData.fieldAttribute == FieldAttribute.array || fieldSettingData.fieldAttribute == FieldAttribute.list) 
			{
				string nameAndLengthFiled = "";

				if (fieldSettingData.fieldAttribute == FieldAttribute.array) 
				{
					nameAndLengthFiled = fieldName + ".Length";

				}
				else
				{
					nameAndLengthFiled = fieldSettingData.fieldName + ".Count";
				}
					
				ProcessAddLine (string.Format ("if ({0} != null)", fieldSettingData.fieldName), 1);
				ProcessAddLine ("{", 1);
				string writeLengthLine = string.Format ("binaryWriter.Write((short){0});", nameAndLengthFiled);
				ProcessAddLine (writeLengthLine,2);
				AddTempLine ();

				ProcessAddLine (string.Format ("for (int i = 0; i < {0}; i++)", nameAndLengthFiled), 2);
				ProcessAddLine ("{", 2);
				ProcessAddLines (ProcessReapeatedFieldData (fieldSettingData), 3);
				ProcessAddLine ("}", 2);
				ProcessAddLine ("}", 1);
				ProcessAddLine ("else", 1);
				ProcessAddLine ("{", 1);
				ProcessAddLine ("binaryWriter.Write ((short)-1);", 2);
				ProcessAddLine ("}", 1);

			}
			else
			{
				ProcessAddLines (ProcessSingleFieldData (fieldSettingData), 1);
			}
		}

		List<string> ProcessSingleFieldData(FieldSettingData fieldSettingData)
		{
			List<string> lines = new List<string> ();

			string typeName = fieldSettingData.typeName;

			string fieldName = fieldSettingData.fieldName;

			lines = GetObjectToByteArray (typeName, fieldName);

			return lines;
		}

		List<string> ProcessReapeatedFieldData(FieldSettingData fieldSettingData)
		{
			List<string> lines = new List<string> ();

			string typeName = fieldSettingData.typeName;

			//因為放在for迴圈裡 譬如 變數名稱叫names => names[i]
			string fieldName = string.Format("m_{0}Item",fieldSettingData.fieldName);

			string announceItemFieldLine = 
				string.Format ("{0} {1} = {2}[i];", typeName, fieldName, fieldSettingData.fieldName);

			lines.Add (announceItemFieldLine);
			lines.AddRange (GetObjectToByteArray (typeName, fieldName));

			return lines;
		}

		List<string> GetObjectToByteArray (string typeName, string fieldName)
		{
			List<string> lines = new List<string> ();

			Func<string,string> WriteBuildinTypeFunc = null;

			if (writeBuildinTypeTable.TryGetValue (typeName, out WriteBuildinTypeFunc)) 
			{
				lines.Add (WriteBuildinTypeFunc.Invoke (fieldName));
			}
			else if (allEnumNames.Contains (typeName)) 
			{
				lines.AddRange (GetInnerEnum (fieldName));
			}
			else
			{
				lines.AddRange (GetInnerClass (fieldName));
			}

			//TODO 拆解物件
			return lines;
		}

		Dictionary<string,Func<string,string>> writeBuildinTypeTable;

		void InitWriteBuildinTypeTable()
		{
			if (writeBuildinTypeTable == null) 
			{
				writeBuildinTypeTable = new Dictionary<string, Func<string, string>> ();

				writeBuildinTypeTable.Add ("bool", GetWriteBool);
				writeBuildinTypeTable.Add ("byte", GetWriteByte);
				writeBuildinTypeTable.Add ("sbyte", GetWriteSbyte);
				writeBuildinTypeTable.Add ("char", GetWriteChar);
				writeBuildinTypeTable.Add ("decimal", GetWriteDecimal);
				writeBuildinTypeTable.Add ("double", GetWriteDouble);
				writeBuildinTypeTable.Add ("float", GetWriteFloat);
				writeBuildinTypeTable.Add ("int", GetWriteInt);
				writeBuildinTypeTable.Add ("uint", GetWriteUint);
				writeBuildinTypeTable.Add ("long", GetWriteLong);
				writeBuildinTypeTable.Add ("short", GetWriteShort);
				writeBuildinTypeTable.Add ("ushort", GetWriteUshort);
				writeBuildinTypeTable.Add ("string", GetWriteString);
			}

		}

		#region GetWriteBuildinType

		string GetWriteBool(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteByte(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteSbyte(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteChar(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteDecimal(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteDouble(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteFloat(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteInt(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteUint(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteLong(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteShort(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteUshort(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}

		string GetWriteString(string fieldName)
		{
			return string.Format ("binaryWriter.Write ({0});", fieldName);
		}
		#endregion

		List<string> GetInnerEnum(string fieldName)
		{
			List<string> lines = new List<string> ();

			string fieldValueName = fieldName + "_value";

			string getFieldValueLine = string.Format ("ushort {0} = (ushort){1};", fieldValueName, fieldName);

			lines.Add (getFieldValueLine);

			string writeContentLine = string.Format ("binaryWriter.Write({0});", fieldValueName);

			lines.Add (writeContentLine);

			return lines;
		}

		List<string> GetInnerClass(string fieldName)
		{
			List<string> lines = new List<string> ();

			string bufferName = fieldName + "Buffer";

			//SO內部的型別一律繼承 IMessage<T>
			string getByteArrayLine = string.Format ("byte[] {0} = {1}.GetByteArray();",bufferName,fieldName);

			lines.Add (getByteArrayLine);

			string writeCountLine = string.Format ("binaryWriter.Write((ushort){0}.Length);", bufferName);

			lines.Add (writeCountLine);

			string writeContentLine = string.Format ("binaryWriter.Write({0});", bufferName);

			lines.Add (writeContentLine);

			return lines;
		}
	}
}