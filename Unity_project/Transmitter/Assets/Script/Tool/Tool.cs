using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Facebook.MiniJSON;

namespace Transmitter.Tool
{
	public static class Tool {

		public static string GetJson(string filePath)
		{
			string projectRootPath = Application.dataPath;

			filePath = projectRootPath +$"/../{filePath}.json";

			string allLines = "";

			using (StreamReader sr = new StreamReader (filePath))
			{
				allLines = sr.ReadToEnd ();
			}

			return allLines;
		}

		public static string SetJsonNode (string originJson, string nodeName, string newValue)
		{
			Dictionary<string,object> jsonDict = (Dictionary<string,object>)Json.Deserialize (originJson);

			if (jsonDict == null) 
			{
				jsonDict = new Dictionary<string, object> ();
			}

			if (!jsonDict.ContainsKey (nodeName)) 
			{
				jsonDict.Add (nodeName, "");
			}

			jsonDict [nodeName] = newValue;

			return Json.Serialize (jsonDict);
		}

		public static void SaveJson(string filePath,string content)
		{
			string projectRootPath = Application.dataPath;

			filePath = projectRootPath +$"/../{filePath}.json";

			using (StreamWriter sw = new StreamWriter (filePath))
			{
				sw.WriteLine (content);
			}
		}

		/// <summary>
		/// 少於幾%之前都為0 將%數以後的 擴大補償
		/// </summary>
		/// <returns>The divide process.</returns>
		/// <param name="progress">Progress.</param>
		/// <param name="divide">Divide.</param>
		public static float GetDivideProcess(float progress,float divide)
		{
			float remaining = progress - divide;

			if (remaining <= 0)
				remaining = 0;

			return remaining / (1 - divide);
		}

		/// <summary>
		/// 把父物件轉為某個角度後 子物件的旋轉為多少
		/// </summary>
		/// <returns>The rot to other reference.</returns>
		/// <param name="localRot">Local rot.</param>
		/// <param name="refRot">Reference rot.</param>
		/// <param name="newRefRot">New reference rot.</param>
		public static Quaternion ProgectionRotToOtherRef(Quaternion oldRot,Quaternion oldRefRot,Quaternion newRefRot)
		{
			Quaternion newRot = Quaternion.identity;

			Quaternion childLocal = Quaternion.Inverse (oldRefRot) * oldRot;
			newRot = newRefRot * childLocal;
			return newRot;
		}

		public static string IndexsDescription(List<int> index)
		{
			StringBuilder sb = new StringBuilder ();

			if (index == null || index.Count == 0) 
			{
				throw new UnityException ("傳入值為空或者長度為0");
			}

			List<int> sorts = new List<int> (index);

			sorts.Sort ((a, b) => a.CompareTo (b));

			int beginPoint = sorts [0];

			sorts.Remove (beginPoint);

			sb.AppendFormat ("first -> {0} \n", beginPoint);

			int endPoint = sorts [sorts.Count - 1];

			sorts.Remove (endPoint);

			sb.AppendFormat ("last -> {0} \n", endPoint);

			List<int> losts = new List<int> ();

			//先把頭尾移掉 因為endPoint本來就是 for -> 小於 所以不用減一
			Range range = new Range (endPoint, beginPoint + 1);

			range.ForEach (r=>
				{
					// sorts的remove不影響結果 只是增加檢索速度
					if(sorts.Exists(sort=>sort==r))
					{
						sorts.Remove(r);
					}
					else
					{
						losts.Add(r);
					}
				});

			if (losts.Count != 0) 
			{
				sb.Append ("losts ->");

				for (int i = 0; i < losts.Count; i++) 
				{
					if (i != 0)
						sb.Append (", ");

					sb.Append (losts[i]);

				}
			}
			else
			{
				sb.Append ("not lost \n");
			}

			return sb.ToString ();
		}

