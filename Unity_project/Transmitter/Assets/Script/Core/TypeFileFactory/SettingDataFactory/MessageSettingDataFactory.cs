using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;
using Transmitter.Tool;
using Transmitter.Serialize;
using Transmitter.TypeSettingDataFactory.Model;

namespace Transmitter.TypeSettingDataFactory
{
	public static class MessageSettingDataFactory {

		public static MessageSettingData Create (string packageDefineText)
		{
			MessageSettingData messageSettingData = new MessageSettingData ();
			messageSettingData.typeSettingDatas = new List<TypeSettingData> ();
			messageSettingData.enumSettingDatas = new List<EnumSettingData> ();

			Dictionary<string,object> classDescriptions = null;
			classDescriptions = (Dictionary<string,object>)Json.Deserialize (packageDefineText);

			if (classDescriptions.Count > 0)
			{
				foreach (var singleClassDescription in classDescriptions) 
				{
					if (!singleClassDescription.Key.ToLower ().StartsWith ("enum.")) 
					{
						TypeSettingData typeSettingData = CreateTypeSettingData (singleClassDescription);
						messageSettingData.typeSettingDatas.Add (typeSettingData);
						messageSettingData.allTypeNames.Add (typeSettingData.typeName);
					}
					else
					{
						EnumSettingData enumSettingData = CreateEnumSettingData (singleClassDescription);
						messageSettingData.enumSettingDatas.Add (enumSettingData);
						messageSettingData.allEnumNames.Add (enumSettingData.enumName);
					}

				}

				try
				{
					List<string> allTypeNames = GetAllTypeNamesAndCheckNotSame (messageSettingData);

					CheckAllFieldUseTypeExist (messageSettingData,allTypeNames);
				}
				catch(Exception e) 
				{
					Debug.Log (e.GetFullMessage ());
				}
			}
			else
			{
				Debug.LogError("尚未輸入完整的資料設定");
			}

			return messageSettingData;
		}

		static List<string> GetAllTypeNamesAndCheckNotSame (MessageSettingData messageSettingData)
		{
			List<string> typeNames = new List<string> (MessageSettingDataUtility.basisTypes);

			messageSettingData.typeSettingDatas.ForEach (typeSettingData=>
				{
					string typeName = typeSettingData.typeName;

					if(!typeNames.Contains(typeName))
					{
						typeNames.Add(typeName);
					}
					else
					{
						throw new UnityException($"出現重覆的Type Name{typeName}");
					}
				});

			messageSettingData.enumSettingDatas.ForEach (enumSettingData=>
				{
					string enumName = enumSettingData.enumName;

					if(!typeNames.Contains(enumName))
					{
						typeNames.Add(enumName);
					}
					else
					{
						throw new UnityException($"出現重覆的Type Name{enumName}");
					}
				});

			return typeNames;
		}

		static void CheckAllFieldUseTypeExist (MessageSettingData messageSettingData,List<string> allTypeNames)
		{
			messageSettingData.typeSettingDatas.ForEach (typeSettingData=>
				{
					typeSettingData.fieldDatas.ForEach(fieldData=>
						{
							if(!allTypeNames.Contains(fieldData.typeName))
							{
								throw new UnityException($"變數引用了不存在的Type Name{fieldData.typeName}");
							}
						});
				});
		}

		static TypeSettingData CreateTypeSettingData(KeyValuePair<string,object> typeNode)
		{
			TypeSettingData typeSettingData = new TypeSettingData ();
			typeSettingData.typeName = typeNode.Key;
			typeSettingData.fieldDatas = new List<FieldSettingData> ();

			Dictionary<string,object> subRoot = (Dictionary<string,object>)typeNode.Value;

			if (subRoot == null) 
			{
				Debug.LogError (typeNode.Key + " 區塊  不是正規的json格式");
			}
			else
			{
				if (subRoot.Count > 0)
				{
					foreach (var fieldSettingRoot in subRoot) 
					{
						try
						{
							FieldSettingData fieldSettingData = CreateFieldSettingData (fieldSettingRoot);
							typeSettingData.fieldDatas.Add (fieldSettingData);
						}
						catch (Exception e)
						{
							Debug.LogException (e);
						}

					}
				}
				else
				{
					Debug.LogError ($"{typeNode.Key} is a temp class");
				}
			}

			return typeSettingData;
		}

