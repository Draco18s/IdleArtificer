
using UnityEngine;

namespace Assets.draco18s.artificer.items {
	public class IndustryInput {
		public readonly Industry item;
		public readonly int quantity;
		public GameObject arrow;
		public IndustryInput(Industry i, int count) {
			item = i;
			quantity = count;
		}
	}

	public class IndustryOutput {
		public readonly Industry item;
		public readonly int quantity;
		public IndustryOutput(Industry i, int count) {
			item = i;
			quantity = count;
		}
	}
}