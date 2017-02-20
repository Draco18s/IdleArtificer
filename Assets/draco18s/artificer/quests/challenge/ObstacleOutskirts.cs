using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleOutskirts : ObstacleType {
		public ObstacleOutskirts() : base("on the town outskirts") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CRIT_FAIL;

			result += theQuest.testLuck(4) + 1;

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL: //go somewhere else
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.HICH_RIDE, 0));
					break;
				case EnumResult.MIXED: //go somewhere else
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.HICH_RIDE, 1));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.GOTO_TOWN, 0));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.GOTO_TOWN, 1));
					break;
			}
		}
	}
}
