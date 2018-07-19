using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalGuardTemple : ObstacleType, IQuestGoal {
		public GoalGuardTemple() : base("guarding temple", new RequireWrapper(RequirementType.DETECTION), new RequireWrapper(RequirementType.ARMOR)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.SUCCESS - fails;

			if(theQuest.testIntelligence(2 + questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(15, DamageType.STARVE);
					theQuest.hastenQuestEnding(90);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in performing guard duty";
		}

		public string relicNames(ItemStack stack) {
			return "Overwatch";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
