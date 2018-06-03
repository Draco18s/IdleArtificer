using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleNightWatch : ObstacleType {
		public ObstacleNightWatch() : base("taking the night watch", new RequireWrapper(RequirementType.DETECTION,RequirementType.DANGER_SENSE), new RequireWrapper(RequirementType.LIGHT)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;
			int mod = partials > 0 ? 2 : 0;
			if(theQuest.testIntelligence(4 + questBonus + mod)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testIntelligence(questBonus + mod)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //set upon by a thief
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.THIEF, 0));
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.LOST, 0));
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS: //early move-on
					theQuest.addTime(-30);
					break;
				case EnumResult.CRIT_SUCCESS: //job well done reward
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_MANA, 1));
					break;
			}
		}
	}
}
