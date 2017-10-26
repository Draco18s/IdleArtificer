using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalRunForOffice : ObstacleType, IQuestGoal {
		public GoalRunForOffice() : base("running for mayor", new RequireWrapper(RequirementType.CHARISMA), new RequireWrapper(RequirementType.FIRM_RESOLVE,RequirementType.MIND_SHIELD)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;
			if(partials > 0 && theQuest.doesHeroHave(RequirementType.INTELLIGENCE, false)) {
				result++;
			}

			if(theQuest.testCharisma(0)) {
				result += 1;
			}
			if(theQuest.testCharisma(-2)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
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
					if(questBonus >= 5) {
						ChallengeTypes.Loot.AddRareResource(theQuest);
						ChallengeTypes.Loot.AddRareResource(theQuest);
					}
					else {
						theQuest.hastenQuestEnding(-60);
						questBonus += 1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					if(questBonus < 5) {
						ChallengeTypes.Loot.AddRareResource(theQuest);
						ChallengeTypes.Loot.AddRareResource(theQuest);
					}
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in being elected mayor";
		}

		public string relicNames(ItemStack stack) {
			if(stack.item.equipType > 0)
				return "Mayoral";
			return "Canvasing";
		}

		public int getNumTotalEncounters() {
			return 9;
		}
	}
}
