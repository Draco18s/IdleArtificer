using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalSphynxRiddles : ObstacleType, IQuestGoal {
		public GoalSphynxRiddles() : base("solving riddles", new RequireWrapper(RequirementType.INTELLIGENCE), new RequireWrapper(RequirementType.MANA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED - fails;
			else result = EnumResult.MIXED;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
				if(theQuest.testIntelligence(questBonus)) {
					result += 1;
					if(theQuest.testIntelligence(questBonus)) {
						result += 1;
						if(theQuest.testIntelligence(questBonus)) {
							result += 1;
						}
					}
				}
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(90);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(60);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					questBonus += 2;
					theQuest.hastenQuestEnding(-30);
					theQuest.repeatTask();
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in solving riddles";
		}

		public string relicNames(ItemStack stack) {
			return "Riddler";
		}

		public int getNumTotalEncounters() {
			return 12;
		}
	}
}
