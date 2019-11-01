using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;
using Transmitter.Tool;
using Transmitter.TypeSettingDataFactory;
using Transmitter.TypeSettingDataFactory.Model;

namespace Transmitter.Serialize
{
	public class MessageSettingDataUtility {

		[UnityEditor.MenuItem("MessageRouter/RefreshMessageSettingData")]
		static void RefreshMessageSettingData()
		{
			string packageDefineText = Transmitter.Tool.Tool.GetJson ("PackageDefine");
			MessageSettingData messageSettingData = MessageSettingDataFactory.Create (packageDefineText);

			string packageDefineUtitltyText = Transmitter.Tool.Tool.GetJson ("PackageDefineUtility");

			Dictionary<string,object> packageDefineUtitltyDict = (Dictionary<string,object>)Json.Deserialize (packageDefineUtitltyText);

			object md5Node = null;
			string md5ext = "";

			if (packageDefineUtitltyDict.TryGetValue ("md5", out md5Node)) 
			{
				md5ext = md5Node.ToString ();
			}

			MD5 md5 = MD5.Create();
			byte[] source = Encoding.Default.GetBytes (JsonUtility.ToJson (messageSettingData));
			byte[] crypto = md5.ComputeHash(source);
			string newMd5Text = Convert.ToBase64String(crypto);

			if (md5ext!=newMd5Text) 
			{
				string newPackageDefineUtitltyText = Transmitter.Tool.Tool.SetJsonNode (packageDefineUtitltyText, "md5", newMd5Text);
				Transmitter.Tool.Tool.SaveJson ("PackageDefineUtility", newPackageDefineUtitltyText);
				ScriptEntityFactory.UpdateScript (messageSettingData);
			}
			else
			{
				Debug.LogError ("設定檔與上次並無變化");
			}
		}

		public static List<string> basisTypes = new List<string> () {
			"bool",
			"byte",
			"sbyte",
			"char",
			"decimal",
			"double",
			"float",
			"int",
			"uint",
			"long",
			"short",
			"ushort",
			"string"
		};
	}
}