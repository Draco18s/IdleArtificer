using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalSowChaos : ObstacleType, IQuestGoal {
		public GoalSowChaos() : base("sowing chaos", new RequireWrapper(RequirementType.UGLINESS), new RequireWrapper(RequirementType.STUPIDITY), new RequireWrapper(RequirementType.UNHOLY_DAMAGE)) {
			
		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.SUCCESS - fails;
			if(theQuest.testAgility(questBonus)) {
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
					ChallengeTypes.Loot.AddRareResource(theQuest);
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.FAIL:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.MIXED:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Embroiled chaos at the cursed forest's behest";
		}

		public string relicNames(ItemStack stack) {
			return "Corrupt";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
