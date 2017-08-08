using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.items {
	[Serializable]
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
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("heroName", heroName);
			info.AddValue("relicName", relicName);
			info.AddValue("questDescription", questDescription);
			info.AddValue("notoriety", notoriety);
		}
		public RelicInfo(SerializationInfo info, StreamingContext context) {
			heroName = info.GetString("heroName");
			relicName = info.GetString("relicName");
			questDescription = info.GetString("questDescription");
			notoriety = info.GetInt32("notoriety");
		}
	}
}
