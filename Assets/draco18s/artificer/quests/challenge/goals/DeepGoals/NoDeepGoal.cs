using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.artificer.init;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	public class NoDeepGoal : IDeepGoal {
		protected string _name;
		protected string _description;
		private Random rand = new Random();

		public string description {
			get {
				return _description;
			}
		}

		public IDeepGoal register() {
			DeepGoalsTypes.register(this);
			return this;
		}

		public string name {
			get {
				return _name;
			}
		}

		public NoDeepGoal() {
			_name = "None";
			_description = "";
		}

		public bool isActive() {
			return false;
		}

		public int minQuestDifficulty() {
			return 0;
		}

		public ObstacleType getQuestType() {
			return ChallengeTypes.Goals.getRandom(rand);
		}

		public void modifyQuest(Quest theQuest) {

		}

		public void finalizeQuest(ref Quest theQuest) {

		}

		public void onSuccessfulQuest(Quest theQuest) {

		}

		public void onFailedQuest(Quest theQuest) {

		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {

		}

		public void deserialize(Hashtable info) {

		}

		public float getSpeedModifier(Industry industry) {
			return 1;
		}

		public float getValuedModifier(Industry industry) {
			return 1;
		}
	}
}
