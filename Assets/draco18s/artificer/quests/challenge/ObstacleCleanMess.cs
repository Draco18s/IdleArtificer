using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleCleanMess : ObstacleType { //TODO: this makes no sense
		public ObstacleCleanMess() : base("picking up dropped items", new RequireWrapper(RequirementType.CLUMSINESS, RequirementType.AGILITY)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;
			if(fails == 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

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
					theQuest.removeItemFromInventory(theQuest.getRandomItem());
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(90);
					break;
				case EnumResult.MIXED:
					theQuest.hastenQuestEnding(30);
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					break;
			}
		}
	}
}
