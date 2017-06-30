using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleExploreTown_Harbor : ObstacleType {
		public ObstacleExploreTown_Harbor() : base("exploring the town harbor") {

		}

		//TODO:
		//MIXED is not possible here

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testCharisma(0) || theQuest.testIntelligence(0)) {
				result += 1;
			}
			else {
				result -= 1;
				if(!theQuest.testStrength(questBonus)) {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(60);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.AMBUSH, 0));
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.PIRATE_SHIP, 0));
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Sub.BAR_BRAWL, 0));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.TAVERN, questBonus));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.SAIL_SEAS, questBonus));
					break;
			}
		}
	}
}
