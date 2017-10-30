using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalSettleTown : ObstacleType, IQuestGoal {
		public GoalSettleTown() : base("settling a new town", new RequireWrapper(RequirementType.WOOD), new RequireWrapper(RequirementType.LEATHER), new RequireWrapper(RequirementType.IRON)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
				if(theQuest.testStrength(0)) {
					result += 1;
				}
			}
			if(theQuest.testCharisma(questBonus)) {
				result += 1;
				if(theQuest.testCharisma(0)) {
					result += 1;
				}
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			else if(result < EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
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
					questBonus += 1;
					if(questBonus < 10) theQuest.repeatTask();
					break;
				case EnumResult.CRIT_SUCCESS:
					questBonus += 2;
					if(questBonus < 10) {
						theQuest.repeatTask();
					}
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in establishing a town";
		}

		public string relicNames(ItemStack stack) {
			return "Settler";
		}

		public int getNumTotalEncounters() {
			return 11;
		}
	}
}
