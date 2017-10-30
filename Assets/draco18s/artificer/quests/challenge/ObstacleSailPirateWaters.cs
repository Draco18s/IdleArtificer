using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleSailPirateWaters : ObstacleType {
		public ObstacleSailPirateWaters() : base("sailing the pirate waters") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
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

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //marooned
					theQuest.hastenQuestEnding(120);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.Pirates.MAROONED, questBonus - 1));
					break;
				case EnumResult.FAIL: //thrown overboard
					theQuest.harmHero(15, DamageType.DROWN);
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseIntelligence(1);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.Pirates.LOST_LAGOON, questBonus));
					break;
				case EnumResult.CRIT_SUCCESS:
					//auto-succeed
					theQuest.addItemToInventory(new ItemStack(Industries.POT_WATER_BREATH, questBonus + 1));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.Pirates.UNDERWATER_RUINS, 0));
					break;
			}
		}
	}
}
