using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalReachSummit : ObstacleType, IQuestGoal {
		public GoalReachSummit() : base("climbing a mountain", new RequireWrapper(RequirementType.ENDURANCE,RequirementType.COLD_IMMUNE), new RequireWrapper(0,RequirementType.FEATHER_FALL), new RequireWrapper(RequirementType.MANA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) {
				result = EnumResult.MIXED;
				if(fails > 2) {
					result = EnumResult.FAIL;
				}
			}
			else result = EnumResult.SUCCESS;

			int mod = 0 + (partials > 0 ? 2 : 0);
			if(theQuest.testCharisma(mod)) {
				result += 1;
			}
			else {
				if(theQuest.testLuck(questBonus + 2) == 0) {
					result -= 1;
				}
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(15, DamageType.FALL);
					theQuest.hastenQuestEnding(90);
					theQuest.repeatTask();
					questBonus -= 1;
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(15, DamageType.FALL);
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
					else {
						ChallengeTypes.Loot.AddRareResource(theQuest);
					}
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand, true), 1));
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Endured rough climes";
		}

		public string relicNames(ItemStack stack) {
			return "Mountaineering";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
