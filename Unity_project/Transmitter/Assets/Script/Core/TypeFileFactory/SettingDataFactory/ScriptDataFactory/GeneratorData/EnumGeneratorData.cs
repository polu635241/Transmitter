using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class EnumGeneratorData:GeneratorData
	{
		string enumTypeName;
		List<string> enumFieldNames;

		public EnumGeneratorData (string enumTypeName, List<string> enumFieldNames) : base (null)
		{
			this.enumTypeName = enumTypeName;
			this.enumFieldNames = new List<string> (enumFieldNames);
		}

		protected override void GeneratorContent ()
		{
			string drawFormatEnumName = string.Format ("public enum {0}", enumTypeName);

			ProcessAddLine (drawFormatEnumName);
			ProcessAddLine ("{");

			for (int i = 0; i < enumFieldNames.Count; i++) 
			{
				string drawFieldItem = enumFieldNames [i];
				
				if (i < enumFieldNames.Count - 1)
				{
					drawFieldItem += fieldSpiltSymbol;
				}

				ProcessAddLine (drawFieldItem, 1);
			}
		}

		const string fieldSpiltSymbol = ",";

		protected override bool ExitHook 
		{
			get 
			{
				return true;
			}
		}
	}
}