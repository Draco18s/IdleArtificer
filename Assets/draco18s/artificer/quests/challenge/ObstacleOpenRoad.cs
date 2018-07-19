using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleOpenRoad : ObstacleType {
		public ObstacleOpenRoad() : base("open road") {

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
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL: //meander about a bit
					theQuest.harmHero(5, DamageType.STARVE);
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.MIXED:
					theQuest.hastenQuestEnding(30);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS: //make up for lost time
					theQuest.hastenQuestEnding(-180);
					break;
			}
		}
	}
}
