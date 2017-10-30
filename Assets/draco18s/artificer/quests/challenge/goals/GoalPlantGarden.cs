using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalPlantGarden : ObstacleType, IQuestGoal {
		public GoalPlantGarden() : base("planting a garden", new RequireWrapper(RequirementType.HERB), new RequireWrapper(RequirementType.HERB), new RequireWrapper(RequirementType.HERB)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;
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
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddResource(theQuest, Items.ItemType.HERB);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddResource(theQuest, Items.ItemType.HERB);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in planting a garden";
		}

		public string relicNames(ItemStack stack) {
			return "Gardener";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