		/// <summary>
		/// 檢查傳入值是否為某root之子物件
		/// </summary>
		/// <returns><c>true</c>, if is child was checked, <c>false</c> otherwise.</returns>
		/// <param name="root">Root.</param>
		/// <param name="target">Target.</param>
		public static bool CheckIsChild(Transform root, Transform target)
		{
			var childs = root.GetComponentsInChildren<Transform> ();

			int id = new List<Transform> (childs).IndexOf(target);

			// -1是找不到 0是root層 不是子物件
			if (id != -1 && id != 0) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 產生 帶顏色的字串
		/// </summary>
		/// <returns>The string color.</returns>
		/// <param name="inputStr">Input string.</param>
		/// <param name="inputColor">Input color.</param>
		public static string MixStringColor(string inputStr, Color inputColor)
		{
			string left = ColorUtility.ToHtmlStringRGB (inputColor).ToLower();

			return string.Format ("<color=#{0}>{1}</color>",left,inputStr);
		}

		public static List<KeyValuePair<T,int>> GetCount<T>(List<T> inputs)
		{
			var result = new List<KeyValuePair<T,int>> ();
			var hestory = new Dictionary<T,int> ();
			inputs.ForEach ((input)=>
				{
					if(hestory.ContainsKey(input))
					{
						hestory[input]++;
					}
					else
					{
						hestory.Add(input,1);
					}

					var count = hestory[input];

					result.Add(new KeyValuePair<T, int>(input,count));
				});

			return result;
		}

		public static List<Transform> GetChilds (this GameObject go)
		{
			List<Transform> result = new List<Transform> (go.GetComponentsInChildren<Transform> ());

			result.Remove (go.transform);

			return result;
		}

		public static void SwitchChildsLayer(this GameObject go,string newLayerName)
		{
			List<Transform> childs = new List<Transform> (go.GetComponentsInChildren<Transform> ());
			childs.ForEach (child => child.gameObject.layer = LayerMask.NameToLayer (newLayerName));
		}

		public static string GetPercentageFormat(float value)
		{
			var str = (value * 100).ToString();
			return str + "%";
		}

		public static string TimeTransferMilliSecond (float time)
		{
			float remain = 0;

			string minute = "";
			string second = "";
			string milliSecond = "";

			if (time >= 60) 
			{
				minute = ((int)time / 60).ToString ("00");
				remain = (time % 60);
			}
			else
			{
				minute = "00";
				remain = time;
			}

			float round = (float)System.Math.Floor (remain);

			second = round.ToString ("00"); 
			milliSecond = System.Math.Floor(((remain - round) * 100)).ToString("00");

			var milliSecondStr = string.Format ("{0}:{1}:{2}", minute, second, milliSecond);

			return milliSecondStr;
		}

		/// <summary>
		/// 生成對應DB格式字串
		/// </summary>
		/// <returns>The DB serialize string.</returns>
		/// <param name="enumerable">Enumerable.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static string ToDBSerializeString<T>(IEnumerable<T> enumerable)
		{
			StringBuilder stringBuilder = new StringBuilder ();
			string result = "";

			stringBuilder.Append ("[");

			try
			{
				bool isSampleType = CheckIsSampleType<T>();

				int i =0;
				foreach (var item in enumerable) 
				{
					if(i!=0)
					{
						stringBuilder.Append (",");
					}

					if(isSampleType)
					{
						stringBuilder.Append (item.ToString());
					}
					else
					{
						stringBuilder.Append (JsonUtility.ToJson(item));
					}

					i++;
				}
					
				stringBuilder.Append("]");
				result = stringBuilder.ToString();

				return result;
			}
			catch (Exception e)
			{
				Debug.LogError ("Current is -> " + result);
				Debug.LogError (e);
				return "[]";
			}
		}
			

		static bool CheckIsSampleType<T>()
		{
			bool result = false;

			result = typeof(T).IsPrimitive || typeof(String).IsAssignableFrom (typeof(T));

			return result;
		}

		/// <summary>
		/// 複製整個資料夾 並遞迴產生路徑
		/// </summary>
		/// <param name="sourceDirPath">Source dir path.</param>
		/// <param name="targetDirPath">Target dir path.</param>
		public static void RecusivelyGenerator(string sourceDirPath,string targetDirPath)
		{
			//分割 index 0應該是系統槽 ex C:\~~~
			var targetPathSpilts = targetDirPath.Split('\\');
			var sourcePathSpilts = sourceDirPath.Split('\\');

			if (!Directory.Exists (sourceDirPath)) 
			{
				throw new Exception ("指定來源不存在");
			}

			if (!Directory.Exists (targetPathSpilts[0])) 
			{
				throw new Exception ("指定路徑的系統槽不存在 -> " + targetDirPath [0].ToString ());
			}

			//清空舊路徑資料夾與檔案
			List<string> oldFiles = new List<string> (Directory.GetFiles (targetDirPath));
			
			oldFiles.ForEach (file => File.Delete (file));
			
			List<string> oldDir = new List<string> (Directory.GetDirectories (targetDirPath));
			
			oldDir.ForEach (dir => Directory.Delete (dir));

			//來源是　C:\AA\BB 就要在目標的後面在接一個 BB
			var newFolderName = sourcePathSpilts [sourcePathSpilts.Length - 1];

			var targetDirFullPath = targetDirPath + "\\" + newFolderName;
			
			if (!Directory.Exists (targetDirFullPath)) 
			{
				Directory.CreateDirectory (targetDirFullPath);
			}

			List<string> sourceFiles = new List<string> (
				Directory.GetFiles (sourceDirPath, "*.*", SearchOption.AllDirectories));


			Func<string,string,string> ProcessStart = (origan,cut) => 
			{
				return origan.Remove(0,cut.Length);
			};

			List<string> relativelySourceFiles = new List<string> ();


			//全部處理成 相對位置
			sourceFiles.ForEach ((sourceFile)=>
				{
					relativelySourceFiles.Add(ProcessStart(sourceFile,sourceDirPath));
				});
					
			List<string> processeds = new List<string> ();

			int countFile = relativelySourceFiles.Count;
			int currentFile = 0;

			relativelySourceFiles.ForEach ((relativelySourceFile)=>
				{
					//本來是0 所以一進入就要++
					currentFile++;

					var recusivelyData = RecusivelyList(relativelySourceFile);

					recusivelyData.dirPaths.ForEach((dirPath)=>
						{
							if(!processeds.Exists(processed=>processed==dirPath))
							{
								if(!Directory.Exists(dirPath))
								{
									var fullDirPath = targetDirFullPath+"\\"+dirPath;
									Directory.CreateDirectory(fullDirPath);
									processeds.Add(dirPath);
								}
							}
						});

					var fullSourcePath = sourceDirPath+"\\"+recusivelyData.filePath;
					var fullFilePath = targetDirFullPath+"\\"+recusivelyData.filePath;

					#if UNITY_EDITOR

					Action<float> displayAction = (progress)=>
					{
						string fileEnqueueInfo = string.Format("當前檔案數量 ->  總檔案數量\n " +
							"{0} -> {1}\n 當前進度 -> {2}",currentFile,countFile,progress);
						UnityEditor.EditorUtility.DisplayProgressBar(progress.ToString(),fileEnqueueInfo,progress);
					};

					CustomCopyFile(fullSourcePath,fullFilePath,displayAction);

					#endif
				});

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.ClearProgressBar();
			#endif

		}

        public static List<T> RecusivelyGetComponents<T>(this Transform m_transform)
        {
            List<Transform> allTransforms = new List<Transform>() { m_transform };

            allTransforms.AddRange(GetTransforms(m_transform));

            List<T> results = new List<T>();

            allTransforms.ForEach(allTransform =>
               results.AddRange(allTransform.GetComponents<T>()));

            return results;
        }

		public static List<Component> RecusivelyGetComponents (this Transform m_transform, Type type)
		{
			List<Transform> allTransforms = new List<Transform>() { m_transform };

			allTransforms.AddRange(GetTransforms(m_transform));

			List<Component> results = new List<Component>();

			allTransforms.ForEach (allTransform =>
				results.AddRange (allTransform.GetComponents (type)));

			return results;
		}

        static List<Transform> GetTransforms(Transform target)
        {
            List<Transform> collecter = new List<Transform>();

            //把IEnumerator解開
            foreach (Transform child in target)
            {
                collecter.Add(child);
                collecter.AddRange(GetTransforms(child));
            }

            return collecter;
        }

        static void CustomCopyFile(string sourceDirPath, string targetDirPath,Action<float> displayAction)
		{
			byte[] buffer = new byte[1024 * 1024];

			FileStream source = null;
			FileStream target = null;

			try
			{
				source = new FileStream(sourceDirPath,FileMode.Open);
				long fileLength = source.Length;

				target = new FileStream(targetDirPath,FileMode.Create);

				long totalBytes = 0;
				int currentBlockSize = 0;

				while((currentBlockSize = source.Read(buffer,0,buffer.Length))>0)
				{
					totalBytes+=currentBlockSize;

					float percentage = (float)totalBytes*100.0f/fileLength;

					target.Write(buffer,0,currentBlockSize);
					displayAction.Invoke(percentage);
				}

			}
			catch(Exception e)
			{
				Debug.LogError (e.Message);
			}

			if (source != null)
				source.Dispose ();
			
			if (target != null)
				target.Dispose ();
		}

		static RecusivelyData RecusivelyList(string relativelyPath)
		{
			var spilts = relativelyPath.Split('\\');

			RecusivelyData recusivelyData = new RecusivelyData ();

			recusivelyData.dirPaths = new List<string> ();

			string current = "";

			for (int i = 0; i < spilts.Length; i++) 
			{
				if (i != 0) 
				{
					current += "\\";
				}

				current += spilts [i];

				if (i != spilts.Length - 1) 
				{
					recusivelyData.dirPaths.Add (current);
				}
				else 
				{
					recusivelyData.filePath = current;
				}
			}

			return recusivelyData;
		}

		class RecusivelyData
		{
			public List<string> dirPaths = new List<string>();
			public string filePath;
		}

		/// <summary>
		/// 產生smooth事件
		/// </summary>
		/// <returns>The smooth.</returns>
		/// <param name="progressAction">Progress action.</param>
		/// <param name="smoothTime">Smooth time.</param>
		public static IEnumerator<float> SampleSmooth(Action<float> progressAction,float smoothTime)
		{
			float beginTime = Time.time;

			float finishTime = beginTime + smoothTime;
			float remingTime = 0;

			while((remingTime = (finishTime-Time.time))>0)
			{
				var progress = (1-(remingTime / smoothTime));

				progressAction (progress);
				yield return 0;
			}

			progressAction (1);
			yield return 0;
		}

		/// <summary>
		/// 傳入 List 隨機 抽出指定數量
		/// </summary>
		/// <returns>The pick.</returns>
		/// <param name="input">Input.</param>
		/// <param name="pickCount">Pick count.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> RandomPick<T>(List<T> input,int pickCount)
		{
			List<T> result = new List<T> ();

			//斷開連接
			List<T> _input = new List<T> (input);

			for (int i = 0; i < pickCount; i++) 
			{
				int randomIndex = UnityEngine.Random.Range (0, _input.Count);
				result.Add (_input [randomIndex]);
				_input.RemoveAt (randomIndex);
			}

			return result;
		}

		public static List<T>RandomSort<T>(List<T> input)
		{
			List<T> result = new List<T> ();
			//隨機抽一個key 然後用key排序
			List<KeyValuePair<float,T>> table = new List<KeyValuePair<float, T>> ();
			input.ForEach ((i)=>
				{
					table.Add(new KeyValuePair<float, T>(UnityEngine.Random.Range(0f,999f),i));
				});

			table.Sort ((a,b)=>
				{
					return -a.Key.CompareTo(b.Key);
				});

			table.ForEach (item => result.Add (item.Value));

			return result;
		}

		public static string RemoveEnd(this string oldStr,string remove)
		{
			if (oldStr.EndsWith (remove)) 
			{
				string processStr = oldStr;
				processStr = processStr.Substring (0, processStr.LastIndexOf (remove));
				return processStr;
			}
			else
			{
				throw new Exception ("並非以此作為結尾");
			}
		}

		public static string GetFullMessage(this Exception e)
		{
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace (e, true);
			return string.Format ("{0} # line -> {1}",
				e.Message,
				trace.GetFrame (0).GetFileLineNumber ());
		}

        public static T DeepClone<T>(T source)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, source);
            memoryStream.Flush();
            memoryStream.Position = 0;
			object result = binaryFormatter.Deserialize (memoryStream);
			memoryStream.Dispose ();
			return (T)result;
        }



