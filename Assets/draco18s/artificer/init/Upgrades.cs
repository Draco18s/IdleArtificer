using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.init {
	public static class Upgrades {
		public static class Cash { 
			//public static Upgrade VENDOR_VALUE1 =			new UpgradeVendorEffectiveness(new BigInteger(5, 3), 0.25f);
			public static Upgrade HALVE_DOUBLE_WOOD1 =		new UpgradeHalveDouble(new BigInteger(5, 5), Industries.WOOD, "HALVE_DOUBLE_WOOD1");
			public static Upgrade HALVE_DOUBLE_LEATHER1 =	new UpgradeHalveDouble(new BigInteger(1, 6), Industries.LEATHER, "HALVE_DOUBLE_LEATHER1");
			public static Upgrade HALVE_DOUBLE_TORCHES1 =	new UpgradeHalveDouble(new BigInteger(5, 6), Industries.TORCHES, "HALVE_DOUBLE_TORCHES1");
			public static Upgrade QUEST_RECHARGE1 =			new UpgradeQuestSpeed  (new BigInteger(75, 5), 120, "QUEST_RECHARGE1");
			public static Upgrade TORCHES1 =				new UpgradeIndustryValue(new BigInteger(1, 7), 5, Industries.TORCHES, "TORCHES1");
			public static Upgrade LEATHER_ARMOR1 =			new UpgradeIndustryValue(new BigInteger(1, 8), 5, Industries.ARMOR_LEATHER, "LEATHER_ARMOR1");
			public static Upgrade HEALTH1 =					new UpgradeIndustryValue(new BigInteger(1, 9), 5, Industries.POT_HEALTH, "HEALTH1");
			public static Upgrade MANA1 =					new UpgradeIndustryValue(new BigInteger(1, 9), 5, Industries.POT_MANA, "MANA1");
			public static Upgrade CLICK_RATE1 =				new UpgradeClickRate	(new BigInteger(1, 10), 0.25f, "CLICK_RATE1");
			public static Upgrade HALVE_DOUBLE_GLASS1 =		new UpgradeHalveDouble	(new BigInteger(2, 10), Industries.GLASS, "HALVE_DOUBLE_GLASS1");
			public static Upgrade JOURNEYMAN_RATE1 =		new UpgradeJourneymanRate(new BigInteger(5, 10), 600, "JOURNEYMAN_RATE1");
			public static Upgrade STR1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 5, Industries.POT_STRENGTH, "STR1");
			public static Upgrade AGL1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 5, Industries.POT_AGILITY, "AGL1");
			public static Upgrade INT1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 5, Industries.POT_INTELLIGENCE, "INT1");
			public static Upgrade CHA1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 5, Industries.POT_CHARISMA, "CHA1");
			public static Upgrade ITEM_CASH_VALUE1 =		new UpgradeCashValue	(new BigInteger(5, 12), 0.1f, "ITEM_CASH_VALUE1");
			public static Upgrade HALVE_DOUBLE_IRON_ORE1 =	new UpgradeHalveDouble	(new BigInteger(1, 13), Industries.IRON_ORE, "HALVE_DOUBLE_IRON_ORE1");
			public static Upgrade QUEST_RECHARGE2 =			new UpgradeQuestSpeed   (new BigInteger(1, 14), 120, "QUEST_RECHARGE2");
			public static Upgrade IRON_SWORD1 =				new UpgradeIndustryValue(new BigInteger(1, 16), 5, Industries.IRON_SWORD, "IRON_SWORD1");
			public static Upgrade IRON_RING1 =				new UpgradeIndustryValue(new BigInteger(1, 19), 5, Industries.IRON_RING, "IRON_RING1");
			public static Upgrade HOLY_SYMBOL1 =			new UpgradeIndustryValue(new BigInteger(1, 21), 5, Industries.HOLY_SYMBOL, "HOLY_SYMBOL1");
			public static Upgrade CLICK_RATE2 =				new UpgradeClickRate	(new BigInteger(1, 22), 0.25f, "CLICK_RATE2");
			public static Upgrade QUEST_MULTI1 =			new UpgradeQuestRenown	(new BigInteger(1, 24), 0.5f, "QUEST_MULTI1");
			public static Upgrade RESEARCH1 =				new UpgradeResearchSpeed(new BigInteger(1, 28), 0.05f, "RESEARCH1");
			public static Upgrade POT_WATER_BREATH1 =		new UpgradeIndustryValue(new BigInteger(1, 22), 5, Industries.POT_WATER_BREATH, "POT_WATER_BREATH1");
			public static Upgrade POT_BARKSKIN1 =			new UpgradeIndustryValue(new BigInteger(1, 22), 5, Industries.POT_BARKSKIN, "POT_BARKSKIN1");
			public static Upgrade POT_ALERTNESS1 =			new UpgradeIndustryValue(new BigInteger(1, 22), 5, Industries.POT_ALERTNESS, "POT_ALERTNESS1");
			public static Upgrade POT_REVIVE1 =				new UpgradeIndustryValue(new BigInteger(1, 22), 5, Industries.POT_REVIVE, "POT_REVIVE1");
			public static Upgrade GOLD_RING1 =				new UpgradeIndustryValue(new BigInteger(1, 23), 5, Industries.GOLD_RING, "GOLD_RING1");
			public static Upgrade INCOME1 =					new UpgradeIncome		(new BigInteger(1, 24), 3, "INCOME1");
			//basically, repeat now
			//prices TBD
			/*public static Upgrade TORCHES2 =				new UpgradeIndustryValue(new BigInteger(1, 24), 10, Industries.TORCHES, "TORCHES2");
			public static Upgrade LEATHER_ARMOR2 =			new UpgradeIndustryValue(new BigInteger(1, 25), 10, Industries.ARMOR_LEATHER, "LEATHER_ARMOR2");
			public static Upgrade HEALTH2 =					new UpgradeIndustryValue(new BigInteger(1, 26), 10, Industries.POT_HEALTH, "HEALTH2");
			public static Upgrade MANA2 =					new UpgradeIndustryValue(new BigInteger(1, 27), 10, Industries.POT_MANA, "MANA2");
			public static Upgrade QUEST_MULTI2 =			new UpgradeQuestRenown	(new BigInteger(1, 28), 0.5f, "QUEST_MULTI2");
			public static Upgrade RESEARCH2 =				new UpgradeResearchSpeed(new BigInteger(1, 29), 0.05f, "RESEARCH2");*/

		}
		public static class Renown {
			public static Upgrade CLICK_RATE1 =				new UpgradeClickRate	(10, 0.25f, "CLICK_RATE1");
			public static Upgrade JOURNEYMAN_RATE1 =		new UpgradeJourneymanRate(50, 600, "JOURNEYMAN_RATE1");
			public static Upgrade START_CASH1 =				new UpgradeStartingCash	(100, 10, "START_CASH1");
			public static Upgrade RESEARCH1 =				new UpgradeResearchSpeed(500, 0.5f, "RESEARCH1");
			public static Upgrade JOURNEYMAN_RATE2 =		new UpgradeJourneymanRate(500, 600, "JOURNEYMAN_RATE2");
			public static Upgrade QUEST_MULTI1 =			new UpgradeQuestRenown	(1000, 1.5f, "QUEST_MULTI1");
			public static Upgrade INCOME1 =					new UpgradeIncome		(5000, 5, "INCOME1");
			public static Upgrade ITEM_CASH_VALUE1 =		new UpgradeCashValue	(20000, 0.1f, "ITEM_CASH_VALUE1");
			public static Upgrade RENOWN_MULTI1 =			new UpgradeRenownMulti	(50000, 0.01f, "RENOWN_MULTI1");

			public static Upgrade WOOD_LEVEL1 =				new UpgradeIndustryLevel(100000, 10, Industries.WOOD, "WOOD_LEVEL1");
			public static Upgrade CHARCOAL_LEVEL1 =			new UpgradeIndustryLevel(100000, 10, Industries.CHARCOAL, "CHARCOAL_LEVEL1");
			public static Upgrade RED_BERRIES_LEVEL1 =		new UpgradeIndustryLevel(100000, 10, Industries.RED_BERRIES, "RED_BERRIES_LEVEL1");
			public static Upgrade BLUE_BERRIES_LEVEL1 =		new UpgradeIndustryLevel(100000, 10, Industries.BLUE_BERRIES, "BLUE_BERRIES_LEVEL1");
			public static Upgrade QUEST_MULTI2 =			new UpgradeQuestRenown	(500000, 1.5f, "QUEST_MULTI2");
			public static Upgrade LEATHER_LEVEL1 =			new UpgradeIndustryLevel(1000000, 10, Industries.LEATHER, "LEATHER_LEVEL1");
			public static Upgrade GLASS_LEVEL1 =			new UpgradeIndustryLevel(1000000, 10, Industries.GLASS, "GLASS_LEVEL1");
			public static Upgrade IRON_ORE_LEVEL1 =			new UpgradeIndustryLevel(1000000, 10, Industries.IRON_ORE, "IRON_ORE_LEVEL1");
			public static Upgrade GOLD_ORE_LEVEL1 =			new UpgradeIndustryLevel(1000000, 10, Industries.GOLD_ORE, "GOLD_ORE_LEVEL1");
			public static Upgrade INCOME2 =					new UpgradeIncome		(2000000, 5, "INCOME2");
			public static Upgrade ITEM_CASH_VALUE2 =		new UpgradeCashValue	(5000000, 0.1f, "ITEM_CASH_VALUE2");
			public static Upgrade RENOWN_MULTI2 =			new UpgradeRenownMulti	(10000000, 0.01f, "RENOWN_MULTI2");
			//decrease enchantment costs?
		}
	}
}
