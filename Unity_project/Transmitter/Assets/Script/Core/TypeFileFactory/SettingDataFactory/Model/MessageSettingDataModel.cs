using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	[Serializable]
	public class MessageSettingData
	{
		public List<TypeSettingData> typeSettingDatas = new List<TypeSettingData>();

		public List<EnumSettingData> enumSettingDatas = new List<EnumSettingData>();

		public List<string> allTypeNames = new List<string> ();

		public List<string> allEnumNames = new List<string> ();

		public bool ExistSameTypeName()
		{
			List<string> cacheTypeNames = new List<string> ();

			for (int i = 0; i < typeSettingDatas.Count; i++) 
			{
				string _typeName = typeSettingDatas [i].typeName;

				if (!cacheTypeNames.Contains (_typeName))
				{
					Debug.LogError ($"has same type name {typeSettingDatas [i]}");
					return true;
				}
				else
				{
					cacheTypeNames.Add (_typeName);
				}
			}

			return false;
		}

		public bool ExistType (string typeName)
		{
			bool exist = false;

			exist = typeSettingDatas.Exists (data => data.typeName == typeName);

			// type裡找不到 去enum找
			if (!exist) 
			{
				exist = enumSettingDatas.Exists (data => data.enumName == typeName);

				return exist;
			}
			else
			{
				return exist;
			}

		}
	}
	
	[Serializable]
	public class EnumSettingData
	{
		public string enumName;
		public List<string> items;
	}

	[Serializable]
	public class TypeSettingData
	{
		public string typeName;

		public List<FieldSettingData> fieldDatas;
	}

	[Serializable]
	public class FieldSettingData
	{
		public string fieldName;

		public string typeName;

		public FieldAttribute fieldAttribute;
	}

	public enum FieldAttribute:int
	{
		singal=0,
		array = 1,
		list =2
	}
}
