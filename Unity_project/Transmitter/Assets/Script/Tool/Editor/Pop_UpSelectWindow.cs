//#define debugMode
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Transmitter.Tool
{
	public class Pop_UpSelectWindow : GeneralEditorWindow
	{
		public static void ShowWindow (SerializedProperty serializedProperty, string[] originItems, string[] descriptions = null)
		{
			Action<string> flushCallback = (newStr) => 
			{
				serializedProperty.stringValue = newStr;
				serializedProperty.serializedObject.ApplyModifiedProperties ();
			};

			string currentValue = serializedProperty.stringValue;

			ShowWindow (flushCallback, currentValue, originItems, descriptions);
		}

		public static void ShowWindow (Action<string> flushCallback, string currentValue, string[] originItems, string[] descriptions = null)
		{
			Pop_UpSelectWindow customSelectWindow = GetWindow<Pop_UpSelectWindow>();
			customSelectWindow.Init (flushCallback, currentValue, originItems, descriptions);
			customSelectWindow.Show ();
		}
		Texture2D selectIcon;

		string cacheSearch;

		List<string> originItems = new List<string> ();
		List<string> mixItems = new List<string> ();

		List<string> processItems = new List<string> ();

		Action<string> flushCallback;

		string currentValue;
		int currentIndex;

		GUIStyle normalStyle;

		GUIStyle selectedStyle;

		GUIContent searchContent;

		void Init (Action<string> flushCallback, string currentValue, string[] originItems, string[] descriptions)
		{
			this.flushCallback = flushCallback;
			this.currentValue = currentValue;
			this.originItems = new List<string> (originItems);

			InitGUIContent ();

			if (descriptions != null && descriptions.Length > 0) 
			{
				mixItems = new List<string> ();

				if (descriptions.Length == originItems.Length) 
				{
					for (int i = 0; i < descriptions.Length; i++) 
					{
						mixItems.Add (descriptions [i] + originItems [i]);
					}
				}
				else
				{
					Debug.LogError ("描述檔與原檔長度不合");
					mixItems = new List<string> (originItems);
				}
			}
			else
			{
				mixItems = new List<string> (originItems);
			}

			currentIndex = this.originItems.FindIndex(item => item == currentValue);

			processItems = new List<string> (this.mixItems);

			float beginPosY = GetFocusPosY (currentIndex);

			scrollPos = new Vector2 (0, beginPosY);
		}

		void OnGUI()
		{
			if (setClose)
			{
				return;
			}

			Event currentEvent = Event.current;

			DrawSearchField();

			ProcessKeyEvent(currentEvent);

			#if debugMode
			EditorGUILayout.LabelField ($"currentEvent -> {currentEvent}");
			EditorGUILayout.LabelField ($"currentIndex -> {currentIndex}");
			#endif

			scrollPos = EditorTool.DrawInScrollView(scrollPos, scrollBarHeight, () =>
				{
					processItems.Map((index, item) =>
						{
							DrawItem(index, item, currentEvent);
						});
				});

			if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow != this)
			{
				setClose = true;
			}
		}

		void Update ()
		{
			if (setClose)
			{
				Close();
			}
		}

		void DrawSearchField()
		{
			EditorTool.DrawInHorizontal(() =>
				{
					EditorGUILayout.LabelField("Search : ", GUILayout.Width(70), GUILayout.Height(flatButtonHigh));
					cacheSearch = EditorGUILayout.TextField(cacheSearch, EditorStyles.textField, GUILayout.Height(flatButtonHigh));

					if (GUILayout.Button(searchContent, GUILayout.Width(buttonWidth), GUILayout.Height(flatButtonHigh)))
					{
						if (!string.IsNullOrEmpty(cacheSearch))
						{
							string processSearch = cacheSearch.ToLower();

							processItems = mixItems.FindAll(item => item.ToLower().Contains(processSearch));
						}
						else
						{
							processItems = new List<string> (mixItems);
						}

						currentIndex = processItems.IndexOf(currentValue);
					}
				});
		}

		void ProcessKeyEvent(Event currentEvent)
		{
			if (currentEvent.type == EventType.KeyDown)
			{
				if (currentEvent.keyCode == KeyCode.UpArrow)
				{
					if (currentIndex > 0)
					{
						currentIndex--;

						currentValue = originItems[currentIndex];
					}

					currentEvent.Use();
				}

				if (currentEvent.keyCode == KeyCode.DownArrow)
				{
					if (currentIndex < processItems.Count - 1)
					{
						currentIndex++;

						currentValue = originItems[currentIndex];
					}

					currentEvent.Use();
				}

				if (currentEvent.keyCode == KeyCode.Return)
				{
					Flush();

					currentEvent.Use();
				}
			}
		}

		bool setClose;

		void DrawItem(int index, string item, Event currentEvent)
		{
			GUIStyle guiStyle;

			if (index == currentIndex)
			{
				guiStyle = selectedStyle;
			}
			else
			{
				guiStyle = normalStyle;
			}

			EditorGUILayout.LabelField(item, guiStyle, GUILayout.Height(flatButtonHigh));

			Rect rect = GUILayoutUtility.GetLastRect();

			if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
			{
				if (rect.Contains(currentEvent.mousePosition))
				{
					currentIndex = index;
					currentValue = item;
					currentEvent.Use();

					if (currentEvent.clickCount >= 2)
					{
						Flush();
					}
				}
			}
		}

		void Flush()
		{
			int index = mixItems.IndexOf (currentValue);

			string originValue = originItems [index];

			flushCallback.Invoke (originValue);
			setClose = true;
		}

		void InitGUIContent ()
		{
			selectIcon = new Texture2D(1, 1);
			Color origanColor = new Color(1, 0.517701f, 0, 1);
			selectIcon.SetPixels(new Color[] { origanColor });
			selectIcon.Apply();
			searchContent = EditorGUIUtility.IconContent("ViewToolZoom");

			normalStyle = new GUIStyle(EditorStyles.label);

			selectedStyle = new GUIStyle(normalStyle);

			selectedStyle.normal.background = selectIcon;

			selectedStyle.normal.textColor = Color.black;
		}

		float GetFocusPosY (int selectIndex)
		{
			float focusPosY = (selectIndex) * flatButtonHigh + (selectIndex - 1) * 2 - scrollBarHeight / 2 - 2;

			if (focusPosY < 0) 
			{
				focusPosY = 0;
			}

			return focusPosY;
		}
	}
}