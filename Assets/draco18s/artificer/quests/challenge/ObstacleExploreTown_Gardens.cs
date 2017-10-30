using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleExploreTown_Gardens : ObstacleType {
		public ObstacleExploreTown_Gardens() : base("relaxing in the gardens") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			if(!theQuest.testAgility(0)) {
				return EnumResult.FAIL + 1;
			}
			if(!theQuest.testIntelligence(0)) {
				return EnumResult.MIXED + 1;
			}
			if(!theQuest.testCharisma(0)) {
				return EnumResult.SUCCESS + 1;
			}
			if(!theQuest.testStrength(0)) {
				return EnumResult.CRIT_SUCCESS + 1;
			}
			return EnumResult.CRIT_FAIL + 1;
			//fiddling because crit fail is not bad
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.addTime(-30);
			switch(result-1) { //undo the fiddling
				case EnumResult.CRIT_FAIL: //stats tests passed: have some loot
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand), 4));
					break;
				case EnumResult.FAIL:
					theQuest.raiseAgility(1);
					break;
				case EnumResult.MIXED:
					theQuest.raiseIntelligence(1);
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseCharisma(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.raiseStrength(1);
					break;
			}
		}
	}
}
