//#define keyMsg
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

		const float moveSpeed = 0.5f;

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
				instance.RefreshSetting ();
			}
		}

		List<RefKeyValuePair<Rect,TypeWindowSettingData>> typeWindowPairs = new List<RefKeyValuePair<Rect, TypeWindowSettingData>> ();
		int typeWindowsCountCache;
		List<RefKeyValuePair<Rect,EnumWindowSettingData>> enumWindowPairs = new List<RefKeyValuePair<Rect, EnumWindowSettingData>> ();

		Vector2 mousePos_Mid;

		MessageSettingCacheData messageSettingCacheData;

		MessageSettingData messageSettingData;

		void OnGUI ()
		{
			Event currentEvent = Event.current;

			//網格先畫 圖層最下面
			DrawGrids();

			BeginWindows ();
			{
				typeWindowsCountCache = typeWindowPairs.Count;

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
		}

		void DrawTypeWindow (int windowID)
		{
			TypeWindowSettingData typeWindowSettingData = GetTypeWindowSettingData (windowID);
			
			GUILayout.Button ("Type");
			GUI.DragWindow ();
		}

		void DrawEnumWindow (int windowID)
		{
			EnumWindowSettingData enumWindowSettingData = GetEnumWindowSettingData (windowID);

			GUILayout.Button ("Enum");
			GUI.DragWindow ();
		}

		void DrawGrids()
		{
			DrawGrid (20f, TranslucentBlack);
			//透過重疊 劃出較粗的外框線
			DrawGrid (100f, TranslucentBlack);
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
			Rect windowRect = new Rect (pos.x, pos.y, 200, 200);
			TypeWindowSettingData typeWindowSettingData = new TypeWindowSettingData ();

			typeWindowPairs.Add (new RefKeyValuePair<Rect, TypeWindowSettingData> (windowRect, typeWindowSettingData));
		}

		void CreateEnumWindow (Vector2 pos)
		{
			Rect windowRect = new Rect (pos.x, pos.y, 200, 200);
			EnumWindowSettingData enumWindowSettingData = new EnumWindowSettingData ();

			enumWindowPairs.Add (new RefKeyValuePair<Rect, EnumWindowSettingData> (windowRect, enumWindowSettingData));
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
			typeWindowPairs = new List<RefKeyValuePair<Rect, TypeWindowSettingData>> ();
			enumWindowPairs = new List<RefKeyValuePair<Rect, EnumWindowSettingData>> ();
		}

		TypeWindowSettingData GetTypeWindowSettingData (int selectIndex)
		{
			if (selectIndex <= typeWindowsCountCache - 1) 
			{
				return typeWindowPairs [selectIndex].value;
			}
			else
			{
				Debug.LogError ($"index is out of range count -> {typeWindowsCountCache}, index -> {selectIndex}");
				return new TypeWindowSettingData ();
			}
		}

		EnumWindowSettingData GetEnumWindowSettingData (int selectIndex)
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
				return new EnumWindowSettingData ();
			}
		}

		private class TypeWindowSettingData
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

		private class EnumWindowSettingData
		{
			public EnumSettingData BindData
			{
				get
				{
					return bindData;
				}
			}
			
			EnumSettingData bindData;
		}
	}
		
}