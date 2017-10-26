using System;
using System.Xml;
using System.Xml.Linq;

namespace Assets.draco18s.artificer.statistics {
	public class StatAchievement {
		public readonly string achieveName;
		public readonly string description;
		public string achieveImage;
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

		protected bool achieved;
		protected DateTime dateAchieved = new DateTime(2016, 1, 1);
		protected int idVal = -1;
		protected bool secret = false;
		protected bool _hidden;
		public bool isHidden {
			get {
				if(secret && achieved) return true;
				return _hidden;
			}
		}

		public StatAchievement(string name) {
			achieveName = "achieve." + name + ".name";
			description = "achieve." + name + ".desc";
			achieveImage = "achievements/" + name;
			/*UnityEngine.Object r = UnityEngine.Resources.Load(achieveImage);
			if(r == null) {
				achieveImage = "achievements/Default";
			}*/
		}

		public void determineImage() {
			UnityEngine.Object r = UnityEngine.Resources.Load(achieveImage);
			if(r == null) {
				achieveImage = "achievements/Default";
			}
		}

		public StatAchievement register() {
			StatisticsTracker.register(this);
			return this;
		}

		public virtual StatAchievement setSecret() {
			secret = true;
			_hidden = true;
			return this;
		}

		public virtual StatAchievement setHidden() {
			_hidden = true;
			return this;
		}

		public virtual void setAchieved() {
			achieved = true;
			dateAchieved = DateTime.Today;
		}

		public virtual void setUnachieved() {
			achieved = false;
			dateAchieved = new DateTime(2016, 1, 1);
		}

		public virtual bool isAchieved() {
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