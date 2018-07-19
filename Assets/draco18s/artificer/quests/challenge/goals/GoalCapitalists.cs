using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalCapitalists : ObstacleType, IQuestGoal {
		public GoalCapitalists() : base("making money", new RequireWrapper(RequirementType.GOLD), new RequireWrapper(RequirementType.CHARISMA,RequirementType.INTELLIGENCE)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL - fails;
			int mod = questBonus;
			if(theQuest.testCharisma(mod)) {
				result += 1;
			}
			if(theQuest.testIntelligence(mod)) {
				result += 1;
				if(theQuest.testCharisma(0)) {
					result += 1;
				}
			}
			if(theQuest.doesHeroHave(RequirementType.TOOLS) || theQuest.testLuck(20) <= 5) {
				result += 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(120);
					theQuest.harmHero(10, DamageType.PETRIFY);
					questBonus -= 1;
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 2;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRelic(theQuest);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Plated in gold and worth some dosh";
		}

		public string relicNames(ItemStack stack) {
			return "Expensive";
		}

		public int getNumTotalEncounters() {
			return 11;
		}
	}
}
