using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace Transmitter.Tool
{
	[CustomEditor(typeof(RefBinder))]
	public class RefBinderEditor : SerializedObjectEditor_FullOverride<RefBinder> {

		const float newRefGatherFieldWidth = 125f;
		const float buttonWidth = 125f;

		const float keyWidth = 125f;
		const float gameObjectWidth = 175f;

		const string refGatherKeyFieldName = "key";
		const string refGatherGOFieldDisplayName = "Gameobject";
		const string refGatherGOFieldName = "go";

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			
			DrawRefGathers ();

			serializedObject.ApplyModifiedProperties ();
		}

		void DrawRefGathers()
		{
			SerializedProperty refGathersProperty = serializedObject.FindProperty ("refGathers");

			EditorGUILayout.LabelField ("RefGathers :", titleNameGUIStyle);

			EditorGUI.indentLevel++;

			DrawRefGatherSerlizedField (refGathersProperty);

			EditorGUILayout.Space ();

			DrawRefGatherAddFunction (refGathersProperty);
			EditorGUI.indentLevel--;
		}

		void DrawRefGatherSerlizedField(SerializedProperty refGathersProperty)
		{
			EditorTool.DrawInVertical (()=>
				{
					int gatherSize = refGathersProperty.arraySize;
					if (gatherSize > 0) 
					{
						List<int> cacheRemoves = new List<int> ();

						EditorTool.DrawInHorizontal (()=>
							{
								EditorGUILayout.LabelField (refGatherKeyFieldName, GUILayout.Width (keyWidth));
								EditorGUILayout.LabelField (refGatherGOFieldDisplayName, GUILayout.Width (gameObjectWidth));
							});

						for (int i = 0; i < gatherSize; i++) 
						{
							EditorTool.DrawInHorizontal (()=>
								{
									SerializedProperty itemProperty = refGathersProperty.GetArrayElementAtIndex (i);

									bool hasRemove = DrawRefGatherItemAndGetRemove (itemProperty, refGathersProperty);

									if (hasRemove) 
									{
										cacheRemoves.Add (i);
									}
								});
						}


						if (cacheRemoves.Count != 0) 
						{
							// 理論上一次的draw只能點一顆按鈕 所以list長度只會是1 
							// 如果前面有移除過了 那應該要把移除後的對應index 再進行移除
							// ex要把0跟1移除掉 改成呼叫兩次移除0
							for (int i = 0; i < cacheRemoves.Count; i++) 
							{
								refGathersProperty.DeleteArrayElementAtIndex (cacheRemoves [i] - i);
							}
						}
					}
				});
		}

		bool DrawRefGatherItemAndGetRemove(SerializedProperty itemProperty,SerializedProperty rootProperty)
		{
			bool getRemove = false;

			EditorTool.DrawInHorizontal (()=>
				{
					SerializedProperty keyProperty = itemProperty.FindPropertyRelative (refGatherKeyFieldName);
					string originKey = keyProperty.stringValue;
					string newKey = EditorGUILayout.DelayedTextField (originKey, GUILayout.Width (keyWidth));

					if (newKey != originKey) 
					{
						int originSize = rootProperty.arraySize;

						bool checkContinue = true;

						for (int i = 0; i < originSize; i++) 
						{
							string otherItemKey = rootProperty.GetArrayElementAtIndex (i).FindPropertyRelative (refGatherKeyFieldName).stringValue;

							if (otherItemKey == newKey) 
							{
								checkContinue = EditorUtility.DisplayDialog ("Error", "已存在相同key之物件 是否還是要改為此key", "是", "否");

								break;
							}
						}

						if (checkContinue) 
						{
							keyProperty.stringValue = newKey;
						}
					}

					SerializedProperty goProperty = itemProperty.FindPropertyRelative (refGatherGOFieldName);
					UnityEngine.Object originBindTarget = goProperty.objectReferenceValue;
					UnityEngine.Object newBindTarget = EditorGUILayout.ObjectField (originBindTarget, typeof(GameObject), true, GUILayout.Width (gameObjectWidth));

					if (newBindTarget != originBindTarget) 
					{
						goProperty.objectReferenceValue = newBindTarget;
					}
					if (GUILayout.Button ("Remove", buttonGUIStyle, GUILayout.MaxWidth (buttonWidth)))
					{
						getRemove = true;
					}
				},EditorTool.GUILayout.boxStyle
			);

			return getRemove;
		}

		void DrawRefGatherAddFunction(SerializedProperty refGathersProperty)
		{
			GUILayout.Space (classIntervalSpace);

			EditorTool.DrawInHorizontal (()=>
				{
					newRefGatherKey = EditorGUILayout.DelayedTextField (newRefGatherKey, GUILayout.Width (keyWidth));

					UnityEngine.Object newGO = EditorGUILayout.ObjectField (newRefGatherGo, typeof(GameObject), true, GUILayout.Width (gameObjectWidth));

					//做修改的當下
					if(newGO!=newRefGatherGo)
					{
						newRefGatherGo = newGO;

						//如果本來有值那就保留 本來沒有值 才要替他補上
						if(newGO!=null)
						{
							if(string.IsNullOrEmpty(newRefGatherKey))
							{
								newRefGatherKey = newGO.name;
							}
						}
					}


					if (GUILayout.Button ("Add New Ref", buttonGUIStyle, GUILayout.MaxWidth (buttonWidth)))
					{
						if (newRefGatherGo != null) 
						{
							int originSize = refGathersProperty.arraySize;

							string newItemkey = newRefGatherKey;

							for (int i = 0; i < originSize; i++) 
							{
								string oldItemKey = refGathersProperty.GetArrayElementAtIndex (i).FindPropertyRelative (refGatherKeyFieldName).stringValue;
								if (oldItemKey == newItemkey) 
								{
									bool checkProcessNewKey = EditorUtility.DisplayDialog ("Error", "新自動生成key和原有key相同 是否加上*號作為標記", "是", "否");

									if (checkProcessNewKey) 
									{
										newItemkey = newItemkey.Insert (0, "*");
									}
									break;
								}
							}

							refGathersProperty.InsertArrayElementAtIndex (originSize);
							SerializedProperty newItem = refGathersProperty.GetArrayElementAtIndex (originSize);
							newItem.FindPropertyRelative (refGatherKeyFieldName).stringValue = newItemkey;
							newItem.FindPropertyRelative (refGatherGOFieldName).objectReferenceValue = newRefGatherGo;

							newRefGatherGo = null;
							newRefGatherKey = "";
						}
						else
						{
							EditorUtility.DisplayDialog ("Error", "欲寫入之物件為空", "ok");
						}

					}
				},EditorTool.GUILayout.boxStyle);

		}

		UnityEngine.Object newRefGatherGo = null;
		string newRefGatherKey;
	}
}
