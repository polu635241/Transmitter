//#define keyMsg
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Transmitter.TypeSettingDataFactory.Model;

namespace Transmitter.Tool
{
	public class MessageBlackboard : GeneralEditorWindow 
	{
		static MessageBlackboard instance;

		Vector2 typeWindowSize = new Vector2 (300, 400);
		Vector2 enumWindowSize = new Vector2 (300, 400);
		const int GridInterval = 20;
		const int BoundryGridInterval = 100;

		const float moveSpeed = 0.5f;
		const string NullBindDataMessage = "None.....";
		const string NewEnumItem = "*";

		GUIStyle EnumGUIStyle
		{
			get
			{
				if (enumGUIStyle == null) 
				{
					enumGUIStyle = new GUIStyle (EditorStyles.label);
					enumGUIStyle.fontStyle = FontStyle.Bold;
					enumGUIStyle.normal.textColor = Color.green;
				}

				return enumGUIStyle;
			}
		}

		GUIStyle enumGUIStyle;

		GUIStyle NoBindEnumGUIStyle
		{
			get
			{
				if (noBindEnumGUIStyle == null) 
				{
					noBindEnumGUIStyle = new GUIStyle (EditorStyles.label);
					noBindEnumGUIStyle.fontStyle = FontStyle.Bold;
					noBindEnumGUIStyle.normal.textColor = Color.red;
				}

				return noBindEnumGUIStyle;
			}
		}

		GUIStyle noBindEnumGUIStyle;

		[MenuItem("Transmitter/MessageBlackboard/Refresh _F2")]
		static void Refresh ()
		{
			if (instance != null) 
			{
				instance.Close ();
			}

			Show ();
		}

		[MenuItem("Transmitter/MessageBlackboard/Show")]
		public static void Show ()
		{
			if (instance != null) 
			{
				SingleDialogCondition ();

				instance.Focus ();
			}
			else
			{
				MessageBlackboard messageBlackboard = GetWindow<MessageBlackboard> ("MessageBlackboard");
				instance = messageBlackboard;
			}
		}

		List<RefKeyValuePair<Rect,TypeWindowData>> typeWindowPairs = new List<RefKeyValuePair<Rect, TypeWindowData>> ();
		List<string> enumNamesCache = new List<string> ();

		List<RefKeyValuePair<Rect,EnumWindowData>> enumWindowPairs = new List<RefKeyValuePair<Rect, EnumWindowData>> ();
		int typeWindowsCountCache;

		Vector2 mousePos_Mid;

		MessageSettingCacheData messageSettingCacheData;

		MessageSettingData messageSettingData;

		List<TypeWindowData> waitRemoveTypeWindowDatas = new List<TypeWindowData> ();
		List<EnumWindowData> waitRemoveEnumWindowDatas = new List<EnumWindowData> ();

		List<TypeSettingData> waitDeleteTypeDatas = new List<TypeSettingData> ();
		List<EnumSettingData> waitDeleteEnumDatas = new List<EnumSettingData> ();

		void OnEnable ()
		{
			RefreshSetting ();
		}

		void OnGUI ()
		{
			Event currentEvent = Event.current;

			//網格先畫 圖層最下面
			DrawGrids();

			BeginWindows ();
			{
				ReFreshCache ();

				typeWindowPairs.Map ((index,pair)=>
					{
						pair.key = GUILayout.Window(index, pair.key, DrawTypeWindow, "Type");
					});

				enumWindowPairs.Map ((index,pair)=>
					{
						int enumSettingWindowIndex = typeWindowsCountCache + index;
						pair.key = GUILayout.Window(enumSettingWindowIndex, pair.key, DrawEnumWindow, "Enum");
					});
			}
			EndWindows ();

			ProcessEvent (currentEvent);
			ProcessRemoveWindows ();
			ProcessDeleteDatas ();
		}

		void ReFreshCache ()
		{
			typeWindowsCountCache = typeWindowPairs.Count;

			enumNamesCache = new List<string> ();

			messageSettingData.enumSettingDatas.ForEach (data => enumNamesCache.Add (data.enumName));

			enumNamesCache.Add (NullBindDataMessage);
		}

		void DrawTypeWindow (int windowID)
		{
			TypeWindowData windowSettingData = GetTypeWindowSettingData (windowID);
			
			GUILayout.Button ("Type");
			GUI.DragWindow ();
		}

