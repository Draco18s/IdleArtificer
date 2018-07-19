using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalBuildHome : ObstacleType, IQuestGoal {
		public GoalBuildHome() : base("building a house", new RequireWrapper(RequirementType.TOOLS), new RequireWrapper(RequirementType.WOOD), new RequireWrapper(RequirementType.LEATHER)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;
			if(theQuest.doesHeroHave(RequirementType.TOOLS,false)) {
				result += 2;
			}
			else {
				result++;
			}

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			if(theQuest.testStrength(questBonus)) {
				result += 1;
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
					theQuest.harmHero(10, DamageType.STARVE);
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 2;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in establishing a home";
		}

		public string relicNames(ItemStack stack) {
			return "Homesteader";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
