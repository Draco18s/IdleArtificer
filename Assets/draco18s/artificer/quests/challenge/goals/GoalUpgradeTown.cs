using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalUpgradeTown : ObstacleType, IQuestGoal {
		public GoalUpgradeTown() : base("building infrastructure", new RequireWrapper(RequirementType.TOOLS, RequirementType.WOOD), new RequireWrapper(RequirementType.IRON, RequirementType.LEATHER)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;

			int mod = questBonus + (partials * 3);
			if(theQuest.testStrength(mod)) {
				result += 1;
			}
			if(theQuest.testStrength(mod)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					QuestManager.availableQuests.Add(Quest.GenerateNewQuest(ChallengeTypes.Goals.REPAIR_DAMN,theQuest.heroName));
					/*theQuest.hastenQuestEnding(240);
					theQuest.repeatTask();*/
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
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in upgrading town infrastructure";
		}

		public string relicNames(ItemStack stack) {
			return "Builder";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
