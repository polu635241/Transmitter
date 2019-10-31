using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public class NamespaceGeneratorData:GeneratorData
	{
		List<string> usingNamespaces = new List<string>();
		string belongNamespace = "";

		public NamespaceGeneratorData (List<string> usingNamespaces, string belongNamespace, List<GeneratorData> nodes) : base (nodes)
		{
			this.usingNamespaces = usingNamespaces;
			this.belongNamespace = belongNamespace;
		}

		protected override void GeneratorContent ()
		{
			//加上引用的空間
			usingNamespaces.ForEach (_namespace => 
				{
					string processUsingLine = GetMixNamespace(_namespace);
					ProcessAddLine(processUsingLine);
				});
			AddTempLine ();
			ProcessAddLine (@"//" + declareMessage);
			string belongNamespaceStr = "namespace" + GetSpace () + belongNamespace;

			ProcessAddLine (belongNamespaceStr);
			ProcessAddLine ("{");
		}


		string GetMixNamespace(string usingNamespace)
		{
			return string.Format ("using{0}{1};", GetSpace (), usingNamespace);
		}
	}
}