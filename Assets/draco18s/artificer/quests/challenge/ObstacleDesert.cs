using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleDesert : ObstacleType {
		public ObstacleDesert() : base("traveling across a desert", new RequireWrapper(RequirementType.ENDURANCE)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testStrength(0)) {
				result += 1;
			}
			else {
				if(theQuest.testLuck(questBonus + 2) == 0) {
					result -= 1;
				}
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(10, DamageType.STARVE);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(30);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand), 1));
					break;
			}
		}
	}
}
