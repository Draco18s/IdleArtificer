using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.game;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	public class CursedWood : IDeepGoal {
		protected string _name;
		protected string _description;

		protected int HeartTrees;
		protected int ForestSize;
		protected int Corruption;
		protected bool Rescued;
		protected int Agents;
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

		public CursedWood() {
			_name = "Cursed Woods";
			_description = "A malevolent dark forest is gathering power.\nReduced speed for Wood and Plant industry types.\nIncreased value for Wood and Plant industry types";
			HeartTrees = 5;
			ForestSize = 5;
			Corruption = 5;
			Agents = 0;
			Rescued = false;
		}

		public IDeepGoal register() {
			DeepGoalsTypes.register(this);
			return this;
		}

		public bool isActive() {

			if(HeartTrees <= 0 && numGuildmastersAtEnd + 1 < StatisticsTracker.guildmastersElected.value) {
				HeartTrees = 5;
				ForestSize = 5;
				Corruption = 5;
				Agents = 0;
				Rescued = false;
			}

			if(HeartTrees <= 0) return false;
			return true;
		}

		public int minQuestDifficulty() {
			return 10;
		}

		public ObstacleType getQuestType() {
			UnityEngine.Debug.Log("Agents: " + Agents + ", Trees: " + HeartTrees + ", Corruption: " + Corruption + ", Forest: " + ForestSize);
			if(Agents >= 5) {
				Agents = 0;
				return ChallengeTypes.Goals.DeepGoalSpecial.SOW_CHAOS;
			}
			bool haveEnoughCorruption = HeartTrees == Corruption;
			bool haveEnoughForest = Corruption*2 == ForestSize;
			if(ForestSize > Corruption * 2) {
				return ChallengeTypes.Goals.DeepGoalSpecial.BURN_FOREST;
			}
			if(haveEnoughCorruption && haveEnoughForest) {
				if(!Rescued)
					return ChallengeTypes.Goals.DeepGoalSpecial.RESCUE_TAKEN;
				return ChallengeTypes.Goals.DeepGoalSpecial.BURN_FOREST;
			}
			if(haveEnoughCorruption && !haveEnoughForest)
				return ChallengeTypes.Goals.DeepGoalSpecial.CLEANS_CORRUPTION;
			if(!haveEnoughCorruption && !haveEnoughForest)
				return ChallengeTypes.Goals.DeepGoalSpecial.HEART_TREE;

			return ChallengeTypes.Goals.DeepGoalSpecial.BURN_FOREST;
		}

		public void modifyQuest(Quest theQuest) {

		}

		public void finalizeQuest(ref Quest theQuest) {
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			theQuest.miscData = new Dictionary<string, object>();
			if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.SOW_CHAOS) {
				theQuest.miscData.Add("forest_corruption", 10);
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.HEART_TREE ||
					goal.type == ChallengeTypes.Goals.DeepGoalSpecial.BURN_FOREST ||
					goal.type == ChallengeTypes.Goals.DeepGoalSpecial.RESCUE_TAKEN ||
					goal.type == ChallengeTypes.Goals.DeepGoalSpecial.CLEANS_CORRUPTION) {
				theQuest.miscData.Add("forest_corruption", 1);
			}
			else
				return;
			UnityEngine.Debug.Log("Finalizing quest: " + theQuest.miscData == null);
		}

		public void onSuccessfulQuest(Quest theQuest) {
			Rescued = false;
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.SOW_CHAOS) {
				return;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.HEART_TREE) {
				HeartTrees--;
				if(HeartTrees <= 0) {
					numGuildmastersAtEnd = StatisticsTracker.guildmastersElected.value;
				}
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.BURN_FOREST) {
				ForestSize -= 1;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.RESCUE_TAKEN) {
				Rescued = true;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.CLEANS_CORRUPTION) {
				Corruption -= 1;
			}
			else {
				return; //not one of ours
			}
			if(theQuest.miscData == null) {
				UnityEngine.Debug.Log("No misc data: " + goal.type.name);
				return;
			}
		}

		public void onFailedQuest(Quest theQuest) {
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.SOW_CHAOS) {
				bool haveEnoughCorruption = HeartTrees == Corruption;
				bool haveEnoughForest = Corruption * 2 == ForestSize;
				if(haveEnoughCorruption && haveEnoughForest)
					HeartTrees++;
				else if(haveEnoughCorruption && !haveEnoughForest)
					ForestSize++;
				else
					Corruption++;
				return;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.HEART_TREE) {
				Corruption += 1;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.BURN_FOREST) {
				if(Corruption * 2 < ForestSize)
					Corruption += 1;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.RESCUE_TAKEN) {
				HeartTrees++;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.CLEANS_CORRUPTION) {
				ForestSize += 1;
			}
			else {
				return; //not one of ours
			}
			if(theQuest.miscData == null) {
				UnityEngine.Debug.Log("No misc data: " + goal.type.name);
				return;
			}
			object corwrap;
			int cor = 0;
			if(theQuest.miscData.TryGetValue("forest_corruption", out corwrap)) {
				cor = (int)corwrap;
				theQuest.miscData.Remove("forest_corruption");
			}
			if(cor >= 6) {
				Agents++;
			}
		}

		public float getSpeedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.PLANT || industry.industryType == Industries.IndustryTypesEnum.WOOD) {
				return 0.2f;
			}
			return 1;
		}

		public float getValuedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.PLANT || industry.industryType == Industries.IndustryTypesEnum.WOOD) {
				return 100;
			}
			return 1;
		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {
			info.AddValue(name + "_HeartTrees", HeartTrees);
			info.AddValue(name + "_ForestSize", ForestSize);
			info.AddValue(name + "_Corruption", Corruption);
			info.AddValue(name + "_Rescued", Rescued);
			info.AddValue(name + "_Agents", Agents);
			info.AddValue(name + "_numGuildmastersAtEnd", numGuildmastersAtEnd);
		}

		public void deserialize(Hashtable info) {
			HeartTrees = (int)info[name + "_HeartTrees"];
			ForestSize = (int)info[name + "_ForestSize"];
			Corruption = (int)info[name + "_Corruption"];
			if(info.ContainsKey(name + "_Rescued")) {
				Rescued = (bool)info[name + "_Rescued"];
				Agents = (int)info[name + "_Agents"];
			}
			if(info.ContainsKey(name + "_numGuildmastersAtEnd")) {
				numGuildmastersAtEnd = (int)info[name + "_numGuildmastersAtEnd"];
			}
		}
	}
}
