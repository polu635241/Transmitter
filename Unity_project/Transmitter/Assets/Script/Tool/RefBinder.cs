using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.Tool
{
	public class RefBinder : MonoBehaviour {

		[Header("key Value pair table")]
		[SerializeField]
		List<RefGather> refGathers = new List<RefGather>();

		public GameObject GetGameobject (string queyKey, bool throwException = true)
		{
			GameObject findTarget = null;
			
			RefGather finder = refGathers.Find (gather => gather.key == queyKey);

			if (finder == null&&throwException) 
			{
				throw new Exception ("綁定不存在 可是綁定內容為空");
			}
			else
			{
				if (finder.go == null&&throwException)
				{
					throw new Exception ("綁定存在 可是綁定內容為空");
				}
				else
				{
					findTarget = finder.go;
				}
			}

			return findTarget;
		}

		public T GetChildComponent<T> (string queyKey, bool throwException = true) where T:Component
		{
			GameObject childGO = GetGameobject (queyKey);
			return childGO.GetComponent<T> ();
		}
	}

	[Serializable]
	class RefGather
	{
		public string key;
		public GameObject go;
	}
}