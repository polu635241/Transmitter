using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class InitFunctionGeneratorData:GeneratorData
	{
		TypeSettingData typeSettingData;
		List<string> allEnumNames;

		public InitFunctionGeneratorData (TypeSettingData typeSettingData,List<string> allEnumNames) : base (null)
		{
			this.typeSettingData = typeSettingData;
			this.allEnumNames = new List<string> (allEnumNames);

			InitReadBuildinTypeTable ();
		}

		protected override void GeneratorContent ()
		{
			ProcessAddLine ("public override void Init (byte[] msg)");
			ProcessAddLine ("{");
			ProcessAddLine ("if (hasInit)", 1);
			ProcessAddLine ("return;", 2);
			AddTempLine ();
			ProcessAddLine ("hasInit = true;", 1);
			ProcessAddLine ("MemoryStream memoryStream = new MemoryStream(msg);", 1);
			ProcessAddLine ("BinaryReader binaryReader = new BinaryReader (memoryStream);", 1);

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

			ProcessAddLine ("binaryReader.Close ();", 1);
			ProcessAddLine ("memoryStream.Close ();", 1);
		}

		void ProcessFieldData(FieldSettingData fieldSettingData)
		{
			string fieldName = fieldSettingData.fieldName;
			string typeName = fieldSettingData.typeName;

			//repeated的話 先寫入長度 然後依序寫入單體
			if (fieldSettingData.fieldAttribute == FieldAttribute.array || fieldSettingData.fieldAttribute == FieldAttribute.list) 
			{
				string nameAndLengthFiled = $"{fieldSettingData.fieldName}ReapeatedCount";

				int count;

				string readLengthLine = $"int {nameAndLengthFiled} = binaryReader.ReadInt16();";
				ProcessAddLine (readLengthLine,1);

				//如果list或是array是null 封包會寫入-1 與0做區別
				ProcessAddLine ($"if ({nameAndLengthFiled} != -1)", 1);
				ProcessAddLine ("{", 1);
				AddTempLine ();

				string repeatedCtor = "";

				if (fieldSettingData.fieldAttribute == FieldAttribute.array) 
				{
					repeatedCtor = $"{fieldName} = new {typeName}[{nameAndLengthFiled}];";
				}
				else
				{
					repeatedCtor = $"{fieldName} = new List<{typeName}>();";
				}

				ProcessAddLine (repeatedCtor, 2);

				ProcessAddLine (string.Format ("for (int i = 0; i < {0}ReapeatedCount; i++)", fieldSettingData.fieldName), 2);
				ProcessAddLine ("{", 2);
				ProcessAddLines (ProcessReapeatedFieldData (fieldSettingData), 3);
				ProcessAddLine ("}", 2);
				ProcessAddLine ("}", 1);
				ProcessAddLine ("else", 1);
				ProcessAddLine ("{", 1);
				ProcessAddLine (string.Format ("{0} = null;", fieldSettingData.fieldName), 2);
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

			lines = GetByteArrayToObject (typeName, fieldName);

			return lines;
		}

		/// <summary>
		/// for裡 一般都是用 i 指代index
		/// </summary>
		/// <returns>The reapeated field data.</returns>
		/// <param name="fieldSettingData">Field setting data.</param>
		/// <param name="indexSymbo">Index symbo.</param>
		/// <param name="factory">Factory.</param>
		List<string> ProcessReapeatedFieldData(FieldSettingData fieldSettingData)
		{
			List<string> lines = new List<string> ();

			string typeName = fieldSettingData.typeName;

			string fieldName = fieldSettingData.fieldName ;

			string itemFieldName = $"{fieldName}_item";

			//宣告 ex int IDs_item;
			string itemAnnounce = $"{typeName} {itemFieldName};";

			lines.Add (itemAnnounce);

			List<string> readObjectLines = GetByteArrayToObject (typeName, itemFieldName);
			lines.AddRange (readObjectLines);

			if (fieldSettingData.fieldAttribute == FieldAttribute.array) 
			{
				//因為放在for迴圈裡 譬如 變數名稱叫names => names[i]
				string arrayProcessField = $"{fieldName}[i]";

				lines.Add ($"{arrayProcessField} = {itemFieldName};");
			}
			else
			{
				//list用Add
				lines.Add ($"{fieldName}.Add({itemFieldName});");
			}

			return lines;
		}

		List<string> GetByteArrayToObject(string typeName,string fieldName)
		{
			List<string> lines = new List<string> ();

			Func<string> ReadBuildinTypeFunc = null;

			if (readBuildinTypeTable.TryGetValue (typeName, out ReadBuildinTypeFunc)) 
			{
				string readBufferLine = ReadBuildinTypeFunc.Invoke ();
				lines.Add ($"{fieldName} = {readBufferLine}");
			}
			else if (allEnumNames.Contains (typeName)) 
			{
				lines.AddRange (GetReadInnerEnum (typeName, fieldName));
			}
			else
			{
				lines.AddRange (GetReadInnerClass (typeName, fieldName));
			}

			return lines;
		}

		void InitReadBuildinTypeTable()
		{
			if (readBuildinTypeTable == null) 
			{
				readBuildinTypeTable = new Dictionary<string, Func<string>> ();
				readBuildinTypeTable.Add ("bool", GetReadBool);
				readBuildinTypeTable.Add ("byte", GetReadByte);
				readBuildinTypeTable.Add ("sbyte", GetReadSbyte);
				readBuildinTypeTable.Add ("char", GetReadChar);
				readBuildinTypeTable.Add ("decimal", GetReadDecimal);
				readBuildinTypeTable.Add ("double", GetReadDouble);
				readBuildinTypeTable.Add ("float", GetReadFloat);
				readBuildinTypeTable.Add ("int", GetReadInt);
				readBuildinTypeTable.Add ("uint", GetReadUint);
				readBuildinTypeTable.Add ("long", GetReadLong);
				readBuildinTypeTable.Add ("short", GetReadShort);
				readBuildinTypeTable.Add ("ushort", GetReadUshort);
				readBuildinTypeTable.Add ("string", GetReadString);
			}
			
		}

		Dictionary<string,Func<string>> readBuildinTypeTable;

		#region GetWriteBuildinType

		string GetReadBool()
		{
			return "binaryWriter.ReadBoolean ();";
		}

		string GetReadByte()
		{
			return "binaryWriter.ReadByte ();";
		}

		string GetReadSbyte()
		{
			return "binaryWriter.ReadSByte ();";
		}

		string GetReadChar()
		{
			return "binaryReader.ReadChar ();";
		}

		string GetReadDecimal()
		{
			return "binaryReader.ReadDecimal ();";
		}

		string GetReadDouble()
		{
			return "binaryReader.ReadDouble ();";
		}

		string GetReadFloat()
		{
			return "binaryReader.ReadSingle ();";
		}

		string GetReadInt()
		{
			return "binaryReader.ReadInt32 ();";
		}

		string GetReadUint()
		{
			return "binaryReader.ReadUInt32 ();";
		}

		string GetReadLong()
		{
			return "binaryReader.ReadInt64 ();";
		}

		string GetReadShort()
		{
			return "binaryReader.ReadInt16 ();";
		}

		string GetReadUshort()
		{
			return "binaryReader.ReadUInt16 ();";
		}

		string GetReadString()
		{
			return "binaryReader.ReadString ();";
		}
		#endregion

		List<string> GetReadInnerEnum(string typeName,string fieldName)
		{
			List<string> lines = new List<string> ();

			string fieldValueName = fieldName + "_value";

			//從記憶體中讀取Ushort
			string getFieldValueLine = string.Format ("ushort {0} = {1}", fieldValueName, GetReadUshort ());

			//把Ushort轉型成變數設定的所屬資料型態
			string setFieldValueLine = string.Format ("{0} = ({1}){2};", fieldName, typeName, fieldValueName);

			lines.Add (getFieldValueLine);

			lines.Add (setFieldValueLine);

			return lines;
		}

		List<string> GetReadInnerClass(string typeName,string fieldName)
		{
			List<string> lines = new List<string> ();

			//SO內部的型別一律繼承 IMessage
			string buffetCountLine = 
				string.Format ("ushort {0}BufferCount = binaryReader.ReadUInt16();",
					fieldName);

			lines.Add (buffetCountLine);

			string buffetLine = 
				string.Format ("byte[] {0}Buffer = binaryReader.ReadBytes({0}BufferCount);", 
					fieldName);

			lines.Add (buffetLine);

			string writeContentLine = string.Format ("{0} = new {1} ();"
				, fieldName, typeName);

			lines.Add (writeContentLine);

			lines.Add (string.Format ("{0}.Init ({0}Buffer);", fieldName));

			return lines;
		}
	}
}