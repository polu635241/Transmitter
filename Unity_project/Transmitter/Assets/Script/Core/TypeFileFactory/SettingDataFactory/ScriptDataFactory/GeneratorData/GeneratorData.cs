﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	public abstract class GeneratorData
	{
		protected const string declareMessage = 
			"此檔案由SerializableFactory 自動生成 為配合效能最大化 請勿在意腳本封裝性";
		
		int beginSpace;

		public GeneratorData(List<GeneratorData> nodes)
		{
			if (nodes != null)
				this.nodes = nodes;
			else
				this.nodes = new List<GeneratorData>();

			collectionLine = new List<string> ();
		}

		protected List<GeneratorData> nodes = new List<GeneratorData>();

		public List<string> GetGeneratorLines (int beginSpace = 0)
		{
			Enter (beginSpace);
			GeneratorContent ();
			GenerateNodesContent ();

			if (ExitHook)
				collectionLine.Add (Exit ());

			return collectionLine;
		}

		void GenerateNodesContent ()
		{
			collectionLine.AddRange (GetNodesContent ());
		}

		protected abstract void GeneratorContent ();

		List<string> GetNodesContent ()
		{
			List<string> nodeLines = new List<string> ();

			int nodeBeginSpace = beginSpace + 1;

			for (int i = 0; i < nodes.Count; i++) 
			{
				GeneratorData node = nodes [i];

				nodeLines.AddRange(node.GetGeneratorLines(nodeBeginSpace));

				//最後一個不用空一行
				if (i < nodes.Count - 1)
					nodeLines.Add ("");
			}

			return nodeLines;
		}


		protected virtual bool ExitHook
		{
			get
			{
				return true;
			}
		}

		void Enter (int beginSpace)
		{
			this.beginSpace = beginSpace;
			collectionLine = new List<string> ();
		}

		string Exit()
		{
			string endLine = GetSpace (beginSpace * 4) + "}";
			return endLine;
		}

		List<string> collectionLine = new List<string>();

		private void ResetCollectionLine()
		{
			collectionLine = new List<string> ();
		}

		protected void ProcessAddLine(string newLine,int innerBeginTab = 0)
		{								  //一個tab 4個空格
			collectionLine.Add (GetSpace ((beginSpace + innerBeginTab) * 4) + newLine);
		}

		protected void ProcessAddLines(List<string> newLines,int innerBeginTab = 0)
		{
			newLines.ForEach (newLine=>
				{								   //一個tab 4個空格
					collectionLine.Add (GetSpace ((beginSpace + innerBeginTab) * 4) + newLine);
				});
		}

		protected List<string> GetCollectionLines()
		{
			return collectionLine;
		}


		protected string GetSpace (int count = 1)
		{
			StringBuilder sb = new StringBuilder ();

			for (int i = 0; i < count; i++) 
			{
				sb.Append (" ");
			}

			return sb.ToString ();
		}

		protected void AddTempLine ()
		{
			collectionLine.Add ("");
		}
	}
}