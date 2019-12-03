using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Transmitter.Demo
{
	public class DialogBox : MonoBehaviour {

		[SerializeField][ReadOnly]
		Text text;

		[SerializeField][ReadOnly]
		string currentOutputLine;

		[SerializeField][ReadOnly]
		int currentWordIndex;

		[SerializeField][ReadOnly]
		int currentLineIndex;

		[SerializeField][ReadOnly]
		float currentProgress;

		[SerializeField][ReadOnly]
		bool firstWord;

		[SerializeField][ReadOnly]
		List<string> waitOutputLines = new List<string>();

		[SerializeField][ReadOnly]
		List<string> keepLines = new List<string>();

		[SerializeField][ReadOnly]
		bool inOutput;

		const float SettingWriteSpeed = 1;

		const float ChineseWordValue = 4f;

		/// <summary>
		/// 該字體 英文跟數字等寬 所以判斷中文就好 
		/// </summary>
		const float EnglisgAndNumberWordValue = 2.273f;

		float GetWriteSpeed
		{
			get
			{
				//還剩幾行要輸出 行數越多寫得越快
				int remaingLinesCount = waitOutputLines.Count + 1;
				float statusSpeed = 1;

				//第一個字出來快一點
				if (firstWord) 
				{
					statusSpeed = 3;
				}
				return SettingWriteSpeed * remaingLinesCount * statusSpeed;
			}
		}

		// Use this for initialization
		void Awake () 
		{
			InitPars ();
		}

		void InitPars()
		{
			text = this.GetComponent<Text> ();
			text.text = "";
			inOutput = false;
			currentWordIndex = 0;
			currentLineIndex = 0;
			waitOutputLines = new List<string>();
		}

		void InternalInput(List<string> messages)
		{
			waitOutputLines.AddRange (messages);
			CheckWakeProcess ();
		}

		public void Input(string message)
		{
			List<string> lines = ProcessToMultiLinse (message);
			
			InternalInput (lines);
		}

		/// <summary>
		/// 判斷是否是從 休眠狀態被喚醒 並做處理
		/// </summary>
		void CheckWakeProcess()
		{
			if (!inOutput) 
			{
				inOutput = true;

				PrepareNewLine ();
			}
		}

		// Update is called once per frame
		void Update () 
		{
			if (inOutput) 
			{
				currentProgress += Time.deltaTime * GetWriteSpeed;

				if (currentProgress >= 1) 
				{
					//最後一個字了
					if (currentWordIndex == currentOutputLine.Length - 1) 
					{
						if (waitOutputLines.Count > 0) 
						{
							PrepareNewLine ();
						}
						else
						{
							inOutput = false;
						}
					}
					else
					{
						string newWord = PopNewWord (firstWord);
						text.text += newWord;

						if (firstWord) 
						{
							firstWord = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// 是否目前輸入處在
		/// </summary>
		/// <value><c>true</c> if from origin point; otherwise, <c>false</c>.</value>
		bool FromOriginPoint
		{
			get
			{
				return currentLineIndex == 0 && currentWordIndex == 0;
			}
		}


		void PrepareNewLine()
		{
			StringBuilder stringBuilder = new StringBuilder (text.text);

			//開場時是從起點開始 其他情況是從上一行的結尾開始
			if (!FromOriginPoint)
			{
				stringBuilder.Append ("\r\n");
				currentLineIndex++;
			}

			currentOutputLine = PopNewLine ();
			keepLines.Add (currentOutputLine);
			currentWordIndex = 0;

			currentProgress = 0;
			currentWordIndex = 0;

			text.text = stringBuilder.ToString ();

			firstWord = true;
		}

		string PopNewLine()
		{
			string result = waitOutputLines [0];
			waitOutputLines.RemoveAt (0);
			return result;
		}

		//首字從0開始 之後每個都要往後移動一個index
		string PopNewWord(bool firstWord)
		{
			if (!firstWord) 
			{
				currentWordIndex++;
			}

			char[] allWords = currentOutputLine.ToCharArray ();

			return  allWords [currentWordIndex].ToString ();
		}

		public void GetDescription()
		{
			string[] linesArray = this.GetComponent<Text> ().text.Split ("\r\n".ToCharArray ());
			List<string> lines = new List<string> (linesArray);

			lines.ForEach (line=>
				{
					print(line.Length);
					print(line);
				});
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
	}	
}