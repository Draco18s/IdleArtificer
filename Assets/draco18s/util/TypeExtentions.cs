using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.util {
	public static class TypeExtensions {
		public static bool IsArrayOf<T>(this Type type) {
			return type == typeof(T[]);
		}
	}
}
