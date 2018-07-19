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
	public class DragonDispute : IDeepGoal {
		protected string _name;
		protected string _description;

		protected int numDragons = 10;
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

		public DragonDispute() {
			_name = "Dragon Dispute";
			_description = "Dragons are fighting amongst themselves.\nGold production haulted.\nSuccessful quest completion adds Gold production.";
		}

		public string completeDescription {
			get {
				return "The dragons have been slain by heroes, effects will persist until next guildmaster election.\nGold production haulted.\nSuccessful quest completion adds Gold production.";
			}
		}

		public IDeepGoal register() {
			DeepGoalsTypes.register(this);
			return this;
		}

		public bool isActive() {
			if(numGuildmastersAtEnd + 1 < StatisticsTracker.guildmastersElected.value) {
				numDragons = 10;
			}
			int numGold = 0;
			foreach(Industry ind in Main.instance.player.builtItems) {
				if(ind.industryType == Industries.IndustryTypesEnum.GOLD) {
					numGold++;
				}
			}
			if(numDragons > 0 && numGold > 1) return true;
			return false;
		}

		public int minQuestDifficulty() {
			return 24;
		}

		public ObstacleType getQuestType() {
			return ChallengeTypes.Goals.DRAGON;
		}

		public void modifyQuest(Quest theQuest) {
			theQuest.numQuestsBefore = Math.Min(theQuest.numQuestsBefore, 10);

		}

		public void finalizeQuest(ref Quest theQuest) {
			
		}

		public void onSuccessfulQuest(Quest theQuest) {
			foreach(Industry ind in Main.instance.player.builtItems) {
				if(ind.industryType == Industries.IndustryTypesEnum.GOLD) {
					ind.addTimeRaw(50);
				}
			}
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(goal.type == ChallengeTypes.Goals.DRAGON) {
				numDragons--;
				if(numDragons <= 0) {
					numGuildmastersAtEnd = StatisticsTracker.guildmastersElected.value;
				}
			}
		}

		public void onFailedQuest(Quest theQuest) {
			
		}

		public float getSpeedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.GOLD) {
				return 0;
			}
			return 1;
		}

		public float getValuedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.GOLD) {
				return 2;
			}
			return 1;
		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {
			info.AddValue(name + "_numDragons", numDragons);
			info.AddValue(name + "_numGuildmastersAtEnd", numGuildmastersAtEnd);
		}

		public void deserialize(Hashtable info) {
			numDragons = (int)info[name + "_numDragons"];
			numGuildmastersAtEnd = (int)info[name + "_numGuildmastersAtEnd"];
		}
	}
}
