
using UnityEngine;

namespace Assets.draco18s.artificer.items {
	public class Scalar {
		public static Scalar _0_RAW =		new Scalar(1.05, new Color(0, 0, 255));
		public static Scalar _1_REFINED =	new Scalar(1.07, new Color(0, 225, 225));
		public static Scalar _2_SIMPLE =	new Scalar(1.09, new Color(0, 255, 0));
		public static Scalar _3_COMPLEX =	new Scalar(1.10, new Color(225, 255, 0));
		public static Scalar _4_RARE =		new Scalar(1.11, new Color(225, 0, 0));

		public double amount;
		public Color color;
		private Scalar(double f, Color c) {
			amount = f;
			color = c;
		}
	}
}