		static EnumSettingData CreateEnumSettingData(KeyValuePair<string,object> enumNode)
		{
			string enumName = enumNode.Key.Remove (0, 5);

			List<object> enumItemTable = (List<object>)enumNode.Value;

			List<string> enumItems = new List<string> ();

			enumItemTable.ForEach (item => enumItems.Add (item.ToString ()));

			EnumSettingData enumSettingData = new EnumSettingData ();
			enumSettingData.enumName = enumName;
			enumSettingData.items = new List<string> (enumItems);

			return enumSettingData;
		}

		static FieldSettingData CreateFieldSettingData(KeyValuePair<string,object> jsonFieldNode)
		{
			FieldSettingData fieldSettingData = null;

			// ex : power : "float"
			if (jsonFieldNode.Value is string) 
			{
				fieldSettingData = ProcessFieldNode_KeyValuePair (jsonFieldNode);
			}
			else if(jsonFieldNode.Value is List<object>)
			{
				//ex : "apples":["list","Apple"]
				fieldSettingData = ProcessFieldNode_Indent (jsonFieldNode);
			}
			else
			{
				//ex : 
				/*
			"root":
			{
				"type":"int",
				"iter":"list"
			}
			*/
				fieldSettingData = ProcessFieldNode_Detailed (jsonFieldNode);
			}

			return fieldSettingData;
		}

		static FieldSettingData ProcessFieldNode_KeyValuePair(KeyValuePair<string,object> jsonFieldNode)
		{
			FieldSettingData fieldSettingData = new FieldSettingData ();
			fieldSettingData.fieldName = jsonFieldNode.Key;

			fieldSettingData.typeName = jsonFieldNode.Value.ToString ();
			fieldSettingData.fieldAttribute = FieldAttribute.singal;

			return fieldSettingData;
		}

		static FieldSettingData ProcessFieldNode_Indent(KeyValuePair<string,object> jsonFieldNode)
		{
			List<string> fieldDescripts = new List<string> ();

			foreach (var item in (List<object>)jsonFieldNode.Value) 
			{
				fieldDescripts.Add (item.ToString ());
			}

			Func<string,FieldDescriptionTag> GetFieldDiscriptionAttr = (fieldStr) => {
				if(fieldStr.ToLower()=="list"||fieldStr.ToLower()=="array")
				{
					return FieldDescriptionTag.Iter;
				}
				else
				{
					return FieldDescriptionTag.TypeName;
				}
			};

			Func<string,string> GetFieldDiscriptionValue = (fieldStr) => {
				return fieldStr;
			};

			FieldSettingData fieldSettingData = ProcessFieldNode<string> (jsonFieldNode.Key, fieldDescripts, GetFieldDiscriptionAttr, GetFieldDiscriptionValue);
			return fieldSettingData;
		}

