using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.statistics;
using UnityEngine;
using Koopakiller.Numerics;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.upgrades;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	class Captialists : IDeepGoal {
		protected string _name;
		protected string _description;
		protected int numGuildmastersAtEnd = -1;
		protected BigInteger cashAtStart = 0;
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

		public Captialists() {
			_name = "Captialist Adventurers";
			_description = "Capitalism takes a hold of the market! Earn a bunch of money!\nAll industires gain a bonus to speed for every 100 levels.\nAll industries gain a bonus to sell value at 250 levels.";
		}

		public string completeDescription {
			get {
				return "Capitalism thrives! Effects last until a new guildmaster is selected.\nAll industires gain a bonus to speed for every 100 levels.\nAll industries gain a bonus to sell value at 250 levels.";
			}
		}

		public IDeepGoal register() {
			DeepGoalsTypes.register(this);
			return this;
		}

		public bool isActive() {
			if(numGuildmastersAtEnd + 3 < StatisticsTracker.guildmastersElected.value) {
				cashAtStart = 0;
				return false;
			}
			if(cashAtStart == 0) {
				cashAtStart = StatisticsTracker.lifetimeMoney.value;
			}
			else if(StatisticsTracker.lifetimeMoney.value >= (cashAtStart * 125) / 100) {
				numGuildmastersAtEnd = StatisticsTracker.guildmastersElected.value;
				return false;
			}
			return true;
		}

		public int minQuestDifficulty() {
			return 5;
		}

		public ObstacleType getQuestType() {
			return ChallengeTypes.Goals.DeepGoalSpecial.CAPITALISM;
		}

		public void modifyQuest(Quest theQuest) {
		}

		public void finalizeQuest(ref Quest theQuest) {
		}

		public void onSuccessfulQuest(Quest theQuest) {
			ObstacleType goal = theQuest.getGoal();
			if(goal == ChallengeTypes.Goals.DeepGoalSpecial.CAPITALISM) {
				ItemStack newRelic = theQuest.determineRelic().clone();
				if(newRelic != null) {
					newRelic.antiquity = Math.Max(newRelic.antiquity, StatisticsTracker.guildmastersElected.value / 5);
					newRelic = QuestManager.makeRelic(newRelic, goal, "Capitalism!");
					UpgradeValueWrapper wrap;
					Main.instance.player.upgrades.TryGetValue(UpgradeType.QUEST_GOODS_VALUE, out wrap);
					float v = Mathf.Max(((UpgradeFloatValue)wrap).value, 0.1f);
					BigInteger val = BigRational.ToBigInt(ItemStack.GetRelicValue(newRelic) / 2 * v);
					Main.instance.player.AddMoney(val);
				}
			}
		}

		public void onFailedQuest(Quest theQuest) {
		}

		public float getSpeedModifier(Industry industry) {
			if(industry.level * industry.getHalveAndDouble() >= 100) {
				return 1<<Mathf.FloorToInt(industry.level * industry.getHalveAndDouble() / 100);
			}
			return 1;
		}

		public float getValuedModifier(Industry industry) {
			if(industry.level * industry.getHalveAndDouble() >= 250) {
				return 2;
			}
			return 1;
		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {
			info.AddValue(name + "_numGuildmastersAtEnd", numGuildmastersAtEnd);
		}

		public void deserialize(Hashtable info) {
			if(info.Contains(name + "_numGuildmastersAtEnd"))
				numGuildmastersAtEnd = (int)info[name + "_numGuildmastersAtEnd"];
		}
	}
}
