using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalPetitionTheDuke : ObstacleType, IQuestGoal {
		public GoalPetitionTheDuke() : base("petitioning the duke", new RequireWrapper(RequirementType.CHARISMA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL - fails;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			if(theQuest.testCharisma(questBonus)) {
				result += 1;
			}
			if(theQuest.testCharisma(questBonus)) {
				result += 1;
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
					theQuest.repeatTask();
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
			return "Swayed an opinion";
		}

		public string relicNames(ItemStack stack) {
			return "Petitioner";
		}

		public int getNumTotalEncounters() {
			return 12;
		}
	}
}
