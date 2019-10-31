using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class ClassGeneratorData:GeneratorData
	{
		GeneratorClassSetting generatorClassSetting;

		public ClassGeneratorData (GeneratorClassSetting drawClassSetting, List<GeneratorData> nodes) : base (nodes)
		{
			this.generatorClassSetting = drawClassSetting;
		}

		protected override void GeneratorContent ()
		{
//			string formatClassName = string.Format ("public partial class {0} :Message", typeName);

			string drawPartial = generatorClassSetting.isPartial ? " partial " : " ";
			string drawClass = generatorClassSetting.className;
			string drawInherit = generatorClassSetting.HasInherit ? (" : " + generatorClassSetting.InheritName) : "";

			string formatClassName = string.Format ("public{0}class {1}{2}", drawPartial, drawClass, drawInherit);

			if (generatorClassSetting.drawSerializableAttribute) 
			{
				ProcessAddLine ("[System.Serializable]");
			}

			ProcessAddLine (formatClassName);
			ProcessAddLine ("{");
		}
	}
}