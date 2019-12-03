using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Transmitter.Tool;

namespace Transmitter.Demo
{
	public class DialogBox : MonoBehaviour {

		[SerializeField][ReadOnly]
		Text text;

		RectTransformAdapter rectTransformAdapter;

		/// <summary>
		/// 打字機風格的輸入系統
		/// </summary>
		TypeWriteController typeWriteController;


		const float ChineseWordValue = 4f;

		const float lineHeight = 17.25f;

		float leastAreaTextHeight;

		int currentLineCount;

		//開場的時候抓取text 高 比初始值低就用初始直 超過就用新值
		float GetTextAreaHeight
		{
			get
			{
				float calculationHight = currentLineCount * lineHeight;

				if (calculationHight < leastAreaTextHeight) 
				{
					calculationHight = leastAreaTextHeight;
				}

				return calculationHight;
			}
		}

		/// <summary>
		/// 該字體 英文跟數字等寬 所以判斷中文就好 
		/// </summary>
		const float EnglisgAndNumberWordValue = 2.273f;

		// Use this for initialization
		void Awake () 
		{
			InitPars ();
			InitController ();
		}

		void Update()
		{
			typeWriteController.Refresh ();
		}

		void InitController()
		{
			typeWriteController = new TypeWriteController (OnTextContentModify);
		}

		void InitPars()
		{
			text = this.GetComponent<Text> ();
			RectTransform rectTransform = this.GetComponent<RectTransform> ();
			rectTransformAdapter = new RectTransformAdapter (rectTransform);
			currentLineCount = 0;
			text.text = "";
			leastAreaTextHeight = rectTransformAdapter.Height;
		}

		// <summary>
		// 判斷會不會超過text的畫面自動切成多行
		// </summary>
		// <returns>The to multi linse.</returns>
		List<string> ProcessToMultiLinse(string msg)
		{
			List<string> lines = new List<string> ();

			//超過100就該換行了
			float currentLineValue = 0;

			StringBuilder stringBuilder = new StringBuilder ();

			Array.ForEach (msg.ToCharArray(),(c)=>
				{
					float wordValue = GetWordValue(c);

					currentLineValue+=wordValue;

					if(currentLineValue > 100)
					{
						lines.Add(stringBuilder.ToString());
						//把上一行完結 這個字元作為下一行的開頭
						stringBuilder = new StringBuilder(c);
						currentLineValue = wordValue;
					}
					else
					{
						stringBuilder.Append(c);
					}
				});

			//把剩餘的字數 作為最後一行
			if (stringBuilder.Length > 0) 
			{
				lines.Add (stringBuilder.ToString ());
			}

			return lines;
		}

		float GetWordValue(Char c)
		{
			bool isChinese = CheckIsChinese (c);

			if (isChinese) 
			{
				return ChineseWordValue;
			}
			else
			{
				return EnglisgAndNumberWordValue;
			}
		}

		bool CheckIsChinese(char c)
		{
			if (c >= 0X4e00 && c < 0X9fbb) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Input(string message)
		{
			List<string> lines = ProcessToMultiLinse (message);

			typeWriteController.Input (lines);
		}

		void OnTextContentModify (int lineCount, string newText)
		{
			if (currentLineCount != lineCount) 
			{
				currentLineCount = lineCount;
				rectTransformAdapter.Height = GetTextAreaHeight;
			}
			text.text = newText;
		}
	}	
}