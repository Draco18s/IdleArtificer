using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleMountainPath : ObstacleType {
		public ObstacleMountainPath() : base("on a narrow mountain path", new RequireWrapper(RequirementType.AGILITY, RequirementType.FEATHER_FALL)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
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
					theQuest.repeatTask();
					theQuest.harmHero(30, DamageType.FALL);
					break;
				case EnumResult.FAIL:
					theQuest.repeatTask();
					theQuest.harmHero(10, DamageType.FALL);
					break;
				case EnumResult.MIXED:
					theQuest.addTime(30);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addTime(-15);
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
