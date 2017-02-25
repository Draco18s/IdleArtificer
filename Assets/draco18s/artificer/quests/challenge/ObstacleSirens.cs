using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleSirens : ObstacleType {
		public ObstacleSirens() : base("encountering sirens") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;
			result = EnumResult.MIXED;
			if(theQuest.testCharisma(questBonus+2)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testCharisma(2)) {
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
				case EnumResult.CRIT_FAIL: //tried to swim to the sirens
					theQuest.harmHero(25, DamageType.DROWN);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.Pirates.MAROONED, 0));
					break;
				case EnumResult.FAIL: //on board, taking a beating from the waves
					theQuest.harmHero(5, DamageType.DROWN);
					questBonus += 2;
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseCharisma(1);
					theQuest.hastenQuestEnding(-60);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.raiseIntelligence(1);
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
