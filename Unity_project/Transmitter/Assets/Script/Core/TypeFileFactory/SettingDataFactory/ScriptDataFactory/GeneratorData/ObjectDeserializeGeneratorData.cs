using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class ObjectDeserializeGeneratorData :  GeneratorData{

		List<string> typeNames;
		List<string> enumNames;
		string typeNamespace;

		public ObjectDeserializeGeneratorData (List<string> classNames, List<string> enumNames, string typeNamespace) : base (null)
		{
			this.typeNames = new List<string> (classNames);
			this.enumNames = new List<string> (enumNames);
			this.typeNamespace = typeNamespace;
		}

		protected override void GeneratorContent ()
		{
			string formatClassName = "public class ObjectDeserialize_CustomType : ObjectDeserialize_Base";

			ProcessAddLine (formatClassName);
			ProcessAddLine ("{");
			
			ProcessAddLine ("public override void Init ()", 1);
			ProcessAddLine ("{", 1);
			ProcessAddLine ("base.Init ();", 2);

			typeNames.ForEach (className=>
				{
					string newTableLine = $"deserializeFunctionTable.Add (\"{typeNamespace}.{className}\", ConvertTo{className});";
					ProcessAddLine(newTableLine,2);
				});

			enumNames.ForEach (enumName=>
				{
					string newTableLine = $"deserializeFunctionTable.Add (\"{typeNamespace}.{enumName}\", ConvertTo{enumName});";
					ProcessAddLine(newTableLine,2);
				});

			ProcessAddLine ("}", 1);

			typeNames.ForEach (typeName=>
				{
					AddTempLine ();
					string funcName = $"object ConvertTo{typeName}(byte[] msg)";
					ProcessAddLine(funcName,1);

					ProcessAddLine("{",1);

					string fieldName = char.ToLower(typeName[0])+typeName.Substring(1);

					string ctorLine = $"{typeName} {fieldName} = new {typeName} ();";
					ProcessAddLine(ctorLine,2);

					string initLine = $"{fieldName}.Init (msg);";
					ProcessAddLine(initLine,2);
					string retutnLine = $"return {fieldName};";
					ProcessAddLine(retutnLine,2);

					ProcessAddLine("}",1);
				});

			enumNames.ForEach (enumName=>
				{
					AddTempLine ();
					string funcName = $"object ConvertTo{enumName}(byte[] msg)";
					ProcessAddLine(funcName,1);

					ProcessAddLine("{",1);

					//變數首字小寫
					string fieldName = char.ToLower(enumName[0])+enumName.Substring(1);

					string fieldUShortValue = $"m_{fieldName}_Value";

					string setValueLine = $"ushort {fieldUShortValue} = BitConverter.ToUInt16 (msg, 0);";
					ProcessAddLine(setValueLine,2);

					string returnLine = $"return ({enumName}){fieldUShortValue};";
					ProcessAddLine(returnLine,2);

					ProcessAddLine("}",1);
				});
		}



	}
}