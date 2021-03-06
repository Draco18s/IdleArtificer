﻿using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstcaleStealSupplies : ObstacleType {
		public ObstcaleStealSupplies() : base("stealing supplies", new RequireWrapper(RequirementType.STEALTH,RequirementType.STUPIDITY)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CRIT_SUCCESS;

			bool armor = theQuest.doesHeroHave(AidType.LIGHT_ARMOR, false) || theQuest.doesHeroHave(AidType.MEDIUM_ARMOR, false) || theQuest.doesHeroHave(AidType.HEAVY_ARMOR, false);
			if(!theQuest.doesHeroHave(AidType.WEAPON, false)) {
				result = EnumResult.FAIL;
			}
			else if(!armor) {
				result = EnumResult.MIXED;
			}
			else if(!theQuest.doesHeroHave(RequirementType.LIGHT, false)) {
				result = EnumResult.SUCCESS;
			}

			if(fails == 0) {
				result = EnumResult.CRIT_FAIL;
			}
			if(partials == 0) {
				theQuest.addTime(30);
				theQuest.hastenQuestEnding(30);
			}

			return result + 1;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result - 1) {
				case EnumResult.CRIT_FAIL:
					theQuest.inventory.Add(new ItemStack(Industries.POT_SM_HEALTH, 2));
					break;
				case EnumResult.FAIL:
					theQuest.inventory.Add(new ItemStack(Industries.QUARTERSTAFF, 2));
					break;
				case EnumResult.MIXED:
					theQuest.inventory.Add(new ItemStack(Industries.LEATHER, 2));
					break;
				case EnumResult.SUCCESS:
					theQuest.inventory.Add(new ItemStack(Industries.TORCHES, 2));
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					theQuest.addTime(-30);
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
