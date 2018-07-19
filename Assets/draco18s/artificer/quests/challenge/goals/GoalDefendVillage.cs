using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalDefendVillage : ObstacleType, IQuestGoal {
		public GoalDefendVillage() : base("defending village", new RequireWrapper(RequirementType.ARMOR), new RequireWrapper(RequirementType.ARMOR), new RequireWrapper(RequirementType.WEAPON), new RequireWrapper(RequirementType.WEAPON)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.MIXED;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
		
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.harmHero(5, DamageType.GENERIC);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(20, DamageType.GENERIC);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.GENERIC);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Defended civilians from attack";
		}

		public string relicNames(ItemStack stack) {
			return "Defending";
		}

		public int getNumTotalEncounters() {
			return 12;
		}
	}
}
