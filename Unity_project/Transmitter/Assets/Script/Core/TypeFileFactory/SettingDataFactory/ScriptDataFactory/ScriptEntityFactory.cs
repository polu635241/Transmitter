using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;
using Transmitter.Tool;
using Transmitter.TypeSettingDataFactory.Model;

namespace Transmitter
{
	public static class ScriptEntityFactory{

		public static void UpdateScript(MessageSettingData messageSettingData)
		{
			List<string> allTypeNames = messageSettingData.allTypeNames;
			List<string> allEnumNames = messageSettingData.allEnumNames;

			MessageGeneratorDataGroup messageGeneratorDataGroup = GetMessageGeneratorDataGroup (messageSettingData, allEnumNames, allEnumNames);

			EntityFileGeneratorDataGroup entityFileGeneratorDataGroup = GetEntityFileGeneratorDatas (messageGeneratorDataGroup, allTypeNames, allEnumNames);

			FlushToEntityFiles (entityFileGeneratorDataGroup);

			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
			#endif
		}

		//MessageGeneratorDataGroup messageGeneratorDataGroup
		static EntityFileGeneratorDataGroup GetEntityFileGeneratorDatas (MessageGeneratorDataGroup messageGeneratorDataGroup, List<string> allTypeNames, List<string> allEnumNames)
		{
			List<GeneratorData> mainClassGeneratorDatas = messageGeneratorDataGroup.mainClassGeneratorDatas;
			List<GeneratorData> functionsGeneratorDatas = messageGeneratorDataGroup.functionsGeneratorDatas;
			
			GeneratorData mainClassScriptData = new NamespaceGeneratorData (usingNamespaces, belongSpace, mainClassGeneratorDatas);

			GeneratorData functionScriptData = new NamespaceGeneratorData (usingNamespaces, belongSpace, functionsGeneratorDatas);

			GeneratorData deserFactoryGeneratorData = new ObjectDeserializeGeneratorData (allTypeNames, allEnumNames, belongSpace);

			GeneratorData deserFactoryScriptData = new NamespaceGeneratorData (usingNamespaces, belongSpace, new List<GeneratorData>{ deserFactoryGeneratorData });

			GeneratorData serFactoryGeneratorData = new ObjectSerializeGeneratorData (allTypeNames, allEnumNames, belongSpace);

			GeneratorData serFactoryScriptData = new NamespaceGeneratorData (usingNamespaces, belongSpace, new List<GeneratorData>{ serFactoryGeneratorData });

			EntityFileGeneratorDataGroup entityFileGeneratorDataGroup = new EntityFileGeneratorDataGroup () 
			{
				mainClassScriptData = mainClassScriptData,
				functionScriptData = functionScriptData,
				deserFactoryScriptData = deserFactoryScriptData,
				serFactoryScriptData = serFactoryScriptData
			};

			return entityFileGeneratorDataGroup;
		}

		static MessageGeneratorDataGroup GetMessageGeneratorDataGroup (MessageSettingData messageSettingData, List<string> allTypeNames, List<string> allEnumNames)
		{
			MessageGeneratorDataGroup messageGeneratorDataGroup = new MessageGeneratorDataGroup ();
			
			messageSettingData.typeSettingDatas.ForEach (typeSettingData=>
				{
					GeneratorData mainClassGeneratorData = GetFieldParticalClassGeneratorData(typeSettingData);

					GeneratorData functionDesriptionData = GetFunctionParticalClassGeneratorData(typeSettingData,allTypeNames,allEnumNames);

					messageGeneratorDataGroup.mainClassGeneratorDatas.Add(mainClassGeneratorData);

					messageGeneratorDataGroup.functionsGeneratorDatas.Add(functionDesriptionData);
				});

			messageSettingData.enumSettingDatas.ForEach (enumSettingData=>
				{
					EnumGeneratorData enumGeneratorData = new EnumGeneratorData(enumSettingData.enumName,enumSettingData.items);
					messageGeneratorDataGroup.mainClassGeneratorDatas.Add(enumGeneratorData);
				});

			return messageGeneratorDataGroup;
		}

		class MessageGeneratorDataGroup
		{
			/// <summary>
			/// 記載變數的partical class
			/// </summary>
			public List<GeneratorData> mainClassGeneratorDatas = new List<GeneratorData> ();

			/// <summary>
			/// 實作序列化以及反序列化的 partical class
			/// </summary>
			public List<GeneratorData> functionsGeneratorDatas = new List<GeneratorData> ();
		}

		class EntityFileGeneratorDataGroup
		{
			public GeneratorData mainClassScriptData;
			public GeneratorData functionScriptData;
			public GeneratorData deserFactoryScriptData;
			public GeneratorData serFactoryScriptData;
		}

