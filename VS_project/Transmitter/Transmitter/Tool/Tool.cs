﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Transmitter.Plugin;

namespace Transmitter.Tool
{
    public static class Tool
    {
        public static string GetFullMessage(this Exception e)
		{

			StackTrace trace = new StackTrace (e, true);

            string message = e.Message;

            StringBuilder stringBuilder = new StringBuilder(message);

            StackFrame[] frames = trace.GetFrames();

            for (int i = 0; i < frames.Length; i++)
            {
                StackFrame frame = frames[i];
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                stringBuilder.Append($"# file -> {fileName} , line -> {lineNumber}");
            }

            return stringBuilder.ToString();
		}

        public static T[] Combine<T>(params T[][] arrs)
        {
            if (arrs.Length == 0)
            {
                CursorModule.Instance.WriteLine("長度為0無法進行合併");

                return new T[0];
            }

            T[] collection = arrs[0];

            for (int i = 1; i < arrs.Length; i++)
            {
                collection = collection.Concat(arrs[i]).ToArray();
            }

            return collection;
        }

        public static bool CheckAdd<T>(this List<T> m_List, T input)
        {
            if (!m_List.Exists(item => item.Equals(input)))
            {
                m_List.Add(input);

                return true;
            }

            return false;
        }

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<KeyValuePair<TKey, TValue>> callback)
        {
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                callback.Invoke(item);
            }
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

        public RefKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
