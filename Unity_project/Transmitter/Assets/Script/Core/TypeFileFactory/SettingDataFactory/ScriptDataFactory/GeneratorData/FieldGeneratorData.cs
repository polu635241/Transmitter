using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class FieldGeneratorData:GeneratorData
	{
		FieldSettingData fieldSettingData;

		public FieldGeneratorData(FieldSettingData fieldSettingData):base(null)
		{
			this.fieldSettingData = fieldSettingData;
		}

		protected override void GeneratorContent ()
		{
			string origanFieldType = fieldSettingData.typeName;
			string processFieldType = "";

			switch(fieldSettingData.fieldAttribute)
			{
			case FieldAttribute.singal:
				{
					processFieldType = origanFieldType;
					break;
				}

			case FieldAttribute.array:
				{
					processFieldType = origanFieldType+"[]";
					break;
				}

			case FieldAttribute.list:
				{
					processFieldType = string.Format("List<{0}>",origanFieldType);
					break;
				}
			}

			string formatFiledLine = string.Format ("public {0} {1};", processFieldType, fieldSettingData.fieldName);

			ProcessAddLine (formatFiledLine);
		}
			
		protected override bool ExitHook 
		{
			get 
			{
				return false;
			}
		}
	}
}