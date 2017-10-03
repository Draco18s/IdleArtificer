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
		public static readonly StatBase moneyMagnitude = new StatHighscore("moneyMagnitude").setDisplayOrder(0).register();
		public static readonly StatBase vendorsPurchased = new StatHighscore("vendorsPurchased").setDisplayOrder(1).register();
		public static readonly StatBase apprenticesPurchased = new StatHighscore("apprenticesPurchased").setDisplayOrder(2).register();
		public static readonly StatBase journeymenPurchased = new StatHighscore("journeymenPurchased").setDisplayOrder(3).register();
		public static readonly StatBase minQuestDifficulty = new StatResetable("minQuestDifficulty").setInitialValue(0).setDisplayOrder(4).register();
		public static readonly StatBase maxQuestDifficulty = new StatResetable("maxQuestDifficulty").setInitialValue(3).setDisplayOrder(5).register();
		public static readonly StatBase questsCompleted = new StatHighscore("questsCompleted").setDisplayOrder(6).register();
		public static readonly StatBase relicsMade = new StatBase("relicsMade").setDisplayOrder(7).register();
		#endregion
		#region achieve list
		public static readonly StatAchievement unlockedQuesting = new StatAchievement("unlockedQuesting").setDisplayOrder(0).register();
		public static readonly StatAchievement unlockedGuild = new StatAchievement("unlockedGuild").setDisplayOrder(1).register();
		public static readonly StatAchievement unlockedResearch = new StatAchievement("unlockedResearch").setDisplayOrder(2).register();
		public static readonly StatAchievement unlockedEnchanting = new StatAchievement("unlockedEnchanting").setDisplayOrder(3).register();
		public static readonly StatAchievement firstQuestCompleted = new StatAchievement("firstQuestCompleted").setDisplayOrder(4).register();
		public static readonly StatAchievement twentiethQuestCompleted = new StatAchievement("twentiethQuestCompleted").setDisplayOrder(5).register();
		public static readonly StatAchievement allQuestsUnlocked = new StatAchievement("allQuestsUnlocked").setDisplayOrder(6).register();
		public static readonly StatAchievement relicFromGenie = new StatAchievement("relicFromGenie").setDisplayOrder(7).register();
		public static readonly StatAchievement firstEnchantment = new StatAchievement("firstEnchantment").setDisplayOrder(8).register();
		public static readonly StatAchievement firstGuildmaster = new StatAchievement("firstGuildmaster").setDisplayOrder(9).register();
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
			while(allStatistics.Find(x => x.displayOrder == stat.displayOrder) != null) {
				stat.setDisplayOrder(stat.displayOrder + 1);
			}
			allStatistics.Add(stat);
			allStatistics.Sort((a, b) => a.displayOrder.CompareTo(b.displayOrder));
		}

		public static void register(StatAchievement stat) {
			stat.ID = allAchievements.Count;
			while(allAchievements.Find(x => x.displayOrder == stat.displayOrder) != null) {
				stat.setDisplayOrder(stat.displayOrder + 1);
			}
			allAchievements.Add(stat);
			allAchievements.Sort((a, b) => a.displayOrder.CompareTo(b.displayOrder));
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
			SerializationInfoEnumerator infoEnum = info.GetEnumerator();
			Hashtable values = new Hashtable();
			while(infoEnum.MoveNext()) {
				SerializationEntry val = infoEnum.Current;
				values.Add(val.Name, val.Value);
			}
			foreach(StatBase s in allStatistics) {
				if(values.Contains(s.statName)) {
					s.setValue((int)values[s.statName]);
				}
				//s.setValue(info.GetInt32(s.statName));
			}
			foreach(StatAchievement s in allAchievements) {
				if(values.Contains(s.achieveName)) {
					if((bool)values[s.achieveName]) s.setAchieved();
				}
				/*if(info.GetBoolean(s.achieveName)) {
					s.setAchieved();
				}*/
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
