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
			text = this.GetComponent<Text> ();
			InitPars ();
		}

		void InitPars()
		{
			inOutput = false;
			currentWordIndex = 0;
			currentLineIndex = 0;
			waitOutputLines = new List<string>();
		}

		public void Input(string message)
		{
			waitOutputLines.Add (message);
			CheckWakeProcess ();
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
	}	
}