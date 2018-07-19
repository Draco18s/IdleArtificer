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
		public static void CashUpgradeInit() {
			//public static Upgrade VENDOR_VALUE1 =				new UpgradeVendorEffectiveness(new BigInteger(5, 3), 0.25f);
			/*Upgrade VENDOR_SIZE1 =				new UpgradeVendorSize(new BigInteger(1, 7), 2, "VENDOR_SIZE1").register(AllCashUps);
			Upgrade QUEST_RECHARGE1 =			new UpgradeQuestSpeed   (new BigInteger(1, 7), 120, "QUEST_RECHARGE1").register(AllCashUps);
			Upgrade CLICK_RATE1 =				new UpgradeClickRate	(new BigInteger(2, 9), 0.1f, "CLICK_RATE1").register(AllCashUps);
			Upgrade RESEARCH1 =					new UpgradeResearchSpeed(new BigInteger(25, 9), 0.25f, "RESEARCH1").register(AllCashUps);
			Upgrade HEROSTAM1 =					new UpgradeHeroStamina	(new BigInteger(25, 10), 120, "HEROSTAM1").register(AllCashUps);
			Upgrade ITEM_CASH_VALUE1 =			new UpgradeCashValue	(new BigInteger(1, 13), 0.1f, "ITEM_CASH_VALUE1").register(AllCashUps);
			Upgrade QUEST_RECHARGE2 =			new UpgradeQuestSpeed   (new BigInteger(1, 14), 120, "QUEST_RECHARGE2").register(AllCashUps);
			Upgrade CLICK_RATE2 =				new UpgradeClickRate	(new BigInteger(1, 22), 0.1f, "CLICK_RATE2").register(AllCashUps);
			Upgrade QUEST_MULTI1 =				new UpgradeQuestRenown	(new BigInteger(1, 24), 0.5f, "QUEST_MULTI1").register(AllCashUps);
			Upgrade RESEARCH2 =					new UpgradeResearchSpeed(new BigInteger(1, 25), 0.15f, "RESEARCH2").register(AllCashUps);
			Upgrade INCOME1 =					new UpgradeIncome		(new BigInteger(1, 28), 3, "INCOME1").register(AllCashUps);
			*/
			//half-double
			List<Industry> basicTypes = new List<Industry>();
			List<Industry> endTypes = new List<Industry>();
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

			Upgrade CLICK_RATE1 =			new UpgradeClickRate	(10, 0.2f, "CLICK_RATE1").register(AllRenownUps);
			Upgrade JOURNEYMAN_RATE1 =		new UpgradeJourneymanRate(25, 600, "JOURNEYMAN_RATE1").register(AllRenownUps);
			Upgrade LOOTING1 =				new UpgradeTheLoots     (50, 0.5f, "LOOTING1").register(AllRenownUps);
			Upgrade RESEARCH1 =				new UpgradeResearchSpeed(75, 0.25f, "RESEARCH1").register(AllRenownUps);
			//Upgrade START_CASH1 =			new UpgradeStartingCash	(90, 10, "START_CASH1").register(AllRenownUps);
			Upgrade ITEM_CASH_VALUE1 =		new UpgradeCashValue	(100, 0.1f, "ITEM_CASH_VALUE1").register(AllRenownUps);
			Upgrade QUEST_MULTI1 =			new UpgradeQuestRenown	(250, 1.5f, "QUEST_MULTI1").register(AllRenownUps);
			Upgrade INCOME1 =				new UpgradeIncome		(500, 2, "INCOME1").register(AllRenownUps);
			Upgrade RENOWN_MULTI1 =			new UpgradeRenownMulti	(750, 0.01f, "RENOWN_MULTI1").register(AllRenownUps);
			Upgrade START_CASH2 =			new UpgradeStartingCash	(1000, 50, "START_CASH2").register(AllRenownUps);
			Upgrade JOURNEYMAN_RATE2 =		new UpgradeJourneymanRate(2500, 600, "JOURNEYMAN_RATE2").register(AllRenownUps);
			Upgrade LOOTING2 =				new UpgradeTheLoots     (5000, 0.5f, "LOOTING2").register(AllRenownUps);
			Upgrade RESEARCH2 =				new UpgradeResearchSpeed(7500, 0.25f, "RESEARCH2").register(AllRenownUps);
			Upgrade ITEM_CASH_VALUE2 =		new UpgradeCashValue	(10000, 0.1f, "ITEM_CASH_VALUE2").register(AllRenownUps);
			Upgrade QUEST_MULTI2 =			new UpgradeQuestRenown	(25000, 1.5f, "QUEST_MULTI2").register(AllRenownUps);
			Upgrade INCOME2 =				new UpgradeIncome		(50000, 5, "INCOME2").register(AllRenownUps);
			Upgrade RENOWN_MULTI2 =			new UpgradeRenownMulti	(75000, 0.01f, "RENOWN_MULTI2").register(AllRenownUps);
			Upgrade QUEST_REQS =			new UpgradeQuestReqs    (100000, "QUEST_REQS").register(AllRenownUps);
			//repeat:
			Upgrade CLICK_RATE2 =			new UpgradeClickRate	(100000, 0.2f, "CLICK_RATE2").register(AllRenownUps);
			Upgrade JOURNEYMAN_RATE3 =		new UpgradeJourneymanRate(250000, 600, "JOURNEYMAN_RATE3").register(AllRenownUps);
			Upgrade LOOTING3 =				new UpgradeTheLoots     (500000, 0.5f, "LOOTING3").register(AllRenownUps);
			Upgrade RESEARCH3 =				new UpgradeResearchSpeed(750000, 0.25f, "RESEARCH3").register(AllRenownUps);
			Upgrade ITEM_CASH_VALUE3 =		new UpgradeCashValue	(1000000, 0.1f, "ITEM_CASH_VALUE3").register(AllRenownUps);
			Upgrade QUEST_MULTI3 =			new UpgradeQuestRenown	(2500000, 1.5f, "QUEST_MULTI3").register(AllRenownUps);
			Upgrade INCOME3 =				new UpgradeIncome		(5000000, 2, "INCOME3").register(AllRenownUps);
			Upgrade RENOWN_MULTI3 =			new UpgradeRenownMulti	(7500000, 0.01f, "RENOWN_MULTI3").register(AllRenownUps);
			Upgrade START_CASH3 =			new UpgradeStartingCash	(10000000, 100, "START_CASH4").register(AllRenownUps);
			Upgrade JOURNEYMAN_RATE4 =		new UpgradeJourneymanRate(25000000, 600, "JOURNEYMAN_RATE4").register(AllRenownUps);
			Upgrade LOOTING4 =				new UpgradeTheLoots     (50000000, 0.5f, "LOOTING4").register(AllRenownUps);
			Upgrade RESEARCH4 =				new UpgradeResearchSpeed(75000000, 0.25f, "RESEARCH4").register(AllRenownUps);
			Upgrade ITEM_CASH_VALUE4 =		new UpgradeCashValue	(100000000, 0.2f, "ITEM_CASH_VALUE4").register(AllRenownUps);
			Upgrade QUEST_MULTI4 =			new UpgradeQuestRenown	(250000000, 1.5f, "QUEST_MULTI4").register(AllRenownUps);
			Upgrade INCOME4 =				new UpgradeIncome		(500000000, 5, "INCOME4").register(AllRenownUps);
			Upgrade RENOWN_MULTI4 =			new UpgradeRenownMulti	(750000000, 0.02f, "RENOWN_MULTI4").register(AllRenownUps);
			
			Upgrade CLICK_RATE3 =			new UpgradeClickRate	 (new BigInteger(1,   9), 0.2f, "CLICK_RATE3").register(AllRenownUps);
			Upgrade JOURNEYMAN_RATE5 =		new UpgradeJourneymanRate(new BigInteger(25,  8), 600, "JOURNEYMAN_RATE5").register(AllRenownUps);
			Upgrade LOOTING5 =				new UpgradeTheLoots      (new BigInteger(5,   9), 0.5f, "LOOTING5").register(AllRenownUps);
			Upgrade RESEARCH5 =				new UpgradeResearchSpeed (new BigInteger(75,  8), 0.25f, "RESEARCH5").register(AllRenownUps);
			Upgrade ITEM_CASH_VALUE5 =		new UpgradeCashValue	 (new BigInteger(1,  10), 0.1f, "ITEM_CASH_VALUE5").register(AllRenownUps);
			Upgrade QUEST_MULTI5 =			new UpgradeQuestRenown	 (new BigInteger(25,  9), 1.5f, "QUEST_MULTI5").register(AllRenownUps);
			Upgrade INCOME5 =				new UpgradeIncome		 (new BigInteger(5,  10), 2, "INCOME5").register(AllRenownUps);
			Upgrade RENOWN_MULTI5 =			new UpgradeRenownMulti	 (new BigInteger(75,  9), 0.01f, "RENOWN_MULTI5").register(AllRenownUps);
			Upgrade START_CASH5 =			new UpgradeStartingCash	 (new BigInteger(1,  11), 250, "START_CASH6").register(AllRenownUps);
			Upgrade JOURNEYMAN_RATE6 =		new UpgradeJourneymanRate(new BigInteger(25, 10), 600, "JOURNEYMAN_RATE6").register(AllRenownUps);
			Upgrade LOOTING6 =				new UpgradeTheLoots      (new BigInteger(5,  11), 0.5f, "LOOTING6").register(AllRenownUps);
			Upgrade RESEARCH6 =				new UpgradeResearchSpeed (new BigInteger(75, 10), 0.5f, "RESEARCH6").register(AllRenownUps);
			Upgrade ITEM_CASH_VALUE6 =		new UpgradeCashValue	 (new BigInteger(1,  12), 0.2f, "ITEM_CASH_VALUE6").register(AllRenownUps);
			Upgrade QUEST_MULTI6 =			new UpgradeQuestRenown	 (new BigInteger(25, 11), 1.5f, "QUEST_MULTI6").register(AllRenownUps);
			Upgrade INCOME6 =				new UpgradeIncome		 (new BigInteger(5,  12), 5, "INCOME6").register(AllRenownUps);
			Upgrade RENOWN_MULTI6 =			new UpgradeRenownMulti	 (new BigInteger(75, 11), 0.02f, "RENOWN_MULTI6").register(AllRenownUps);
			//decrease enchantment costs?
		}
		#region hidden
		//public static Upgrade WOOD_LEVEL1 =				new UpgradeIndustryLevel(10000, 10, Industries.WOOD, "WOOD_LEVEL1");
		//public static Upgrade CHARCOAL_LEVEL1 =			new UpgradeIndustryLevel(25000, 10, Industries.CHARCOAL, "CHARCOAL_LEVEL1");
		//public static Upgrade RED_BERRIES_LEVEL1 =		new UpgradeIndustryLevel(50000, 10, Industries.RED_BERRIES, "RED_BERRIES_LEVEL1");
		//public static Upgrade BLUE_BERRIES_LEVEL1 =		new UpgradeIndustryLevel(50000, 10, Industries.BLUE_BERRIES, "BLUE_BERRIES_LEVEL1");
		//public static Upgrade LEATHER_LEVEL1 =			new UpgradeIndustryLevel(250000, 10, Industries.LEATHER, "LEATHER_LEVEL1");
		//public static Upgrade GLASS_LEVEL1 =				new UpgradeIndustryLevel(500000, 10, Industries.GLASS, "GLASS_LEVEL1");
		//public static Upgrade IRON_ORE_LEVEL1 =			new UpgradeIndustryLevel(750000, 10, Industries.IRON_ORE, "IRON_ORE_LEVEL1");
		//public static Upgrade GOLD_ORE_LEVEL1 =			new UpgradeIndustryLevel(100000, 10, Industries.GOLD_ORE, "GOLD_ORE_LEVEL1");
		#endregion

		//temporary boosty things
		//[ ] NEEDS SUPPORTING ARCHITECTURE
		//[ ] clicking gives 5 seconds increased income
		//[ ] clicking makes that industry faster for 5 seconds
		//[ ] clicking produces goods

	}
}
