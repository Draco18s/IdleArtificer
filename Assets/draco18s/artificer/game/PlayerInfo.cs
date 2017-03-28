using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using Koopakiller.Numerics;
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
		public BigInteger renown = 0;
		public BigInteger totalRenown = 0;
		public int maxVendors = 5;
		public int currentVendors = 0;
		public int maxApprentices = 0;
		public int currentApprentices = 0;
		public int journeymen = 0;
		public long totalQuestsCompleted = 0;
		public long questsCompleted = 0;
		public int skillPoints = 0;
		public int resetLevel;
		internal float researchTime;
		public Dictionary<UpgradeType, UpgradeValueWrapper> upgrades = new Dictionary<UpgradeType, UpgradeValueWrapper>();

		//achivements
		//upgrades
		//skills

		public PlayerInfo() {
			lifetimeMoney = money = 20;
			builtItems = new List<Industry>();
			resetLevel = 1;
			SetDefaultUpgrades();
		}

		private void SetDefaultUpgrades() {
			upgrades.Add(UpgradeType.CLICK_RATE, new UpgradeFloatValue(0.25f));
			upgrades.Add(UpgradeType.MONEY_INCOME, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.QUEST_DIFFICULTY, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.QUEST_SCALAR, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.QUEST_SPEED, new UpgradeFloatValue(0f));
			upgrades.Add(UpgradeType.RENOWN_INCOME, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.RENOWN_MULTI, new UpgradeFloatValue(0.02f));
			upgrades.Add(UpgradeType.START_CASH, new UpgradeIntValue(20));
			upgrades.Add(UpgradeType.TICK_RATE, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.VENDOR_SELL_VALUE, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.VENDOR_SIZE, new UpgradeIntValue(5));
			upgrades.Add(UpgradeType.RESEARCH_RATE, new UpgradeFloatValue(1f));
			upgrades.Add(UpgradeType.JOURNEYMAN_RATE, new UpgradeFloatValue(0f));
			upgrades.Add(UpgradeType.QUEST_GOODS_VALUE, new UpgradeFloatValue(0f));
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

		public void addItemToInventory(ItemStack stack, NotificationItem notify) {
			addItemToInventory(stack);
			GuiManager.ShowNotification(notify);
		}

		public void addItemToInventory(ItemStack stack) {
			if(stack == null) return;
			stack.wasAddedByJourneyman = false;
			if(Main.instance.player.miscInventory.Contains(stack)) return;
			if(Main.instance.player.unidentifiedRelics.Contains(stack)) {
				if(stack.isIDedByPlayer) {
					Main.instance.player.unidentifiedRelics.Remove(stack);
				}
				else {
					return;
				}
			}
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
		}

		public int GetVendorSize() {
			UpgradeValueWrapper vendorSellQuantity;
			upgrades.TryGetValue(UpgradeType.VENDOR_SIZE, out vendorSellQuantity);
			return ((UpgradeIntValue)vendorSellQuantity).value;
		}

		public float GetVendorValue() {
			int v = StatisticsTracker.moneyMagnitude.value;
			v -= (v % 3);
			v /= 3;
			v = Math.Max(v - 1, 0);
			UpgradeValueWrapper vendorSellEffectiveness;
			upgrades.TryGetValue(UpgradeType.VENDOR_SELL_VALUE, out vendorSellEffectiveness);
			return ((UpgradeFloatValue)vendorSellEffectiveness).value + (0.05f * v);
		}

		public BigRational GetSellMultiplierFull() {
			UpgradeValueWrapper multi;
			upgrades.TryGetValue(UpgradeType.MONEY_INCOME, out multi);
			if(renown > 0) {
				UpgradeValueWrapper wrap;
				upgrades.TryGetValue(UpgradeType.RENOWN_MULTI, out wrap);
				return 1 + (((BigRational)renown + (totalQuestsCompleted - questsCompleted)) * ((UpgradeFloatValue)wrap).value * ((UpgradeFloatValue)multi).value);
			}
			return 1 * ((UpgradeFloatValue)multi).value;
		}

		public BigInteger GetStartingCash() {
			UpgradeValueWrapper wrap;
			upgrades.TryGetValue(UpgradeType.START_CASH, out wrap);
			return ((UpgradeIntValue)wrap).value;
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
				Main.Destroy(ind.craftingGridGO);
				ind.craftingGridGO = null;
			}
			builtItems = new List<Industry>();
			//Debug.Log("digits: " + digits);
			double digitsCur = BigInteger.Log10(money / moneyFloor);
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
			UpgradeValueWrapper questScalar;
			upgrades.TryGetValue(UpgradeType.QUEST_SCALAR, out questScalar);
			int v = Mathf.RoundToInt(goal.getRewardScalar() * ((UpgradeFloatValue)questScalar).value);
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
			info.AddValue("SaveVersion", 6);
			info.AddValue("money", money.ToString());
			info.AddValue("moneyFloor", moneyFloor.ToString());
			info.AddValue("lifetimeMoney", lifetimeMoney.ToString());
			info.AddValue("renown", renown.ToString());
			info.AddValue("totalRenown", totalRenown.ToString());
			info.AddValue("maxVendors", maxVendors);
			info.AddValue("currentVendors", currentVendors);
			info.AddValue("maxApprentices", maxApprentices);
			info.AddValue("journeymen", journeymen);
			info.AddValue("currentApprentices", currentApprentices);
			info.AddValue("totalQuestsCompleted", totalQuestsCompleted);
			info.AddValue("questsCompleted", questsCompleted);
			info.AddValue("skillPoints", skillPoints);
			info.AddValue("resetLevel", resetLevel);

			/*UpgradeValueWrapper wrap;
			upgrades.TryGetValue(UpgradeType.VENDOR_SIZE, out wrap);
			info.AddValue("vendorSellQuantity", ((UpgradeIntValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.VENDOR_SELL_VALUE, out wrap);
			info.AddValue("vendorSellEffectiveness", ((UpgradeFloatValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.QUEST_SCALAR, out wrap);
			info.AddValue("questScalar", ((UpgradeFloatValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.CLICK_RATE, out wrap);
			info.AddValue("clickRate", ((UpgradeFloatValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.START_CASH, out wrap);
			info.AddValue("startCash", ((UpgradeIntValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.RENOWN_MULTI, out wrap);
			info.AddValue("renownMulti", ((UpgradeFloatValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.RESEARCH_RATE, out wrap);
			info.AddValue("researchRate", ((UpgradeFloatValue)wrap).value);
			upgrades.TryGetValue(UpgradeType.QUEST_SPEED, out wrap);
			info.AddValue("newQuestMaxTime", ((UpgradeFloatValue)wrap).value);*/

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
			GuildManager.writeSaveData(ref info, ref context);
		}

		private List<IndustryLoadWrapper> industriesFromDisk = new List<IndustryLoadWrapper>();
		private List<QuestLoadWrapper> activeQuestsFromDisk = new List<QuestLoadWrapper>();
		private List<QuestLoadWrapper> questsFromDisk = new List<QuestLoadWrapper>();

		public PlayerInfo(SerializationInfo info, StreamingContext context) {
			SetDefaultUpgrades();
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
			money = 20;// new BigInteger(info.GetString("money"));
			moneyFloor = 1;// new BigInteger(info.GetString("moneyFloor"));
			lifetimeMoney = money;// new BigInteger(info.GetString("lifetimeMoney"));
			renown = 1000;// new BigInteger(info.GetString("renown"));
			totalRenown = 1000;// new BigInteger(info.GetString("totalRenown"));

			maxVendors = info.GetInt32("maxVendors");
			currentVendors = 0;// info.GetInt32("currentVendors");
			maxApprentices = info.GetInt32("maxApprentices");
			if(Main.saveVersionFromDisk >= 6) {
				journeymen = info.GetInt32("journeymen");
			}
			currentApprentices = 0;// info.GetInt32("currentApprentices");
			totalQuestsCompleted = info.GetInt64("totalQuestsCompleted");
			questsCompleted = info.GetInt64("questsCompleted");
			skillPoints = info.GetInt32("skillPoints");
			resetLevel = info.GetInt32("resetLevel");
			//we don't actually need this
			//UpgradeValueWrapper wrap;
			/*int a;
			float f;
			a = info.GetInt32("vendorSellQuantity");
			upgrades.TryGetValue(UpgradeType.VENDOR_SIZE, out wrap);
			((UpgradeIntValue)wrap).value = a;

			upgrades.TryGetValue(UpgradeType.VENDOR_SELL_VALUE, out wrap);
			f = (float)info.GetDouble("vendorSellEffectiveness");
			((UpgradeFloatValue)wrap).value = f;
			upgrades.TryGetValue(UpgradeType.QUEST_SCALAR, out wrap);
			f = (float)info.GetDouble("questScalar");
			((UpgradeFloatValue)wrap).value = f;
			if(Main.saveVersionFromDisk >= 4) {
				upgrades.TryGetValue(UpgradeType.CLICK_RATE, out wrap);
				f = (float)info.GetDouble("clickRate");
				((UpgradeFloatValue)wrap).value = f;
			}
			if(Main.saveVersionFromDisk >= 5) {
				upgrades.TryGetValue(UpgradeType.START_CASH, out wrap);
				a = info.GetInt32("startCash");
				((UpgradeIntValue)wrap).value = a;
				upgrades.TryGetValue(UpgradeType.RENOWN_MULTI, out wrap);
				f = (float)info.GetDouble("renownMulti");
				((UpgradeFloatValue)wrap).value = f;
				upgrades.TryGetValue(UpgradeType.RESEARCH_RATE, out wrap);
				f = (float)info.GetDouble("researchRate");
				((UpgradeFloatValue)wrap).value = f;
			}*/

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

			//TDOO: placeholder
			//upgrades.TryGetValue(UpgradeType.QUEST_SPEED, out wrap);
			//f = (float)info.GetDouble("newQuestMaxTime");
			//((UpgradeFloatValue)wrap).value = f;
			QuestManager.tickAllQuests(3600);

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
				if(ind.level > 0)
					CraftingManager.BuildIndustry(ind, true, false);
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
			UpgradeValueWrapper clickRate;
			upgrades.TryGetValue(UpgradeType.CLICK_RATE, out clickRate);
			return ((UpgradeFloatValue)clickRate).value;
		}
	}
}
