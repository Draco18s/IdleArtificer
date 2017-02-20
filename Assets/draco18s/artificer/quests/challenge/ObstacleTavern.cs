using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleTavern : ObstacleType {
		public ObstacleTavern() : base("exploring a town", new RequireWrapper(RequirementType.DANGER_SENSE, RequirementType.STRENGTH)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(theQuest.testLuck(2) == 0) {
				if(theQuest.testIntelligence(0)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}
			else {
				if(theQuest.testStrength(0)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //get drunk and have an item stolen
					theQuest.addTime(60);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.THIEF, -2));
					break;
				case EnumResult.FAIL: //get drunk and lose time
					theQuest.addTime(60);
					break;
				case EnumResult.MIXED: //spend a night without incident
					theQuest.addTime(30);
					theQuest.hastenQuestEnding(-30);
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseCharisma(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.General.MAIDEN, 0));
					break;
			}
		}
	}
}
