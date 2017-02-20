using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleWilderness : ObstacleType {
		public ObstacleWilderness() : base("out in the wilderness") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			if(theQuest.testIntelligence(questBonus-2)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testStrength(0) || theQuest.testAgility(0)) {
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
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(10, DamageType.STARVE);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					theQuest.addTime(30);
					theQuest.hastenQuestEnding(90);
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-120);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}
	}
}
