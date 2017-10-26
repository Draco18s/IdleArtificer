using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalWolves : ObstacleType, IQuestGoal {
		public GoalWolves() : base("hunting wolves", new RequireWrapper(RequirementType.WEAPON), new RequireWrapper(RequirementType.ARMOR), new RequireWrapper(RequirementType.POISON_DAMAGE)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 1) result = EnumResult.FAIL;
			else result = EnumResult.MIXED;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			else {
				if(theQuest.testStrength(questBonus)) {
					result += 1;
				}
				result -= 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.repeatTask();
					theQuest.hastenQuestEnding(60);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.WINTERWOLF, 0));
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.WINTERWOLF, 0));
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.WINTERWOLF, 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.WINTERWOLF, 0));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.WINTERWOLF, 2));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Helped fight off wolves";
		}

		public string relicNames(ItemStack stack) {
			return "Hunting";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
