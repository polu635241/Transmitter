using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	[System.Serializable]
	public class GeneratorClassSetting
	{
		public bool isPartial = false;

		public bool drawSerializableAttribute;

		public string className;

		bool hasInherit = false;
		public bool HasInherit
		{
			get
			{
				return hasInherit;
			}
		}

		string inheritName;
		public string InheritName
		{
			get
			{
				return inheritName;
			}

			set
			{
				inheritName = value;

				hasInherit = true;
			}
		}

	}	
}