		void DrawEnumWindow (int windowID)
		{
			EnumWindowData windowSettingData = GetEnumWindowSettingData (windowID);
			EnumSettingData bindEnumSettingData = windowSettingData.BindData;

			EditorGUILayout.Space ();

			DrawSwitchBindModule (bindEnumSettingData, windowID);

			EditorGUILayout.Space ();

			//已經綁定完畢
			if (bindEnumSettingData != null) 
			{
				DrawEnumItems (bindEnumSettingData);
			}

			//置底
			GUILayout.FlexibleSpace ();

			EditorTool.DrawInHorizontal (()=>
				{
					if (bindEnumSettingData != null) 
					{
						DrawDeleteEnum (windowSettingData);
					}

					DrawCloseEnum (windowSettingData);
				});

			GUI.DragWindow ();
		}

		void DrawDeleteEnum (EnumWindowData windowSettingData)
		{
			GUILayout.Space (3);

			if(GUILayout.Button("Delete", GUILayout.Width (buttonWidth)))
			{
				if (RemoveDialogCondition.Invoke ())
				{
					waitDeleteEnumDatas.Add (windowSettingData.BindData);
					waitRemoveEnumWindowDatas.Add (windowSettingData);
				}	
			}
		}

		void DrawCloseEnum (EnumWindowData windowSettingData)
		{
			GUILayout.FlexibleSpace ();

			if(GUILayout.Button("Close", GUILayout.Width (buttonWidth)))
			{
				waitRemoveEnumWindowDatas.Add (windowSettingData);
			}

			GUILayout.Space (10);
		}

		void DrawSwitchBindModule (EnumSettingData bindEnumSettingData, int windowID)
		{
			bool hasBind = true;

			string currentTitle;

			if (bindEnumSettingData != null)
			{
				currentTitle = bindEnumSettingData.enumName;
				hasBind = true;
			}
			else
			{
				currentTitle = NullBindDataMessage;
				hasBind = false;
			}

			GUIStyle enumNameGUIStyle = hasBind ? EnumGUIStyle : NoBindEnumGUIStyle;

			EditorTool.DrawInHorizontal (() =>
			{
				EditorGUILayout.LabelField (currentTitle, enumNameGUIStyle);

				if (GUILayout.Button ("Switch", GUILayout.Width (buttonWidth)))
				{
					Action<string> switchBindData = (bindDataValue) =>
					{
						//可能會延遲執行 所以重新抓取
						EnumWindowData _WindowSettingData = GetEnumWindowSettingData (windowID);
						_WindowSettingData.BindData = GetEnumSettingData (bindDataValue);
					};

					Pop_UpSelectWindow.ShowWindow (switchBindData, currentTitle, enumNamesCache.ToArray ());
				}

				GUILayout.Space (10);
			});
		}

		void DrawEnumItems (EnumSettingData bindEnumSettingData)
		{
			List<string> enumItems = bindEnumSettingData.items;

			Action<int> onRemoveEnumItem = (removeIndex) => 
			{
				enumItems.RemoveAt (removeIndex);
			};

			Action<int> onCloneEnumItem = (cloneIndex) => 
			{
				string enumItem = enumItems [cloneIndex];
				enumItems.Insert (cloneIndex + 1, enumItem);
			};

			RemoveAndCloneCache enumItemsRemoveAndCloneCache = new RemoveAndCloneCache (onRemoveEnumItem, onCloneEnumItem);

			EditorTool.DrawInVertical (() => 
				{
					if (bindEnumSettingData != null) 
					{
						enumItems.Map ((index,enumValue)=>
							{
								DrawEnumItem (index, enumValue, bindEnumSettingData, enumItemsRemoveAndCloneCache);
							});

						enumItemsRemoveAndCloneCache.Flush ();

						GUILayout.Space (10);

						if(GUILayout.Button ("Add", GUILayout.Width (buttonWidth)))
						{
							enumItems.Add (NewEnumItem);
						}
					}
				}, BoxGUIStyle);
		}

		void DrawEnumItem (int index, string enumValue, EnumSettingData bindEnumSettingData, RemoveAndCloneCache enumItemsRemoveAndCloneCache)
		{
			EditorGUILayout.Space ();
			
			EditorTool.DrawInHorizontal(()=>
				{
					//這裡取按鈕的寬度 是為了版型
					string newEnumValue = EditorGUILayout.TextField (enumValue, GUILayout.Width (buttonWidth), GUILayout.Height (20));

					GUILayout.FlexibleSpace ();

					if(newEnumValue != enumValue)
					{
						bindEnumSettingData.items[index] = newEnumValue;
						SaveToEntity ();
					}

					if(GUILayout.Button("Remove", GUILayout.Width (buttonWidth)))
					{
						if(RemoveDialogCondition.Invoke())
						{
							enumItemsRemoveAndCloneCache.RemoveIndex = index;
						}
					}

					GUILayout.Space (10);

					if(GUILayout.Button ("Clone", GUILayout.Width (buttonWidth)))
					{
						enumItemsRemoveAndCloneCache.CloneIndex = index;
					}

					GUILayout.Space (7);
				});
		}

