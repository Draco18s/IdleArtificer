using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.util {
	public static class TypeExtensions {
		public static bool IsArrayOf<T>(this Type type) {
			return type == typeof(T[]);
		}

		public static IEnumerable<string> ChunksUpto(this string str, int maxChunkSize) {
			for(int i = 0; i < str.Length; i += maxChunkSize)
				yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
		}
	}
}
