using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class ObjectSerializeGeneratorData:GeneratorData
	{
		List<string> typeNames;
		List<string> enumNames;
		string typeNamespace;

		public ObjectSerializeGeneratorData (List<string> typeNames, List<string> enumNames, string typeNamespace) : base (null)
		{
			this.typeNames = new List<string> (typeNames);
			this.enumNames = new List<string> (enumNames);
			this.typeNamespace = typeNamespace;
		}

		protected override void GeneratorContent ()
		{
			string formatClassName = "public class ObjectSerialize_CustomType : ObjectSerialize_Base";

			ProcessAddLine (formatClassName);
			ProcessAddLine ("{");

			ProcessAddLine ("public override void Init ()", 1);
			ProcessAddLine ("{",1);
			ProcessAddLine ("base.Init ();", 2);

			this.typeNames.ForEach (typeName=>
				{
					string formatContent = $"serializeFunctionTable.Add (\"{typeNamespace}.{typeName}\", {typeName}ConvertToBuffer);";
					ProcessAddLine (formatContent, 2);
				});

			this.enumNames.ForEach (enumName => 
				{
					string formatContent = $"serializeFunctionTable.Add (\"{typeNamespace}.{enumName}\", {enumName}ConvertToBuffer);";
					ProcessAddLine (formatContent, 2);
				});

			ProcessAddLine ("}",1);

			this.typeNames.ForEach (typeName=>
				{
					AddTempLine ();
					string formatFuncName = $"byte[] {typeName}ConvertToBuffer(Object msg)";
					ProcessAddLine (formatFuncName, 1);
					ProcessAddLine ("{", 1);
					//駝峰命名法 首字小寫轉成變數
					string fieldName = char.ToLower(typeName[0])+typeName.Substring(1);
					ProcessAddLine($"{typeName} {fieldName} = ({typeName})msg;", 2);
					ProcessAddLine($"return {fieldName}.GetByteArray ();", 2);
					ProcessAddLine ("}", 1);
				});

			this.enumNames.ForEach (enumName=>
				{
					AddTempLine ();
					string formatFuncName = $"byte[] {enumName}ConvertToBuffer(Object msg)";
					ProcessAddLine (formatFuncName, 1);
					ProcessAddLine ("{", 1);
					//駝峰命名法 首字小寫轉成變數
					string fieldName = char.ToLower(enumName[0])+enumName.Substring(1);
					ProcessAddLine($"{enumName} {fieldName} = ({enumName})msg;", 2);
					string fieldUShortValue = $"m_{fieldName}_Value";
					ProcessAddLine($"ushort {fieldUShortValue} = (ushort){fieldName};", 2);
					ProcessAddLine($"return BitConverter.GetBytes ({fieldUShortValue});", 2);
					ProcessAddLine ("}", 1);
				});
		}


		/*
			byte[] Test1ConvertToBuffer(Object msg)
	        {
				Test1 test1 = (Test1)msg;
				return test1.GetByteArray ();
	        }
		*/
	}
}