		static void FlushToEntityFiles(EntityFileGeneratorDataGroup entityFileGeneratorDataGroup)
		{
			using (StreamWriter sw = new StreamWriter (ProcessMainClassScriptPath, false))
			{
				List<string> mainClassScriptLines = entityFileGeneratorDataGroup.mainClassScriptData.GetGeneratorLines ();
				mainClassScriptLines.ForEach (scriptLine => sw.WriteLine (scriptLine));
			}

			using (StreamWriter sw = new StreamWriter (ProcessFunctionScriptPath, false))
			{
				List<string> functionClassScriptLines = entityFileGeneratorDataGroup.functionScriptData.GetGeneratorLines ();
				functionClassScriptLines.ForEach (scriptLine => sw.WriteLine (scriptLine));
			}

			using (StreamWriter sw = new StreamWriter (ObjectDeserializeScriptPath, false))
			{
				List<string> deserFactoryScriptLines = entityFileGeneratorDataGroup.deserFactoryScriptData.GetGeneratorLines ();
				deserFactoryScriptLines.ForEach (scriptLine => sw.WriteLine (scriptLine));
			}

			using (StreamWriter sw = new StreamWriter (ObjectSerializeScriptPath, false))
			{
				List<string> serFactoryScriptLines = entityFileGeneratorDataGroup.serFactoryScriptData.GetGeneratorLines ();
				serFactoryScriptLines.ForEach (scriptLine => sw.WriteLine (scriptLine));
			}
		}

		static GeneratorData GetFieldParticalClassGeneratorData(TypeSettingData typeSettingData)
		{
			List<GeneratorData> fieldsGeneratorDatas = 
				new List<GeneratorData> ();

			typeSettingData.fieldDatas.ForEach(fieldData=>
				{
					fieldsGeneratorDatas.Add(new FieldGeneratorData(fieldData));
				});

			GeneratorClassSetting generatorClassSetting = new GeneratorClassSetting()
			{
				isPartial = true,
				className = typeSettingData.typeName,
				InheritName = "Message",
				drawSerializableAttribute = true
			};

			ClassGeneratorData mainClassGeneratorData = 
				new ClassGeneratorData(generatorClassSetting,fieldsGeneratorDatas);

			return mainClassGeneratorData;
		}

		static GeneratorData GetFunctionParticalClassGeneratorData (TypeSettingData typeSettingData, List<string> allTypeNames, List<string> allEnumNames)
		{
			GeneratorClassSetting generatorClassSetting = new GeneratorClassSetting()
			{
				isPartial = true,
				className = typeSettingData.typeName,
				InheritName = "Message",
				drawSerializableAttribute = false
			};

			List<GeneratorData> interfaceFunctionGeneratorDatas = new List<GeneratorData>();

			GetByteArrayFunctionGeneratorData serializeGeneratorData = new GetByteArrayFunctionGeneratorData(typeSettingData,allEnumNames);

			interfaceFunctionGeneratorDatas.Add(serializeGeneratorData);

			InitFunctionGeneratorData deserializeGeneratorData = new InitFunctionGeneratorData(typeSettingData,allEnumNames);

			interfaceFunctionGeneratorDatas.Add(deserializeGeneratorData);

			ClassGeneratorData functionDesriptionData = 
				new ClassGeneratorData (generatorClassSetting, interfaceFunctionGeneratorDatas);

			return functionDesriptionData;
		}

		#region file setting

		static List<string> usingNamespaces = new List<string> { 
			"System",
			"System.IO",
			"System.Collections.Generic",
			"Transmitter.Serialize.Tool"
		};

		const string belongSpace = "Transmitter.Serialize";

		#endregion

		#region IO Path

		static string scriptPathFolder = @"Script\Core\Message";

		static string ProcessMainClassScriptPath
		{
			get
			{
				string mainClassScriptPath = Path.Combine (scriptPathFolder, "PacketTypeTable.cs");

				return Path.Combine (Application.dataPath,mainClassScriptPath);
			}
		}

		static string ProcessFunctionScriptPath
		{
			get
			{
				string functionScriptPath = Path.Combine (scriptPathFolder, "PacketFunction.cs");

				return Path.Combine (Application.dataPath,functionScriptPath);
			}
		}

		static string ObjectDeserializeScriptPath
		{
			get
			{
				string objectDeserializeScriptPath = Path.Combine (scriptPathFolder, "ObjectDeserialize_CustomType.cs");

				return Path.Combine (Application.dataPath, objectDeserializeScriptPath);
			}
		}

		static string ObjectSerializeScriptPath
		{
			get
			{
				string objectSerializeScriptPath = Path.Combine (scriptPathFolder, "ObjectSerialize_CustomType.cs");

				return Path.Combine (Application.dataPath, objectSerializeScriptPath);
			}
		}

		#endregion
	}
}