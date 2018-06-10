using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
 public class ObstacleHome : ObstacleType {
		public ObstacleHome() : base("at home", new RequireWrapper(0,RequirementType.MANA)) { //the mana potion just prevents losing time during attempt result

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CRIT_FAIL;
			if(fails == 0) {
				theQuest.hastenQuestEnding(-180);
			}
			result += theQuest.testLuck(4) + 1;

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(60);
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.getRandom(theQuest.questRand), 0));
					break;
				case EnumResult.SUCCESS: //get a ride
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.HICH_RIDE, 0));
					break;
				case EnumResult.CRIT_SUCCESS: //get a ride, much bonus
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.HICH_RIDE, 1));
					break;
			}
		}
	}
}
