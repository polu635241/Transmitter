using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Transmitter.Tool
{
	public static class EditorTool 
	{
		public static void DrawInReadOnly(Action body)
		{
			GUI.enabled = false;
			body.Invoke ();
			GUI.enabled = true;
		}

		public static void DrawInHorizontal(Action body,GUIStyle style=null)
		{
			if (style == null) 
			{
				EditorGUILayout.BeginHorizontal ();
			}
			else 
			{
				EditorGUILayout.BeginHorizontal (style);
			}
			
			body.Invoke ();
			EditorGUILayout.EndHorizontal ();
		}

		public static void DrawInVertical(Action body,GUIStyle style=null)
		{
			if (style == null) 
			{
				EditorGUILayout.BeginVertical ();
			}
			else 
			{
				EditorGUILayout.BeginVertical (style);
			}

			body.Invoke ();
			EditorGUILayout.EndVertical ();
		}

		public static void DrawInProperty (Rect position, SerializedProperty property, GUIContent label, Action body)
		{
			EditorGUI.BeginProperty (position, label, property);
			{
				body.Invoke ();
			}
			EditorGUI.EndProperty ();
		}

		public static void DrawInNoIndent (Action body)
		{
			int originIndent = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;
			{
				body.Invoke ();
			}
			EditorGUI.indentLevel = originIndent;
		}

		public static void DrawInIndent (Action body)
		{
			EditorGUI.indentLevel++;
			{
				body.Invoke ();
			}
			EditorGUI.indentLevel--;
		}

		public static Vector2 DrawInScrollView (Vector2 scorllPosition, float scrollBarHeight, Action body)
		{
			Vector2 newPosition;

			newPosition = EditorGUILayout.BeginScrollView (scorllPosition, new GUIStyle (), GUILayout.Height (scrollBarHeight));
			{
				body.Invoke ();
			}
			EditorGUILayout.EndScrollView ();

			return newPosition;
		}

		public static void DrawInHandles (Action body)
		{ 
			Handles.BeginGUI ();
			{
				body.Invoke ();
			}
			Handles.EndGUI ();
		}

		public static void ForEach(this SerializedProperty arrayProperty,Action<SerializedProperty> loopAction)
		{
			int originSize = arrayProperty.arraySize;

			for (int i = 0; i < originSize; i++) 
			{
				SerializedProperty item = arrayProperty.GetArrayElementAtIndex (i);
				loopAction.Invoke (item);
			}
		}

		public static void ForEach(this SerializedProperty arrayProperty,Action<SerializedProperty,int> loopAction)
		{
			int originSize = arrayProperty.arraySize;

			for (int i = 0; i < originSize; i++) 
			{
				SerializedProperty item = arrayProperty.GetArrayElementAtIndex (i);
				loopAction.Invoke (item, i);
			}
		}

		public static T GetCacheData<T> ()  where T : ScriptableObject
		{
			string[] dataGuids = UnityEditor.AssetDatabase.FindAssets ($"t:{typeof(T)}", null);
			string dataPath = UnityEditor.AssetDatabase.GUIDToAssetPath(dataGuids[0]);
			return UnityEditor.AssetDatabase.LoadAssetAtPath<T> (dataPath);
		}
	}
}