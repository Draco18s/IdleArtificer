using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalDelivery : ObstacleType, IQuestGoal {
		public GoalDelivery() : base("delivering items") {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CRIT_FAIL;
			foreach(ItemStack stack in theQuest.inventory) {
				if(stack.item == Items.SpecialItems.MISC_GOODS) {
					result = EnumResult.CRIT_SUCCESS;
				}
			}
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					break;
				case EnumResult.FAIL:
				case EnumResult.MIXED:
				case EnumResult.SUCCESS: //don't occur
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in a delivery";
		}

		public string relicNames(ItemStack stack) {
			return "Portage";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
