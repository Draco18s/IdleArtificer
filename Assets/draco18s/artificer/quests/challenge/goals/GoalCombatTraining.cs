using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalCombatTraining : ObstacleType, IQuestGoal {
		public GoalCombatTraining() : base("training for a fight", new RequireWrapper(RequirementType.WEAPON, RequirementType.ARMOR)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL - fails;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			if(theQuest.testAgility(questBonus)) {
				result += 1;
			}
			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(5, DamageType.GENERIC);
					theQuest.hastenQuestEnding(150);
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
					if(questBonus < 7) {
						questBonus = +1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided training";
		}

		public string relicNames(ItemStack stack) {
			return "Trainer";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