        public static List<T> ToList<T>(this T[] srcArray)
        {
            List<T> result = new List<T>(srcArray);

            return result;
        }
		
		public static bool HasSameValue<T>(IList<T> a,IList<T> b,bool ignoreOrder=false) where T:class
		{
			bool result = true;
			
			if (ignoreOrder) 
			{
				List<T> cloneA = new List<T> (a);
				List<T> cloneB = new List<T> (b);

				for (int i = 0; i < cloneA.Count; i++) 
				{
					int findIndex = cloneB.FindIndex (_b => _b == cloneA [i]);

					if (findIndex != -1) 
					{
						cloneB.RemoveAt (findIndex);
					}
					else
					{
						result = false;
						break;
					}
				}
			}
			else
			{
				for (int i = 0; i < a.Count; i++) 
				{
					if (!(a [i]==b [i]))
					{
						result = false;
						break;
					}
				}
			}

			return result;
		}

		public static bool TryPop<T> (this List<T> origin, Predicate<T> queryPredicate, ref T finder)
		{
			int? findIndex = null;
			
			for (int i = 0; i < origin.Count; i++) 
			{
				if (queryPredicate.Invoke (origin [i])) 
				{
					findIndex = i;
					break;
				}
			}	

			if (findIndex != null) 
			{
				finder = origin [findIndex.Value];

				origin.RemoveAt (findIndex.Value);
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool Remove<T> (this List<T> origin, Predicate<T> queryPredicate)
		{
			int? findIndex = null;

			for (int i = 0; i < origin.Count; i++) 
			{
				if (queryPredicate.Invoke (origin [i])) 
				{
					findIndex = i;
					break;
				}
			}	

			if (findIndex != null) 
			{
				origin.RemoveAt (findIndex.Value);
				return true;
			}
			else
			{
				return false;
			}
		}

		public static List<RefKeyValuePair<TKey, TValue>> ToPairList<TKey, TValue>(this Dictionary<TKey, TValue> source)
		{
			List<RefKeyValuePair<TKey, TValue>> outResult = new List<RefKeyValuePair<TKey, TValue>> ();

			foreach (var item in source) 
			{
				RefKeyValuePair<TKey, TValue> processItem = new RefKeyValuePair<TKey, TValue> (item.Key, item.Value);
				
				outResult.Add (processItem);
			}

			return outResult;
		}
    }

	/// <summary>
	/// 本來的keyValuePair是 valueType 簡而言之就是refenceType
	/// </summary>
	[Serializable]
	public class RefKeyValuePair<TKey, TValue>
	{
		public TKey key;
		public TValue value;

		public RefKeyValuePair(TKey key,TValue value)
		{
			this.key = key;
			this.value = value;
		}
	}

	[Serializable]
	public class Range : IEnumerable<int>
	{
		List<int> values;

		public List<int> Values
		{
			get
			{
				return values;
			}
		}
		
		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return values.GetEnumerator ();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator ();
		}
		
		public Range(int endPoint,int beginPoint=0)
		{
			values = new List<int>();
			
			for (int i = beginPoint; i < endPoint; i++) 
			{
				values.Add(i);
			}
		}

		public void ForEach(Action<int> trigger)
		{
			values.ForEach (trigger);
		}

		public int Count
		{
			get
			{
				return values.Count;
			}
		}
		
		public int this[int key]
		{
			get
			{
				return values [key];
			}
		}
	}
}
