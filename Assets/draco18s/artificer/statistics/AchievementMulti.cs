using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.statistics {
	public class AchievementMulti : StatAchievement {
		protected readonly IStat stat;
		protected readonly object[] achievementValues;
		public AchievementMulti(string name, IStat linkedStat, object[] values) : base(name) {
			stat = linkedStat;
			achievementValues = values;
		}

		public override void setAchieved() {
			achieved = getNumAchieved() >= achievementValues.Length;
		}

		public override void setUnachieved() {
			stat.resetValue();
			achieved = false;
			dateAchieved = new DateTime(2016, 1, 1);
		}

		public override bool isAchieved() {
			return achieved;
		}

		public int getNumValues() {
			return achievementValues.Length;
		}

		public object getNextValue() {
			for(int i = 0; i < achievementValues.Length; i++) {
				if(!stat.isGreaterThan(achievementValues[i])) {
					return achievementValues[i];
				}
			}
			return achievementValues[0];
		}

		public int getNumAchieved() {
			int count = 0;
			for(int i=0; i< achievementValues.Length; i++) {
				if(stat.isGreaterThan(achievementValues[i])) {
					count++;
				}
			}
			return count;
		}
	}
}
