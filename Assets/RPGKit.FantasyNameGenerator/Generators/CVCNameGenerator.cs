using RPGKit.FantasyNameGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGKit.FantasyNameGenerator.Generators {
	public class CVCNameGenerator : NameGenerator {
		public static string[] cUpper = { "B", "C", "D", "F", "G", "H", "K", "L", "M", "N", "P", "R", "S", "T" };
		public static string[] cLower = { "b", "c", "d", "f", "g", "h", "k", "l", "m", "n", "p", "r", "s", "t" };

		public static string[] vLower = { "a", "e", "i", "o", "u" };

		public static string[] endings = { "ith", "ton", "on", "field", "man" };

		public override string GetName() {
			return GetRandomElement(cUpper) + GetRandomElement(vLower) + GetRandomElement(cLower) + GetRandomElement(endings);
		}
	}
}
