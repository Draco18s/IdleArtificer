using System;
using System.Xml;
using System.Xml.Linq;

namespace Assets.draco18s.artificer.statistics {
	public class StatAchievement {
		public readonly string achieveName;
		public readonly string description;
		public readonly string achieveImage;

		public int ID {
			get {
				return idVal;
			}
			set {
				if(idVal == -1) {
					idVal = value;
				}
			}
		}

		private bool achieved;
		private DateTime dateAchieved = new DateTime(2016, 1, 1);
		private int idVal = -1;

		public StatAchievement(string name) {
			achieveName = "achieve." + name + ".name";
			description = "achieve." + name + ".desc";
			achieveImage = "Achievements/" + name;
			UnityEngine.Object r = UnityEngine.Resources.Load(achieveImage);
			if(r == null) {
				achieveImage = "Achievements/Default";
			}
		}

		public StatAchievement register() {
			StatisticsTracker.register(this);
			return this;
		}

		public void setAchieved() {
			achieved = true;
			dateAchieved = DateTime.Today;
		}

		public void setUnachieved() {
			achieved = false;
			dateAchieved = new DateTime(2016, 1, 1);
		}

		public bool isAchieved() {
			return achieved;
		}

		public string getImageString() {
			if(this.achieved) {
				return this.achieveImage;
			}
			return this.achieveImage + "_off";
		}

		public string getAchievedDate() {
			if(this.achieved)
				return this.dateAchieved.ToShortDateString();
			return "";
		}
	}
}