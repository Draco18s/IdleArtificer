using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalFreeSlaves : ObstacleType, IQuestGoal {
		public GoalFreeSlaves() : base("freeing slaves", new RequireWrapper(RequirementType.FIRM_RESOLVE, RequirementType.CHARISMA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(theQuest.testCharisma(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testCharisma(questBonus - 4)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(240);
					theQuest.repeatTask();
					theQuest.harmHero(20, DamageType.GENERIC);
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
					break;
				case EnumResult.CRIT_SUCCESS:
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Freed family from slavery";
		}

		public string relicNames(ItemStack stack) {
			return "Freedom";
		}

		public int getNumTotalEncounters() {
			return 17;
		}
	}
}