		void DrawGrids()
		{
			DrawGrid (GridInterval, TranslucentBlack);
			//透過重疊 劃出較粗的外框線
			DrawGrid (BoundryGridInterval, TranslucentBlack);
		}

		Vector2 windowDeltaPos;

		void DrawGrid(float gridInterval, Color gridColor)
		{
			int horzCount = Mathf.CeilToInt (position.width / gridInterval);
			int verticalCount = Mathf.CeilToInt (position.height / gridInterval);

			EditorTool.DrawInHandles(()=>
				{
					Handles.color = gridColor;
					//因為底圖不是無盡的延伸 如果直接用原始的偏移量當作基礎的話 框線會被拉到螢幕外部
					//只要關注被求餘數後的偏移量就好
					Vector3 processOffset = new Vector3 (windowDeltaPos.x % gridInterval, windowDeltaPos.y % gridInterval, 0);

					for (int i = 0 ; i < horzCount ; i++)
					{
						Handles.DrawLine (new Vector3 (gridInterval * i, -gridInterval, 0) + processOffset, new Vector3 (gridInterval * i, position.height, 0f) + processOffset);
					}

					for (int i = 0 ; i < verticalCount ; i++)
					{
						Handles.DrawLine (new Vector3 (-gridInterval, gridInterval * i, 0) + processOffset, new Vector3 (position.width, gridInterval * i, 0f) + processOffset);
					}

					Handles.color = Color.white;
				});
		}

		void ProcessEvent (Event e)
		{	
			//攔截到就直接return 最後才給空白區

			Vector2 mousePosition = e.mousePosition;

			bool inTypeWindow = typeWindowPairs.Exists (pair=>
				{
					return pair.key.Contains (mousePosition);
				});

			if (inTypeWindow) 
			{
				return;
			}

			bool inEnumWindow = enumWindowPairs.Exists (pair=>
				{
					return pair.key.Contains (mousePosition);
				});

			if (inEnumWindow) 
			{
				return;
			}
			
			ProcessTempSpaceEvent (e);
		}

		void ProcessRemoveWindows ()
		{
			if (waitRemoveTypeWindowDatas.Count != 0) 
			{
				waitRemoveTypeWindowDatas.ForEach (data=>
					{
						typeWindowPairs.Remove (pair => pair.value== data);
					});

				waitRemoveTypeWindowDatas.Clear ();
			}
			
			if (waitRemoveEnumWindowDatas.Count != 0) 
			{
				waitRemoveEnumWindowDatas.ForEach (data=>
					{
						enumWindowPairs.Remove (pair => pair.value== data);
					});
				
				waitRemoveEnumWindowDatas.Clear ();
			}
		}

		void ProcessDeleteDatas ()
		{
			bool needSaveEntity = false;
			
			if (waitDeleteTypeDatas.Count != 0) 
			{
				needSaveEntity = true;
				
				waitDeleteTypeDatas.ForEach (data=>
					{
						messageSettingData.typeSettingDatas.Remove (data);
					});

				waitDeleteTypeDatas.Clear ();
			}

			if (waitDeleteEnumDatas.Count != 0) 
			{
				needSaveEntity = true;
				
				waitDeleteEnumDatas.ForEach (data=>
					{
						messageSettingData.enumSettingDatas.Remove (data);
					});

				waitDeleteEnumDatas.Clear ();
			}

			if (needSaveEntity) 
			{
				SaveToEntity ();
			}
		}

		/// <summary>
		/// 當手點到空白處的事件
		/// </summary>
		/// <param name="e">E.</param>
		void ProcessTempSpaceEvent (Event e)
		{	
			switch (e.type)
			{
			case EventType.MouseDown:
				{
					TempSpaceMouseDownEvent (e);
					break;
				}

			case EventType.MouseDrag:
				{
					TempSpaceMouseDragEvent (e);
					break;
				}

			case EventType.MouseUp:
				{
					TempSpaceMouseUpEvent (e);
					break;
				}
			}
		}

		void TempSpaceMouseDownEvent (Event e)
		{
			#if keyMsg
			Debug.Log ($"MouseDown -> {e.button}");
			#endif

			Vector2 mousePosition = e.mousePosition;

			if (e.button == 1) 
			{
				GenericMenu genericMenu = new GenericMenu ();

				genericMenu.AddItem (new GUIContent ("Setting Type"), false, () => CreateTypeWindow (mousePosition));
				genericMenu.AddItem (new GUIContent ("Setting Enum"), false, () => CreateEnumWindow (mousePosition));
				genericMenu.ShowAsContext ();
			}

			if (e.button == 2) 
			{
				mousePos_Mid = e.mousePosition;
			}
		}

