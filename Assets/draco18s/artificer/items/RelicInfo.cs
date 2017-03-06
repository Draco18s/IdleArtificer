using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.items {
	public class RelicInfo {
		public readonly string heroName;
		public readonly string relicName;
		public readonly String questDescription;
		public readonly int notoriety;

		public RelicInfo(string hero, string name, string descip, int notoriety) {
			heroName = hero;
			relicName = name;
			questDescription = descip;
			this.notoriety = notoriety;
		}

		public override string ToString() {
			return relicName + ", " + questDescription + "(" + notoriety + ")\n   " + "Owned by " + heroName;
		}
	}
}