		static FieldSettingData ProcessFieldNode_Detailed(KeyValuePair<string,object> jsonFieldNode)
		{
			Dictionary<string,object> fieldDescriptTable= (Dictionary<string,object>)jsonFieldNode.Value;

			if (fieldDescriptTable == null) 
			{
				throw new Exception (jsonFieldNode.Key + " 區塊  不是正規的Dictionary格式");
			} 
			else
			{
				List<RefKeyValuePair<string,string>> fieldDescripts = new List<RefKeyValuePair<string, string>> ();

				foreach (var item in fieldDescriptTable) 
				{
					fieldDescripts.Add (new RefKeyValuePair<string, string> (item.Key, item.Value.ToString ()));
				}

				Func<RefKeyValuePair<string,string>,FieldDescriptionTag> GetFieldDiscriptionAttr = (fieldPair) => {
					if(fieldPair.key.ToLower() == "iter")
					{
						return FieldDescriptionTag.Iter;
					}
					else if(fieldPair.key.ToLower() == "type")
					{
						return FieldDescriptionTag.TypeName;
					}
					else
					{
						throw new UnityException ("tag不該為 iter,type以外的值");
					}
				};

				Func<RefKeyValuePair<string,string>,string> GetFieldDiscriptionValue = (fieldPair) => {
					return fieldPair.value;
				};

				FieldSettingData fieldSettingData = ProcessFieldNode (jsonFieldNode.Key, fieldDescripts, GetFieldDiscriptionAttr, GetFieldDiscriptionValue);
				return fieldSettingData;
			}
		}

		static FieldSettingData ProcessFieldNode<T> (string fieldNodeName,List<T> fieldDescription, 
			Func<T,FieldDescriptionTag> GetItemAttr, Func<T,string> GetItemValue)
		{
			FieldSettingData fieldSettingData = new FieldSettingData ();
			fieldSettingData.fieldName = fieldNodeName;

			if (fieldDescription.Count > 2) 
			{
				throw new Exception ("變數描述不該長度超過3  " + fieldNodeName);
			}
			else if (fieldDescription.Count==1)
			{
				if (GetItemAttr (fieldDescription [0]) == FieldDescriptionTag.TypeName)
				{
					fieldSettingData.fieldAttribute = FieldAttribute.singal;
				}
				else
				{
					throw new Exception ("未填寫Type欄位" + fieldNodeName + "->" + GetItemValue (fieldDescription [0]));
				}
			}
			else
			{
				Predicate<T> CheckIsListTag = CreateCheckListTagPredicate (GetItemAttr, GetItemValue);

				Predicate<T> CheckIsArrayTag = CreateCheckArrayTagPredicate (GetItemAttr, GetItemValue);

				bool removeListTag = fieldDescription.Remove (CheckIsListTag);

				bool removeArrayTag = fieldDescription.Remove (CheckIsArrayTag);

				if (removeListTag&&removeArrayTag) 
				{
					throw new Exception ("不應該同時填寫 list 又填寫了 array" + fieldNodeName);
				}
				else if (removeListTag) 
				{
					fieldSettingData.fieldAttribute = FieldAttribute.list;
				}
				else if (removeArrayTag)
				{

					fieldSettingData.fieldAttribute = FieldAttribute.array;
				}
				else
				{
					throw new Exception ("不應該填寫兩個 非list 又 非array的變數" + fieldNodeName);
				}

				fieldSettingData.typeName = GetItemValue (fieldDescription [0]);

			}

			return fieldSettingData;
		}

		static Predicate<T> CreateCheckListTagPredicate<T>(Func<T,FieldDescriptionTag> GetItemAttr,Func<T,string> GetItemValue)
		{
			Predicate<T> CheckIsListTag = (item) => 
			{
				if(GetItemAttr(item)== FieldDescriptionTag.Iter&&GetItemValue(item).ToLower () == "list")
				{
					return true;
				}
				else
				{
					return false;
				}
			};

			return CheckIsListTag;
		}

		static Predicate<T> CreateCheckArrayTagPredicate<T>(Func<T,FieldDescriptionTag> GetItemAttr,Func<T,string> GetItemValue)
		{
			Predicate<T> CheckIsListTag = (item) => 
			{
				if(GetItemAttr(item)== FieldDescriptionTag.Iter&&GetItemValue(item).ToLower () == "array")
				{
					return true;
				}
				else
				{
					return false;
				}
			};

			return CheckIsListTag;
		}

		enum FieldDescriptionTag
		{
			TypeName,Iter
		}	
	}
}