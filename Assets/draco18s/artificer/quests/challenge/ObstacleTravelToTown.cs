using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleTravelToTown : ObstacleType {
		public ObstacleTravelToTown() : base("going to town") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CRIT_FAIL;

			result += theQuest.testLuck(4) + 1;

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			int b = 0;
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL: //wasted some time
					theQuest.hastenQuestEnding(90);
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					b = 1;
					break;
				case EnumResult.CRIT_SUCCESS:
					b = 2;
					theQuest.hastenQuestEnding(-60);
					break;
			}
			theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.EXPLORE_TOWN, b+ questBonus));
		}
	}
}
