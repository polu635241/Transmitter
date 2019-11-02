using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.Net
{
	static class ProxyTable<Tkey,TValue> 
	{
		static Dictionary<Tkey,TValue> table = new Dictionary<Tkey,TValue> ();
		
		public static void Add (Tkey key, TValue value)
		{
			table.Add (key, value);
		}
		
		public static bool TryGetValue (Tkey key, out TValue value)
		{
			return table.TryGetValue (key, out value);
		}
		
		public static void CleanUp() 
		{
			table.Clear ();
		}
	}
}