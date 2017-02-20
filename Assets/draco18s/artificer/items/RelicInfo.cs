using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.items {
	public class RelicInfo {
		public readonly string relicName;
		public readonly String questDescription;
		public readonly int notoriety;

		public RelicInfo(string name, string descip, int notoriety) {
			relicName = name;
			questDescription = descip;
			this.notoriety = notoriety;
		}

		public override string ToString() {
			return relicName + ", " + questDescription + "(" + notoriety + ")";
		}
	}
}
