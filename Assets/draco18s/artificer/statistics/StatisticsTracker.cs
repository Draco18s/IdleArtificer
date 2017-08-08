using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Assets.draco18s.artificer.statistics {
	public static class StatisticsTracker {
		private static List<StatBase> allStatistics = new List<StatBase>();
		private static List<StatAchievement> allAchievements = new List<StatAchievement>();

		#region stat list
		public static readonly StatBase minQuestDifficulty = new StatResetable("minQuestDifficulty").setInitialValue(0).register();
		public static readonly StatBase maxQuestDifficulty = new StatResetable("maxQuestDifficulty").setInitialValue(3).register();
		public static readonly StatBase moneyMagnitude = new StatHighscore("moneyMagnitude").register();
		public static readonly StatBase questsCompleted = new StatHighscore("questsCompleted").register();
		public static readonly StatBase vendorsPurchased = new StatHighscore("vendorsPurchased").register();
		public static readonly StatBase relicsMade = new StatBase("relicsMade").register();
		#endregion
		#region achieve list
		public static readonly StatAchievement firstEnchantment = new StatAchievement("firstEnchantment").register();
		public static readonly StatAchievement firstQuestCompleted = new StatAchievement("firstQuestCompleted").register();
		public static readonly StatAchievement twentiethQuestCompleted = new StatAchievement("twentiethQuestCompleted").register();
		public static readonly StatAchievement allQuestsUnlocked = new StatAchievement("allQuestsUnlocked").register();
		public static readonly StatAchievement relicFromGenie = new StatAchievement("relicFromGenie").register();
		public static readonly StatAchievement unlockedEnchanting = new StatAchievement("unlockedEnchanting").register();
		public static readonly StatAchievement unlockedQuesting = new StatAchievement("unlockedQuesting").register();
		public static readonly StatAchievement unlockedGuild = new StatAchievement("unlockedGuild").register();
		public static readonly StatAchievement unlockedResearch = new StatAchievement("unlockedResearch").register();
		#endregion
		//unlocked all quests
		//built all buildings - have ever built x
		//hired vendors
		//cash magnitude
		//craft all enchantments - have ever crafted x
		//relic notoriety
		//relic antiquity
		//find/identify a relic
		//resource value given to quests
		//days played


		public static void register(StatBase stat) {
			stat.ID = allStatistics.Count;
			allStatistics.Add(stat);
		}

		public static void register(StatAchievement stat) {
			stat.ID = allAchievements.Count;
			allAchievements.Add(stat);
		}

		public static void newLevelReset() {
			foreach(StatBase stat in allStatistics) {
				if(stat.shouldResetOnNewLevel) {
					stat.resetValue();
				}
			}
		}

		public static void serializeAllStats(ref SerializationInfo info, ref StreamingContext context) {
			foreach(StatBase s in allStatistics) {
				info.AddValue(s.statName, s.value);
			}
			foreach(StatAchievement s in allAchievements) {
				info.AddValue(s.achieveName, s.isAchieved());
			}
		}

		public static void deserializeAllStats(ref SerializationInfo info, ref StreamingContext context) {
			foreach(StatBase s in allStatistics) {
				s.setValue(info.GetInt32(s.statName));
			}
			foreach(StatAchievement s in allAchievements) {
				if(info.GetBoolean(s.achieveName)) {
					s.setAchieved();
				}
			}
		}

		public static IEnumerator<StatAchievement> getAchievementsList() {
			return allAchievements.GetEnumerator();
		}

		public static IEnumerator<StatBase> getStatsList() {
			return allStatistics.GetEnumerator();
		}
	}
}
