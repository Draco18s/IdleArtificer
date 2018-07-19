using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalSkeletons : ObstacleType, IQuestGoal {
		public GoalSkeletons() : base("quelling skeletons", new RequireWrapper(RequirementType.DISRUPTION, RequirementType.HOLY_DAMAGE), new RequireWrapper(RequirementType.UNHOLY_IMMUNE)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.FAIL; 
			else if(fails > 0) result = EnumResult.CRIT_FAIL;
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
			theQuest.harmHero(5, DamageType.GENERIC);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(10, DamageType.UNHOLY);
					theQuest.repeatTask();
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(60);
					theQuest.repeatTask();
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 2));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 2));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in quelling a skeleton uprising";
		}

		public string relicNames(ItemStack stack) {
			if(stack.doesStackHave(RequirementType.WEAPON))
				return "Bone Crusher";
			return "Bone";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
