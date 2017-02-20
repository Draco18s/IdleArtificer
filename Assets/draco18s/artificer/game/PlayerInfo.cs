using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.game {
	[Serializable]
	public class PlayerInfo : ISerializable {
		public readonly Dictionary<GameObject, Industry> itemData = new Dictionary<GameObject, Industry>();
		public readonly List<ItemStack> miscInventory = new List<ItemStack>();
		public readonly List<ItemStack> unidentifiedRelics = new List<ItemStack>();
		public List<Industry> builtItems;
		public BigInteger money = 0;
		public BigInteger moneyFloor = 1;
		public BigInteger lifetimeMoney = 0;// = new BigInteger("1000000000");
		public BigInteger renown = 100;
		public BigInteger totalRenown = 100;
		public int maxVendors = 5;
		public int currentVendors = 0;
		public int maxApprentices = 0;
		public int currentApprentices = 0;
		public long totalQuestsCompleted = 0;
		public long questsCompleted = 0;
		public int skillPoints = 0;
		public int resetLevel;
		private int vendorSellQuantity = 5;
		private float vendorSellEffectiveness = 1.0f;
		private float questScalar = 1;
		private float clickRate = 0.25f;

		//achivements
		//upgrades
		//skills

		public PlayerInfo(BigInteger starting) {
			lifetimeMoney = money = starting;
			builtItems = new List<Industry>();
			resetLevel = 1;
		}

		public void AddMoney(BigInteger val) {
			//TODO: add various multipliers
			//no multipliers should be added here, probably
			money += val;
			lifetimeMoney += val;
			if(money < 0) {
				throw new Exception("Money has gone negative.  What did you do?");
			}
			StatisticsTracker.moneyMagnitude.setValue(money.ToString().Length - 1);
		}

		public void addItemToInventory(ItemStack stack) {
			if(stack == null) return;
			if(stack.relicData == null) {
				foreach(ItemStack s in miscInventory) {
					if(s.item == stack.item) {
						if(s.enchants.Count == 0 && stack.enchants.Count == 0) {
							s.stackSize += stack.stackSize;
							return;
						}
						else {
							bool match = true;
							for(int e = 0; e < s.enchants.Count; e++) {
								if(s.enchants[e] != stack.enchants[e]) {
									match = false;
								}
							}
							if(match) {
								s.stackSize += stack.stackSize;
								return;
							}
						}
					}
				}
			}
			miscInventory.Add(stack);
			miscInventory.Sort((x, y) => y.getDisplayIndex().CompareTo(x.getDisplayIndex()));
			//miscInventory.Reverse();
		}

		public int GetVendorSize() {
			return vendorSellQuantity;
		}

		public float GetVendorValue() {
			int v = StatisticsTracker.moneyMagnitude.value;
			v -= (v % 3);
			v /= 3;
			v = Math.Max(v - 1, 0);
			return vendorSellEffectiveness + (0.05f * v);
		}

		public void IncreaseVendorValue(float amt) {
			vendorSellEffectiveness += amt;
		}

		public BigInteger GetSellMultiplier() {
			if(renown > 0)
				return 1 + ((renown + (totalQuestsCompleted - questsCompleted)) / 50);
			else
				return 1;
		}

		public BigInteger GetStartingCash() {
			return 20;
		}

		public float GetSellMultiplierMicro() {
			BigInteger temp = ((renown + (totalQuestsCompleted - questsCompleted)) / 50);
			temp *= 50;
			temp = (renown + (totalQuestsCompleted - questsCompleted)) - temp;
			//int percenty = BigInteger.convertToInt(temp);
			int percenty = BigInteger.ToInt32(temp);
			return (float)percenty / 50;
		}

		public void reset() {
			foreach(Industry ind in builtItems) {
				ind.quantityStored = 0;
				currentVendors += ind.getRawVendors();
				ind.AdjustVendors(0);
				ind.setTimeRemaining(0);
				ind.level = 0;
				ind.consumeAmount = 0;
				foreach(IndustryInput input in ind.inputs) {
					Main.Destroy(input.arrow);
				}
				Main.Destroy(ind.guiObj);
				ind.guiObj = null;
			}
			builtItems = new List<Industry>();
			//Debug.Log("digits: " + digits);
			double digitsCur = BigInteger.logBigInteger(money / moneyFloor);
			BigInteger spentRenown = totalRenown - renown;
			Debug.Log("before:      " + lifetimeMoney);
			totalRenown = BigInteger.CubeRoot(lifetimeMoney);
			Debug.Log("totalRenown1: " + totalRenown);
			totalRenown /= 10000;
			Debug.Log("totalRenown2: " + totalRenown);
			renown = totalRenown - spentRenown;
			skillPoints += (int)digitsCur;
			moneyFloor *= 2;
			money = GetStartingCash();
			//totalQuestsCompleted;
			questsCompleted = 0;
			StatisticsTracker.minQuestDifficulty.resetValue();
			StatisticsTracker.maxQuestDifficulty.resetValue();
			StatisticsTracker.firstQuestCompleted.setUnachieved();
			if(StatisticsTracker.firstEnchantment.isAchieved()) {
				StatisticsTracker.maxQuestDifficulty.addValue(2);
				StatisticsTracker.minQuestDifficulty.addValue(1);
			}
			if(StatisticsTracker.relicsMade.value >= 1) {
				StatisticsTracker.maxQuestDifficulty.addValue(1);
			}
		}

		public void QuestComplete(ObstacleType goal) {
			int v = Mathf.RoundToInt(goal.getRewardScalar() * questScalar);
			totalQuestsCompleted += v;
			questsCompleted += v;
			goal.numOfTypeCompleted++;
			StatisticsTracker.questsCompleted.addValue(1);
			if(!StatisticsTracker.firstQuestCompleted.isAchieved()) {
				StatisticsTracker.firstQuestCompleted.setAchieved();
				StatisticsTracker.maxQuestDifficulty.addValue(1);
			}
			if(StatisticsTracker.questsCompleted.value >= 20 && !StatisticsTracker.twentiethQuestCompleted.isAchieved()) {
				StatisticsTracker.twentiethQuestCompleted.setAchieved();
				totalQuestsCompleted += 20;
				questsCompleted += 20;
			}
		}

		public float GetQuestDifficultyMultiplier(long complete) {
			return Mathf.Pow(1.25f, complete);
		}


		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("SaveVersion", 4);
			info.AddValue("money", money.ToString());
			info.AddValue("moneyFloor", moneyFloor.ToString());
			info.AddValue("lifetimeMoney", lifetimeMoney.ToString());
			info.AddValue("renown", renown.ToString());
			info.AddValue("totalRenown", totalRenown.ToString());
			info.AddValue("maxVendors", maxVendors);
			info.AddValue("currentVendors", currentVendors);
			info.AddValue("maxApprentices", maxApprentices);
			info.AddValue("currentApprentices", currentApprentices);
			info.AddValue("totalQuestsCompleted", totalQuestsCompleted);
			info.AddValue("questsCompleted", questsCompleted);
			info.AddValue("skillPoints", skillPoints);
			info.AddValue("resetLevel", resetLevel);
			info.AddValue("vendorSellQuantity", vendorSellQuantity);
			info.AddValue("vendorSellEffectiveness", vendorSellEffectiveness);
			info.AddValue("questScalar", questScalar);
			info.AddValue("clickRate", clickRate);

			info.AddValue("miscInventorySize", miscInventory.Count);
			for(int i = 0; i < miscInventory.Count; i++) {
				info.AddValue("miscInventory_" + i, miscInventory[i], typeof(ItemStack));
			}
			info.AddValue("unidentifiedRelicsSize", unidentifiedRelics.Count);
			for(int i = 0; i < unidentifiedRelics.Count; i++) {
				info.AddValue("unidentifiedRelics_" + i, unidentifiedRelics[i], typeof(ItemStack));
			}
			info.AddValue("builtItemsSize", builtItems.Count);
			for(int i = 0; i < builtItems.Count; i++) {
				info.AddValue("buildItemID_" + i, builtItems[i].ID);
				info.AddValue("builtItems_" + i, builtItems[i], typeof(Industry));
			}
			//Debug.Log("Saving " + QuestManager.activeQuests.Count + " active quests");
			info.AddValue("activeQuestsSize", QuestManager.activeQuests.Count);
			for(int i = 0; i < QuestManager.activeQuests.Count; i++) {
				info.AddValue("activeQuests_" + i, QuestManager.activeQuests[i], typeof(Quest));
			}
			//Debug.Log("Saving " + QuestManager.availableQuests.Count + " available quests");
			info.AddValue("availableQuestsSize", QuestManager.availableQuests.Count);
			for(int i = 0; i < QuestManager.availableQuests.Count; i++) {
				info.AddValue("availableQuests_" + i, QuestManager.availableQuests[i], typeof(Quest));
			}
			info.AddValue("availableRelicsSize", QuestManager.availableRelics.Count);
			for(int i = 0; i < QuestManager.availableRelics.Count; i++) {
				info.AddValue("availableRelics_" + i, QuestManager.availableRelics[i], typeof(ItemStack));
			}
			info.AddValue("newQuestTimer", QuestManager.getNewQuestTimer());
			info.AddValue("newQuestMaxTime", QuestManager.getNewQuestMaxTime());
			GuildManager.writeSaveData(ref info, ref context);
		}

		private List<IndustryLoadWrapper> industriesFromDisk = new List<IndustryLoadWrapper>();
		private List<QuestLoadWrapper> activeQuestsFromDisk = new List<QuestLoadWrapper>();
		private List<QuestLoadWrapper> questsFromDisk = new List<QuestLoadWrapper>();

		public PlayerInfo(SerializationInfo info, StreamingContext context) {
#pragma warning disable 0168
			try {
				Main.saveVersionFromDisk = info.GetInt32("SaveVersion");
			}
			catch(Exception ex) {
				Main.saveVersionFromDisk = 0;
			}
#pragma warning restore 0168
			//TODO: uncomment this stuff
			builtItems = new List<Industry>();
			money = new BigInteger(1,16);// new BigInteger(info.GetString("money"));
			moneyFloor = 1;// new BigInteger(info.GetString("moneyFloor"));
			lifetimeMoney = 20000;// new BigInteger(info.GetString("lifetimeMoney"));
			renown = 0;// new BigInteger(info.GetString("renown"));
			totalRenown = 0;// new BigInteger(info.GetString("totalRenown"));

			maxVendors = info.GetInt32("maxVendors");
			currentVendors = info.GetInt32("currentVendors");
			maxApprentices = info.GetInt32("maxApprentices");
			currentApprentices = info.GetInt32("currentApprentices");
			totalQuestsCompleted = info.GetInt64("totalQuestsCompleted");
			questsCompleted = info.GetInt64("questsCompleted");
			skillPoints = info.GetInt32("skillPoints");
			resetLevel = info.GetInt32("resetLevel");
			vendorSellQuantity = info.GetInt32("vendorSellQuantity");
			vendorSellEffectiveness = (float)info.GetDouble("vendorSellEffectiveness");
			questScalar = (float)info.GetDouble("questScalar");
			if(Main.saveVersionFromDisk >= 4)
				clickRate = (float)info.GetDouble("clickRate");

			int num;
			num = info.GetInt32("miscInventorySize");
			miscInventory = new List<ItemStack>();
			for(int o = 0; o < num; o++) {
				//miscInventory.Add((ItemStack)info.GetValue("miscInventory_" + o, typeof(ItemStack)));
			}
			num = info.GetInt32("unidentifiedRelicsSize");
			unidentifiedRelics = new List<ItemStack>();
			for(int o = 0; o < num; o++) {
				//unidentifiedRelics.Add((ItemStack)info.GetValue("unidentifiedRelics_" + o, typeof(ItemStack)));
			}
			num = info.GetInt32("builtItemsSize");
			for(int o = 0; o < num; o++) {
				int id = info.GetInt32("buildItemID_" + o);
				Industry temp = (Industry)info.GetValue("builtItems_" + o, typeof(Industry));
				//industry data from disk isn't actually available yet
				industriesFromDisk.Add(new IndustryLoadWrapper(temp, id));
			}

			num = info.GetInt32("activeQuestsSize");
			Debug.Log("Reading " + num + " active quests");
			for(int o = 0; o < num; o++) {
				//Quest temp = (Quest)info.GetValue("activeQuests_" + o, typeof(Quest));
				//quest obstacle type data from disk isn't actually available yet
				//activeQuestsFromDisk.Add(new QuestLoadWrapper(temp));
			}
			num = info.GetInt32("availableQuestsSize");
			Debug.Log("Reading " + num + " available quests");
			for(int o = 0; o < num; o++) {
				//Quest temp = (Quest)info.GetValue("availableQuests_" + o, typeof(Quest));
				//quest obstacle type data from disk isn't actually available yet
				//questsFromDisk.Add(new QuestLoadWrapper(temp));
			}
			num = info.GetInt32("availableRelicsSize");
			for(int o = 0; o < num; o++) {
				//QuestManager.availableRelics.Add((ItemStack)info.GetValue("availableRelics_" + o, typeof(ItemStack)));
			}
			//QuestManager.LoadTimerFromSave((float)info.GetDouble("newQuestTimer"));
			//QuestManager.LoadMaxTimeFromSave((float)info.GetDouble("newQuestMaxTime"));
			QuestManager.LoadTimerFromSave(-3600);
			QuestManager.LoadMaxTimeFromSave(1200);
			
			if(Main.saveVersionFromDisk >= 2)
				GuildManager.readSaveData(ref info, ref context);
		}

		private class IndustryLoadWrapper {
			public Industry ind;
			public int ID;

			public IndustryLoadWrapper(Industry i, int d) {
				ind = i;
				ID = d;
			}
		}

		private class QuestLoadWrapper {
			public readonly Quest quest;

			public QuestLoadWrapper(Quest q) {
				quest = q;
			}
		}

		public void FinishLoad() {
			if(industriesFromDisk == null) return;
			for(int o = 0; o < industriesFromDisk.Count; o++) {
				Industry ind = GameRegistry.GetIndustryByID(industriesFromDisk[o].ID);
				ind.ReadFromCopy(industriesFromDisk[o].ind);
				CraftingManager.BuildIndustry(ind, true);
			}
			industriesFromDisk.Clear();
			industriesFromDisk = null;
			for(int o = 0; o < questsFromDisk.Count; o++) {
				questsFromDisk[o].quest.FinishLoad();
				QuestManager.availableQuests.Add(questsFromDisk[o].quest);
			}
			questsFromDisk.Clear();
			questsFromDisk = null;
			for(int o = 0; o < activeQuestsFromDisk.Count; o++) {
				activeQuestsFromDisk[o].quest.FinishLoad();
				QuestManager.activeQuests.Add(activeQuestsFromDisk[o].quest);
			}
			activeQuestsFromDisk.Clear();
			activeQuestsFromDisk = null;
		}

		public float getClickRate() {
			return clickRate;
		}

		public void adjustClickRate(float v) {
			clickRate += v;
		}
	}
}
