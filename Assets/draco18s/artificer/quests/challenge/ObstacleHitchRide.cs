using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleHitchRide : ObstacleType {
		public ObstacleHitchRide() : base("hitching a ride") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			if(theQuest.testCharisma(questBonus)) {
				result += 1;
				if(theQuest.testCharisma(0)) {
					result += 1;
				}
			}
			else {
				if(!theQuest.testCharisma(0)) {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL:
					theQuest.raiseCharisma(questBonus > 0 ? 1 : 0); //if the hero had a bonus and still failed
					theQuest.addTime(60);
					break;
				case EnumResult.MIXED: //nothing bad, nothing good
					break;
				case EnumResult.SUCCESS:
					theQuest.hastenQuestEnding(-300);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-360);
					break;
			}
		}
	}
}
