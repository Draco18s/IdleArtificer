using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.init;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalExploreCave : ObstacleType, IQuestGoal {
		public GoalExploreCave() : base("exploring scary cave", new RequireWrapper(RequirementType.LIGHT)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testCharisma(0)) {
				result += 1;
			}
			else {
				if(theQuest.testLuck(questBonus+2) == 0) {
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
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand, true), 1));
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Lit up the darkness";
		}

		public string relicNames(ItemStack stack) {
			return "Glowing";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
