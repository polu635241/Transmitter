using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Tool;

namespace Transmitter.Demo
{
	[CustomEditor(typeof(DialogBox))]
	public class DialogBoxEditor : SerializedObjectEditor<DialogBox> 
	{
		string textMsg;
		
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			GUILayout.Space (10);

			if (GUILayout.Button ("Test input",GUILayout.Width(100),GUILayout.Height(30)))
			{
				runtimeScript.Input (textMsg);
			}

			GUILayout.Space (10);

			EditorTool.DrawInHorizontal (()=>
				{
					EditorGUILayout.LabelField("msg : ",textMsg,GUILayout.Width(35));
					textMsg = GUILayout.TextArea (textMsg);
				});

			GUILayout.Space (10);

			if (GUILayout.Button ("GetDescription",GUILayout.Width(100),GUILayout.Height(30)))
			{
				runtimeScript.GetDescription ();
			}
		}

	}
}
