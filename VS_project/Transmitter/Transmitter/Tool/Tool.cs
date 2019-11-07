using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmitter.Tool
{
    public static class Tool
    {
        public static bool CheckAdd<T>(this List<T> m_List, T input)
        {
            if (!m_List.Exists(item => item.Equals(input)))
            {
                m_List.Add(input);

                return true;
            }

            return false;
        }

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> callback)
        {
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                callback.Invoke(item.Key, item.Value);
            }
        }
    }
}
