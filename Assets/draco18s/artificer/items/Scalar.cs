
using UnityEngine;

namespace Assets.draco18s.artificer.items {
	public class Scalar {
		public static Scalar _0_RAW =		new Scalar(1.05f, new Color(0, 0, 255));
		public static Scalar _1_REFINED =	new Scalar(1.07f, new Color(0, 225, 225));
		public static Scalar _2_SIMPLE =	new Scalar(1.09f, new Color(0, 255, 0));
		public static Scalar _3_COMPLEX =	new Scalar(1.10f, new Color(225, 255, 0));
		public static Scalar _4_RARE =		new Scalar(1.11f, new Color(225, 0, 0));

		public float amount;
		public Color color;
		private Scalar(float f, Color c) {
			amount = f;
			color = c;
		}
	}
}