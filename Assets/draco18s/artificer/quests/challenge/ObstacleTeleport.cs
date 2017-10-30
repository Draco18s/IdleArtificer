using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.hero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleTeleport : ObstacleType {
		public ObstacleTeleport() : base("teleporting") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
				if(theQuest.testIntelligence(0)) {
					result += 1;
				}
			}
			else {
				if(theQuest.testLuck(20) < 10 + questBonus) {
					result += 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL:
					theQuest.addTime(60);
					theQuest.harmHero(10, DamageType.HOLY);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.LOST, 0));
					break;
				case EnumResult.MIXED: //nothing bad, nothing good
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.LOST, 4));
					break;
				case EnumResult.SUCCESS:
					theQuest.hastenQuestEnding(-300);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-300);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}
	}
}
