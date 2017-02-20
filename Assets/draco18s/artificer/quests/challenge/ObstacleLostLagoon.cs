using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleLostLagoon : ObstacleType {
		public ObstacleLostLagoon() : base("exploring an island") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CRIT_FAIL;

			result += theQuest.testLuck(3) + 1;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
				case EnumResult.MIXED:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, questBonus + 1));
					break;
			}
		}
	}
}
