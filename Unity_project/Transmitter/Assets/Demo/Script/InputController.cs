using System;
using System.Collections.Generic;
using UnityEngine;
using Transmitter.Tool;
using UnityEngine.UI;

namespace Transmitter.Demo.UI
{
	public class InputController
	{
		/// <summary>
		/// 如果輸入的訊息為空 是否無視
		/// </summary>
		[SerializeField][ReadOnly][Header("如果輸入的訊息為空 是否無視")]
		bool ignoreNull;

		[SerializeField][ReadOnly]
		Button flushBtn;

		[SerializeField][ReadOnly]
		InputField inputField;

		event Action<string> OnTriggerFlushEvent;

		/// <summary>
		/// 如果輸入的訊息為空 是否無視
		/// </summary>
		public InputController (InputField inputField, Button flushBtn, Action<string> OnTriggerFlushCallback, bool ignoreNull = true)
		{
			this.flushBtn = flushBtn;
			this.inputField = inputField;
			this.OnTriggerFlushEvent = OnTriggerFlushCallback;
			flushBtn.onClick.AddListener (OnTriggerFlush);
			this.ignoreNull = ignoreNull;
		}


		void OnTriggerFlush()
		{
			string msg = inputField.text;

			if (!ignoreNull || !string.IsNullOrEmpty (msg)) 
			{
				OnTriggerFlushEvent.Invoke (msg);
			}

			//接收完畢 清空輸入框
			inputField.text = "";
		}

	}
}
