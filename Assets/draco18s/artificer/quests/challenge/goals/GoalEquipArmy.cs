using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalEquipArmy : ObstacleType, IQuestGoal {
		public GoalEquipArmy() : base("equipping soldiers", new RequireWrapper(RequirementType.ARMOR), new RequireWrapper(RequirementType.WEAPON)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;

			ItemStack stackA = theQuest.getHeroItemWith(RequirementType.ARMOR);
			ItemStack stackB = theQuest.getHeroItemWith(RequirementType.WEAPON);
			if(stackA != null && stackA.relicData != null) result = ((EnumResult)Math.Max((int)result, (int)EnumResult.MIXED) + 1);
			if(stackB != null && stackB.relicData != null) result = ((EnumResult)Math.Max((int)result, (int)EnumResult.MIXED) + 1);

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
				case EnumResult.MIXED:
					theQuest.hastenQuestEnding(300);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Outfitted soldiers";
		}

		public string relicNames(ItemStack stack) {
			return "Quartermaster";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
