using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.TypeSettingDataFactory.Model
{
	[CreateAssetMenu(menuName ="Transmitter/MessageBlackboard/Create")]
	public class MessageSettingCacheData : ScriptableObject 
	{
		public MessageSettingData MessageSettingData
		{
			get
			{
				return messageSettingData;
			}
		}

		[SerializeField]
		MessageSettingData messageSettingData;
	}
}