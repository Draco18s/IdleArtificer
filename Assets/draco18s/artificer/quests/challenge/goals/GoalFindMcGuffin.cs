using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalFindMcGuffin : ObstacleType, IQuestGoal {
		public GoalFindMcGuffin() : base("finding magic item", new RequireWrapper(RequirementType.INTELLIGENCE), new RequireWrapper(RequirementType.MANA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand, true), 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.getRandom(theQuest.questRand), 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.getRandom(theQuest.questRand), 0));
					break;
				case EnumResult.SUCCESS:
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.getRandom(theQuest.questRand), 2));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.getRandom(theQuest.questRand), 2));
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in a search";
		}

		public string relicNames(ItemStack stack) {
			return "Finder";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
