using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
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