		void CreateTypeWindow(Vector2 pos)
		{
			Rect windowRect = new Rect (pos.x, pos.y, typeWindowSize.x, typeWindowSize.y);
			TypeWindowData typeWindowSettingData = new TypeWindowData ();

			typeWindowPairs.Add (new RefKeyValuePair<Rect, TypeWindowData> (windowRect, typeWindowSettingData));
		}

		void CreateEnumWindow (Vector2 pos)
		{
			Rect windowRect = new Rect (pos.x, pos.y, enumWindowSize.x, enumWindowSize.y);
			EnumWindowData enumWindowSettingData = new EnumWindowData ();

			enumWindowPairs.Add (new RefKeyValuePair<Rect, EnumWindowData> (windowRect, enumWindowSettingData));
		}

		void TempSpaceMouseDragEvent (Event e)
		{
			#if keyMsg
			Debug.Log ($"MouseDrag -> {e.button}");
			#endif

			if (e.button == 2) 
			{
				Vector2 newMousePos = e.mousePosition;
				Vector2 deltaMousePos = newMousePos - mousePos_Mid;
				mousePos_Mid = newMousePos;

				ProcessRectPosDelta (deltaMousePos);
			}
		}

		void TempSpaceMouseUpEvent (Event e)
		{
			#if keyMsg
			Debug.Log ($"MouseUp -> {e.button}");
			#endif

			if (e.button == 2) 
			{

			}
		}

		void ProcessRectPosDelta (Vector2 deltaPos)
		{
			Vector2 processDeltaPos = deltaPos * moveSpeed;

			enumWindowPairs.ForEach ((pair)=>
				{
					ModifyRectPosDelta(processDeltaPos ,ref pair.key);
				});

			windowDeltaPos += processDeltaPos;
			Repaint ();
		}

		void ModifyRectPosDelta (Vector2 deltaPos, ref Rect rect)
		{
			float processPosX = rect.x + deltaPos.x;
			float processPosY = rect.y + deltaPos.y;

			rect = new Rect (processPosX, processPosY, rect.width, rect.height);
		}

		void RefreshSetting ()
		{
			messageSettingCacheData = EditorTool.GetCacheData<MessageSettingCacheData> ();
			messageSettingData = messageSettingCacheData.MessageSettingData;
			windowDeltaPos = Vector2.zero;
			typeWindowPairs = new List<RefKeyValuePair<Rect, TypeWindowData>> ();
			enumWindowPairs = new List<RefKeyValuePair<Rect, EnumWindowData>> ();
		}

		void SaveToEntity ()
		{
			EditorUtility.SetDirty(messageSettingCacheData);
		}

		TypeWindowData GetTypeWindowSettingData (int selectIndex)
		{
			if (selectIndex <= typeWindowsCountCache - 1) 
			{
				return typeWindowPairs [selectIndex].value;
			}
			else
			{
				Debug.LogError ($"index is out of range count -> {typeWindowsCountCache}, index -> {selectIndex}");
				return new TypeWindowData ();
			}
		}

		EnumWindowData GetEnumWindowSettingData (int selectIndex)
		{
			int processSelectIndex = selectIndex - typeWindowsCountCache;
			int enumWindowsCount = enumWindowPairs.Count;

			if (processSelectIndex <= enumWindowsCount - 1) 
			{
				return enumWindowPairs [processSelectIndex].value;
			}
			else
			{
				Debug.LogError ($"index is out of range typeCount -> {typeWindowsCountCache}, enumCount -> {enumWindowsCount}, index -> {processSelectIndex}");
				return new EnumWindowData ();
			}
		}

		EnumSettingData GetEnumSettingData (string enumSettingDataName)
		{	
			EnumSettingData enumSettingData = messageSettingData.enumSettingDatas.Find (data => data.enumName == enumSettingDataName);

			if (enumSettingData != null) 
			{
				return enumSettingData;
			}
			else
			{
				Debug.LogError ($"can't get EnumSettingData -> {enumSettingDataName}");
				
				return null;
			}
		}


		private class TypeWindowData
		{
			public TypeSettingData BindData
			{
				get
				{
					return bindData;
				}
			}

			TypeSettingData bindData;
		}

		private class EnumWindowData
		{
			public EnumWindowData ()
			{
				BindData = null;
			}
			
			public EnumSettingData BindData{ get; set;}
		}
	}
		
}