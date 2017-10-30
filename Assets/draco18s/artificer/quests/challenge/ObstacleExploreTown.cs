using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleExploreTown : ObstacleType {
		public ObstacleExploreTown() : base("exploring a town", new RequireWrapper(RequirementType.CHARISMA)) {//does basically nothing

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.CONTINUE + theQuest.testLuck(5);

			if(theQuest.testCharisma(0) || theQuest.testIntelligence(0)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.addTime(-30);
			switch(result) { //undo the fiddling
				case EnumResult.CRIT_FAIL: //leave town
					theQuest.addTime(60); //undo time addition, above
					if(theQuest.testCharisma(0)) {
						theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.HICH_RIDE, 0));
					}
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.HARBOR,0));
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.TEMPLE, 0));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.MARKET, 0));
					break;
				case EnumResult.CRIT_SUCCESS: 
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.GARDENS, 0));
					break;
			}
		}
	}
}
