using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleWaterTransport : ObstacleType {
		public ObstacleWaterTransport() : base("hiring a boat", new RequireWrapper(RequirementType.CHARISMA)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;

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
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(60);
					theQuest.addTime(30);
					break;
				case EnumResult.FAIL:
					theQuest.raiseCharisma(questBonus > 0 ? 1 : 0); //if the hero had a bonus and still failed
					theQuest.addTime(30);
					break;
				case EnumResult.MIXED: //nothing bad, nothing good
					break;
				case EnumResult.SUCCESS:
					theQuest.hastenQuestEnding(-180);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-240);
					break;
			}
			theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.RIVER, questBonus));
		}
	}
}
