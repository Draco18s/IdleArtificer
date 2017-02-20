using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleUneventful : ObstacleType {
		public ObstacleUneventful() : base("traveling") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			ChallengeTypes.Loot.AddCommonResource(theQuest);
			theQuest.hastenQuestEnding(60);
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL: //meander about a bit
					theQuest.hastenQuestEnding(30);
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
				case EnumResult.CRIT_SUCCESS: //make up for lost time
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
