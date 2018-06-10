using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.statistics;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	public class SpiritUprising : IDeepGoal {
		protected string _name;
		protected string _description;
		protected int numSpirits = 10;
		protected int spiritPower = 10;
		protected int numGuildmastersAtEnd = -1;
		public string description {
			get {
				return _description;
			}
		}

		public string name {
			get {
				return _name;
			}
		}

		public SpiritUprising() {
			_name = "Spirit Uprising";
			_description = "Spirits of the Land are becoming more and more powerful and causing havok.\nIncreased value for all Potion industry types.\nReduced speed for Iron and Gold industry types.";
		}

		public IDeepGoal register() {
			DeepGoalsTypes.register(this);
			return this;
		}

		public bool isActive() {
			if(numSpirits <= 0 && numGuildmastersAtEnd + 1 < StatisticsTracker.guildmastersElected.value) {
				numSpirits = 10;
				spiritPower = 10;
			}

			return numSpirits > 0;
		}

		public float getSpeedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.IRON || industry.industryType == Industries.IndustryTypesEnum.GOLD) {
				return 1 / (0.9f + (spiritPower * numSpirits * 0.002f));
			}
			return 1;
		}

		public float getValuedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.SIMPLE_POTIONS) {
				return 4;
			}
			if(industry.industryType == Industries.IndustryTypesEnum.COMPLEX_POTIONS) {
				return 2;
			}
			return 1;
		}

		public int minQuestDifficulty() {
			return 15;
		}

		public ObstacleType getQuestType() {
			return ChallengeTypes.Goals.DeepGoalSpecial.HUNT_SPIRITS;
		}

		public void modifyQuest(Quest theQuest) {
			theQuest.numQuestsBefore = Math.Min(theQuest.numQuestsBefore, 10);

		}

		public void finalizeQuest(ref Quest theQuest) {
			
		}

		public void onSuccessfulQuest(Quest theQuest) {
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.HUNT_SPIRITS) {
				numSpirits--;
				if(numSpirits <= 0) {
					numGuildmastersAtEnd = StatisticsTracker.guildmastersElected.value;
				}
			}
		}

		public void onFailedQuest(Quest theQuest) {
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.HUNT_SPIRITS) {
				if(theQuest.testLuck(2) == 0)
					numSpirits++;
				else
					spiritPower++;
			}
		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {
			info.AddValue(name + "_numSpirits", numSpirits);
			info.AddValue(name + "_spiritPower", spiritPower);
			info.AddValue(name + "_numGuildmastersAtEnd", numGuildmastersAtEnd);
		}

		public void deserialize(Hashtable info) {
			if(info.ContainsKey(name + "_numSpirits")) {
				numSpirits = (int)info[name + "_numSpirits"];
				spiritPower = (int)info[name + "_spiritPower"];
				if(info.ContainsKey(name + "_numGuildmastersAtEnd")) {
					numGuildmastersAtEnd = (int)info[name + "_numGuildmastersAtEnd"];
				}
			}
		}
	}
}
