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
			public static Upgrade TORCHES1 =				new UpgradeIndustryValue(new BigInteger(1, 7), 10, Industries.TORCHES, "TORCHES1");
			public static Upgrade LEATHER_ARMOR1 =			new UpgradeIndustryValue(new BigInteger(1, 8), 10, Industries.ARMOR_LEATHER, "LEATHER_ARMOR1");
			public static Upgrade HEALTH1 =					new UpgradeIndustryValue(new BigInteger(1, 9), 10, Industries.POT_HEALTH, "HEALTH1");
			public static Upgrade MANA1 =					new UpgradeIndustryValue(new BigInteger(1, 9), 10, Industries.POT_MANA, "MANA1");
			public static Upgrade CLICK_RATE1 =				new UpgradeClickRate(new BigInteger(1,10), 0.25f, "CLICK_RATE1");
			public static Upgrade STR1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 10, Industries.POT_STRENGTH, "STR1");
			public static Upgrade AGL1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 10, Industries.POT_AGILITY, "AGL1");
			public static Upgrade INT1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 10, Industries.POT_INTELLIGENCE, "INT1");
			public static Upgrade CHA1 =					new UpgradeIndustryValue(new BigInteger(1, 11), 10, Industries.POT_CHARISMA, "CHA1");
			public static Upgrade QUEST_RECHARGE2 =			new UpgradeQuestSpeed   (new BigInteger(1, 14), 120, "QUEST_RECHARGE2");
			public static Upgrade IRON_SWORD1 =				new UpgradeIndustryValue(new BigInteger(1, 16), 10, Industries.IRON_SWORD, "IRON_SWORD1");
			public static Upgrade IRON_RING1 =				new UpgradeIndustryValue(new BigInteger(1, 19), 10, Industries.IRON_RING, "IRON_RING1");
			public static Upgrade HOLY_SYMBOL1 =			new UpgradeIndustryValue(new BigInteger(1, 21), 10, Industries.HOLY_SYMBOL, "HOLY_SYMBOL1");
			//other potions here?
			public static Upgrade TORCHES2 =				new UpgradeIndustryValue(new BigInteger(1, 23), 10, Industries.TORCHES, "TORCHES2");
			public static Upgrade LEATHER_ARMOR2 =			new UpgradeIndustryValue(new BigInteger(1, 24), 10, Industries.ARMOR_LEATHER, "LEATHER_ARMOR2");
			public static Upgrade HEALTH2 =					new UpgradeIndustryValue(new BigInteger(1, 25), 10, Industries.POT_HEALTH, "HEALTH2");
			public static Upgrade MANA2 =					new UpgradeIndustryValue(new BigInteger(1, 25), 10, Industries.POT_MANA, "MANA2");
			//increase relic ID speed

		}
		public static class Renown {
			//upgrade renown value
			//upgrade renown from quests
			//decrease enchantment costs
			//big income multiplier?
			//+X of an industry
			//get some cash for quest items?
			//upgrade click rate
		}
	}
}
