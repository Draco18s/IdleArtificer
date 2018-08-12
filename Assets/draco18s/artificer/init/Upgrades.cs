using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics;
using Assets.draco18s.artificer.items;
using System.Reflection;

namespace Assets.draco18s.artificer.init {
	public static class Upgrades {
		public static List<Upgrade> AllCashUps = new List<Upgrade>();
		public static List<Upgrade> AllRenownUps = new List<Upgrade>();
		public static List<Industry> basicTypes = new List<Industry>();
		public static List<Industry> endTypes = new List<Industry>();
		public static void CashUpgradeInit() {
			//public static new UpgradeVendorEffectiveness(new BigInteger(5, 3), 0.25f);
			/*new UpgradeVendorSize(new BigInteger(1, 7), 2, "VENDOR_SIZE1").register(AllCashUps);
			new UpgradeQuestSpeed   (new BigInteger(1, 7), 120, "QUEST_RECHARGE1").register(AllCashUps);
			new UpgradeClickRate	(new BigInteger(2, 9), 0.1f, "CLICK_RATE1").register(AllCashUps);
			new UpgradeResearchSpeed(new BigInteger(25, 9), 0.25f, "RESEARCH1").register(AllCashUps);
			new UpgradeHeroStamina	(new BigInteger(25, 10), 120, "HEROSTAM1").register(AllCashUps);
			new UpgradeCashValue	(new BigInteger(1, 13), 0.1f, "ITEM_CASH_VALUE1").register(AllCashUps);
			new UpgradeQuestSpeed   (new BigInteger(1, 14), 120, "QUEST_RECHARGE2").register(AllCashUps);
			new UpgradeClickRate	(new BigInteger(1, 22), 0.1f, "CLICK_RATE2").register(AllCashUps);
			new UpgradeQuestRenown	(new BigInteger(1, 24), 0.5f, "QUEST_MULTI1").register(AllCashUps);
			new UpgradeResearchSpeed(new BigInteger(1, 25), 0.15f, "RESEARCH2").register(AllCashUps);
			new UpgradeIncome		(new BigInteger(1, 28), 3, "INCOME1").register(AllCashUps);
			*/
			//half-double
			Dictionary<Industry, bool> hasOutputs = new Dictionary<Industry, bool>();
			FieldInfo[] fields = typeof(Industries).GetFields();
			foreach(FieldInfo field in fields) {
				Industry item = (Industry)field.GetValue(null);
				if(!hasOutputs.ContainsKey(item)) hasOutputs.Add(item, false);
				if(item.inputs.Count == 0) {
					basicTypes.Add(item);
				}
				else {
					foreach(IndustryInput ins in item.inputs) {
						if(hasOutputs.ContainsKey(ins.item)) {
							hasOutputs.Remove(ins.item);
						}
						hasOutputs.Add(ins.item, true);
					}
				}
			}
			foreach(KeyValuePair<Industry,bool> pair in hasOutputs.AsEnumerable()) {
				if(!pair.Value || pair.Key.industryType == Industries.IndustryTypesEnum.SIMPLE_POTIONS || pair.Key.industryType == Industries.IndustryTypesEnum.COMPLEX_POTIONS) {
					endTypes.Add(pair.Key);
				}
			}
			for(int i = 1; i <= 5; i++) {
				BigInteger upped = BigInteger.Pow(new BigInteger(10, 25), i - 1);
				foreach(Industry ind in basicTypes) {
					BigInteger cost = ind.cost * 1000 * upped;
					new UpgradeHalveDouble(cost, ind, "HALVE_DOUBLE_" + ind.saveName.ToUpper()+i).register(AllCashUps);
				}
				new UpgradeJourneymanRate(new BigInteger(5, 10) * upped, 600, "JOURNEYMAN_RATE"+i).register(AllCashUps);
				new UpgradeVendorSize(new BigInteger(1, 6) * upped, 2, "VENDOR_SIZE" + i).register(AllCashUps);
				new UpgradeQuestSpeed(new BigInteger(1, 7) * upped, 120, "QUEST_RECHARGE" + (i * 2 - 1)).register(AllCashUps);
				new UpgradeClickRate(new BigInteger(2, 9) * upped, 0.1f, "CLICK_RATE" + (i * 2 - 1)).register(AllCashUps);
				new UpgradeResearchSpeed(new BigInteger(25, 9) * upped, 0.25f, "RESEARCH" + (i * 2 - 1)).register(AllCashUps);
				new UpgradeHeroStamina(new BigInteger(25, 10) * upped, 120, "HEROSTAM"+i).register(AllCashUps);
				new UpgradeCashValue(new BigInteger(1, 13) * upped, 0.1f, "ITEM_CASH_VALUE"+i).register(AllCashUps);
				new UpgradeQuestSpeed(new BigInteger(1, 18) * upped, 120, "QUEST_RECHARGE" + (i * 2)).register(AllCashUps);
				new UpgradeClickRate(new BigInteger(1, 22) * upped, 0.1f, "CLICK_RATE" + (i * 2)).register(AllCashUps);
				new UpgradeQuestRenown(new BigInteger(1, 24) * upped, 0.5f, "QUEST_MULTI"+i).register(AllCashUps);
				new UpgradeResearchSpeed(new BigInteger(1, 25) * upped, 0.15f, "RESEARCH" + (i * 2)).register(AllCashUps);
				new UpgradeIncome(new BigInteger(1, 28) * upped, 3, "INCOME"+i).register(AllCashUps);
			}
			//value
			for(int i = 1; i <= 5; i++) {
				BigInteger upped = BigInteger.Pow(new BigInteger(10, 27), i - 1);
				foreach(Industry ind in endTypes) {
					BigInteger cost = ind.cost * 1000 * upped;
					new UpgradeIndustryValue(cost, 5, ind, ind.saveName.ToUpper() + i).register(AllCashUps);
				}
			}
			AllCashUps.Sort((x, y) => x.cost.CompareTo(y.cost));
		}
		public static void RenownUpgradeInit() {
			
			for(int i = 1; i <= 5; i++) {
				BigInteger upped = BigInteger.Pow(new BigInteger(10, 4), i - 1);
				new UpgradeClickRate	(10 * upped, 0.2f, "CLICK_RATE" + i).register(AllRenownUps);
				if(i < 4)
					new UpgradeJourneymanRate(25 * upped, 600, "JOURNEYMAN_RATE" + (i * 2 - 1)).register(AllRenownUps);
				new UpgradeTheLoots     (50 * upped, 0.5f, "LOOTING" + (i * 2 - 1)).register(AllRenownUps);
				if(i < 4)
					new UpgradeResearchSpeed(75 * upped, 0.25f, "RESEARCH" + (i * 2 - 1)).register(AllRenownUps);
				if(i > 1)
					new UpgradeStartingCash	(90 * upped, 10, "START_CASH" + (i * 2 - 1)).register(AllRenownUps);
				new UpgradeCashValue	(100 * upped, 0.1f, "ITEM_CASH_VALUE" + (i * 2 - 1)).register(AllRenownUps);
				new UpgradeQuestRenown	(250 * upped, 1.5f, "QUEST_MULTI" + (i * 2 - 1)).register(AllRenownUps);
				new UpgradeIncome		(500 * upped, 2, "INCOME" + (i * 2 - 1)).register(AllRenownUps);
				new UpgradeRenownMulti	(750 * upped, 0.01f, "RENOWN_MULTI" + (i * 2 - 1)).register(AllRenownUps);
				new UpgradeStartingCash	(1000 * upped, 50, "START_CASH" + (i * 2)).register(AllRenownUps);
				if(i < 4)
					new UpgradeJourneymanRate(2500 * upped, 600, "JOURNEYMAN_RATE" + (i * 2)).register(AllRenownUps);
				new UpgradeTheLoots     (5000 * upped, 0.5f, "LOOTING" + +(i * 2)).register(AllRenownUps);
				if(i < 4)
					new UpgradeResearchSpeed(7500 * upped, 0.25f, "RESEARCH" + (i * 2)).register(AllRenownUps);
				new UpgradeCashValue	(10000 * upped, 0.1f, "ITEM_CASH_VALUE" + (i * 2)).register(AllRenownUps);
				new UpgradeQuestRenown	(25000 * upped, 1.5f, "QUEST_MULTI" + (i * 2)).register(AllRenownUps);
				new UpgradeIncome		(50000 * upped, 5, "INCOME" + (i * 2)).register(AllRenownUps);
				new UpgradeRenownMulti	(75000 * upped, 0.01f, "RENOWN_MULTI" + (i * 2)).register(AllRenownUps);
				if(i == 1)
					new UpgradeQuestReqs    (100000 * upped, "QUEST_REQS").register(AllRenownUps);
				if (i > 1 && i < 4)
					foreach(Industry ind in basicTypes) {
						BigInteger cost = ind.cost * 5 * upped;
						new UpgradeIndustryLevel(cost, 50, ind, ind.saveName.ToUpper() + "_LEVEL" + i).register(AllRenownUps);
					}
			}
			AllRenownUps.Sort((x, y) => x.cost.CompareTo(y.cost));
			//decrease enchantment costs?
		}

		//temporary boosty things
		//[ ] NEEDS SUPPORTING ARCHITECTURE
		//[ ] clicking gives 5 seconds increased income
		//[ ] clicking makes that industry faster for 5 seconds
		//[ ] clicking produces goods

	}
}
