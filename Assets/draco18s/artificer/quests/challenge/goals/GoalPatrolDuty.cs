using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalPatrolDuty : ObstacleType, IQuestGoal {
		public GoalPatrolDuty() : base("patrolling", new RequireWrapper(RequirementType.LIGHT), new RequireWrapper(RequirementType.HEALING)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testStrength(3)) {
				result += 1;
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(20, DamageType.GENERIC);
					theQuest.hastenQuestEnding(90);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.GENERIC);
					theQuest.hastenQuestEnding(60);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					if(questBonus < 3) {
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
			return "Aided in patrols";
		}

		public string relicNames(ItemStack stack) {
			return "Vigilant";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
