using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalFindCows : ObstacleType, IQuestGoal {
		public GoalFindCows() : base("finding lost cows", new RequireWrapper(RequirementType.MANA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testLuck(questBonus + 3) != 0) {
				result += 1;
			}
			else {
				if(!theQuest.testAgility(0)) {
					result -= 1;
				}
				result -= 1;
			}

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
					if(questBonus < 5) {
						questBonus += 1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Found lost livestock";
		}

		public string relicNames(ItemStack stack) {
			return "Searching";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
