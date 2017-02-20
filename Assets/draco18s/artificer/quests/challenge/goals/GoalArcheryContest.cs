using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalArcheryContest : ObstacleType, IQuestGoal {
		public GoalArcheryContest() : base("practicing archery", new RequireWrapper(RequirementType.RANGED), new RequireWrapper(RequirementType.AGILITY)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL - fails;
			int mod = questBonus;// + (theQuest.doesHeroHave(RequirementType.AGILITY) ? 4 : 0);
			if(theQuest.testAgility(mod)) {
				result += 1;
			}
			if(theQuest.testAgility(mod)) {
				result += 1;
				if(theQuest.testAgility(0)) {
					result += 1;
				}
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(240);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(120);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Hit the mark";
		}

		public string relicNames(ItemStack stack) {
			return "Unerring";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
