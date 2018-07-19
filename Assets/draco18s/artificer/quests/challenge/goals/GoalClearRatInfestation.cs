using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalClearRatInfestation : ObstacleType, IQuestGoal {
		public GoalClearRatInfestation() : base("exterminating rats", new RequireWrapper(RequirementType.POISON_DAMAGE, RequirementType.WEAKNESS)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED; 
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(theQuest.testAgility(questBonus)) {
				result += 1;
				if(theQuest.doesHeroHave(RequirementType.WEAPON) || theQuest.testStrength(questBonus)) {
					result += 1;
				}
			}
			else {
				if(theQuest.testStrength(questBonus)) {
					result += 1;
				}
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(60);
					theQuest.harmHero(25, DamageType.GENERIC);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(30);
					theQuest.harmHero(15, DamageType.GENERIC);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					if(questBonus < 5)
						theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in killing rats";
		}

		public string relicNames(ItemStack stack) {
			return "Rat Exterminator";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
