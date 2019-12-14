using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Tool;

namespace Transmitter.Demo
{
	[CustomEditor(typeof(NetworkPlayer))]
	public class NetworkPlayerEditor : SerializedObjectEditor<NetworkPlayer> 
	{
		string textMsg;
		
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			GUILayout.Space (10);

			if (GUILayout.Button ("Connect",GUILayout.Width(100),GUILayout.Height(30)))
			{
				
			}

			GUILayout.Space (10);

			EditorTool.DrawInHorizontal (()=>
				{
					EditorGUILayout.LabelField("msg : ",textMsg,GUILayout.Width(35));
					textMsg = GUILayout.TextArea (textMsg);
				});
		}

	}
}
