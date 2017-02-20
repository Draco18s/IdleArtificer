using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleLost : ObstacleType {
		public ObstacleLost() : base("lost") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			if(theQuest.testIntelligence(4 + questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testCharisma(0)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(60);
			switch(result) {
				case EnumResult.CRIT_FAIL: //set upon by a thief
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.THIEF, 0));
					break;
				case EnumResult.FAIL: //still lost
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED: //lose time
					theQuest.addTime(30);
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.SUCCESS: //find your way
					break;
				case EnumResult.CRIT_SUCCESS: //make up for lost time
					theQuest.hastenQuestEnding(-120);
					break;
			}
		}
	}
}
