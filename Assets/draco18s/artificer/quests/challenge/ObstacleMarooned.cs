using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleMarooned : ObstacleType {
		public ObstacleMarooned() : base("marooned at sea") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;
			result = EnumResult.MIXED;
			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testAgility(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testIntelligence(0)) {
				result += 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) return EnumResult.CRIT_SUCCESS;
			if(result < EnumResult.CRIT_FAIL) return EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //tried to swim away, failed
					theQuest.harmHero(5, DamageType.DROWN);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL: //still here, but closer to escape, low on supplies
					theQuest.harmHero(5, DamageType.STARVE);
					questBonus += 2; //TODO!!!
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED: //lose time
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.SUCCESS: //rescued
					theQuest.raiseCharisma(1);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.EXPLORE_TOWN, 0));
					theQuest.hastenQuestEnding(-60);
					break;
				case EnumResult.CRIT_SUCCESS: //find buried treasure
					theQuest.raiseIntelligence(1);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
