using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.requirement {
	[Flags]
	public enum RequirementType : long { //key: q=a quest uses it, i=an item has it, e=an enchantment supplies it.
										 //     anything in [brackets] is planned or under consideration
		FIRE_IMMUNE =			(1L << 0), //qe
		COLD_IMMUNE =			(1L << 1), //qe
		POISON_IMMUNE =			(1L << 2), //qe
		ACID_IMMUNE =			(1L << 3), //qie
		HOLY_IMMUNE =			(1L << 4), //qie
		UNHOLY_IMMUNE =			(1L << 5), //qie
		FIRE_DAMAGE =			(1L << 6), //qe
		COLD_DAMAGE =			(1L << 7), //qe
		POISON_DAMAGE =			(1L << 8), //qie
		ACID_DAMAGE =			(1L << 9), //qie
		HOLY_DAMAGE =			(1L << 10), //qi[e]
		UNHOLY_DAMAGE =			(1L << 11), //q[e]
		STEALTH =				(1L << 12), //qie
		LIGHT =					(1L << 13), //qie
		WATER_BREATH =			(1L << 14), //qi
		FEATHER_FALL =			(1L << 15), //qe
		DANGER_SENSE =			(1L << 16), //qe
		SPELL_RESIST =			(1L << 17), //qe
		ETHEREALNESS =			(1L << 18), //qe
		MIRRORED =				(1L << 19), //qe //gaze attacks
		ENDURANCE =				(1L << 20), //qe
		DISRUPTION =			(1L << 21), //qe
		VORPAL =				(1L << 22), //qe
		BRILIANT_ENERGY =		(1L << 23), //e //may remove
		DISPELLING =			(1L << 24), //qe
		FREE_MOVEMENT =			(1L << 25), //qe
		PHASE_LOCK =			(1L << 26),     //may remove?
		DETECTION =				(1L << 27), //qe
		CLEANSING =				(1L << 28), //may remove
		COUNTERSPELL =			(1L << 29), //may remove
		MIND_SHIELD =			(1L << 30), //qe
		FIRM_RESOLVE =			(1L << 31), //qe
		ARROW_CATCHING =		(1L << 32), //qe
		EVASION =				(1L << 33),
		PERFECT_AIM =			(1L << 34), //e
		//animals?


		//generic-y bollocks
		HEALING =				(1L << 35), //qie
		MANA =					(1L << 36), //qi
		WOOD =					(1L << 37), //qi
		HERB =					(1L << 38), //qie
		LEATHER =				(1L << 39), //qi
		IRON =					(1L << 40), //qi
		TOOLS =					(1L << 41), //qi
		WEAPON =				(1L << 42), //qi
		RANGED =				(1L << 43), //qi
		ARMOR =					(1L << 44), //qi
		STRENGTH =				(1L << 45), //qie
		AGILITY =				(1L << 46), //qie
		INTELLIGENCE =			(1L << 47), //qie
		CHARISMA =				(1L << 48), //qie
		WEAKNESS =				(1L << 49), //qi
		CLUMSINESS =			(1L << 50), //qi
		STUPIDITY =				(1L << 51), //qi
		UGLINESS =				(1L << 52)  //qi

			//thieves tools!
	}

	[Flags]
	public enum AidType {
		WEAPON =			(1 << 0), //i
		RANGED_WEAPON =		(1 << 1), //i
		LIGHT_ARMOR =		(1 << 2), //i
		MEDIUM_ARMOR =		(1 << 3), //i
		HEAVY_ARMOR =		(1 << 4),
		LIGHT_SHIELD =		(1 << 5), //i
		HEAVY_SHIELD =		(1 << 6),
		BARKSKIN =			(1 << 7), //i
		HEALING_TINY =		(1 << 8), //i
		HEALING_SMALL =		(1 << 9), //i
		HEALING_MEDIUM =	(1 << 10), //i
		HEALING_LARGE =		(1 << 11), //i
		MANA_TINY =			(1 << 12), //i
		MANA_SMALL =		(1 << 13), //i
		MANA_MEDIUM =		(1 << 14), //i
		MANA_LARGE =		(1 << 15), //i
		RESSURECTION =		(1 << 16), //i
		RETRY_FAILURE =		(1 << 17) //i
	}
}
