using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmitter.Tool
{
    public static class Tool
    {
        public static T[] Combine<T>(T[] arr1, T[] arr2)
        {
            return arr1.Concat(arr2).ToArray();
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
