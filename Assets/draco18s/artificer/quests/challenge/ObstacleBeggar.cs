using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleBeggar : ObstacleType {
		public ObstacleBeggar() : base("talking to a beggar") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL + theQuest.testLuck(4);

			/*if(theQuest.testCharisma(0) || theQuest.testIntelligence(0)) {
				result += 1;
			}*/

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL: //cutpurse
					theQuest.addTime(-30);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.THIEF, questBonus - 1));
					break;
				case EnumResult.MIXED:
				case EnumResult.SUCCESS: //just a beggar
					break;
				case EnumResult.CRIT_SUCCESS: //genie
					Debug.Log("Crit!");
					theQuest.addTime(-30);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Sub.GENIE, questBonus + 1));
					break;
			}
		}
	}
}
