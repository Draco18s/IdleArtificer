using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleFairMaiden : ObstacleType {
		public ObstacleFairMaiden() : base("talking with a maiden", new RequireWrapper(RequirementType.CHARISMA)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			if(theQuest.heroCurHealth < 60) {
				return EnumResult.FAIL;
			}
			if(theQuest.QuestTimeLeft() < 600) {
				return EnumResult.MIXED;
			}
			if(fails > 0 && !theQuest.testCharisma(questBonus)) {
				return EnumResult.SUCCESS;
			}
			return EnumResult.CRIT_SUCCESS;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //not possible
					theQuest.addTime(60);
					break;
				case EnumResult.FAIL:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_HEALTH, 1));
					break;
				case EnumResult.MIXED:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_MANA, 1));
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseCharisma(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}
	}
}