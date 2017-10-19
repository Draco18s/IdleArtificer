using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleBadWeather : ObstacleType {
		public ObstacleBadWeather() : base("dealing with bad weather", new RequireWrapper(0,RequirementType.ENDURANCE)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED + partials;

			if(theQuest.testIntelligence(4)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(60);
			theQuest.harmHero(5, DamageType.GENERIC);
			switch(result) {
				case EnumResult.CRIT_FAIL: //repeat task
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL: //still stuck, but making progress
					questBonus++;
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED: //get away
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS: //make up for lost time
					theQuest.hastenQuestEnding(-120);
					break;
			}
		}
	}
}
