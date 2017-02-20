using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleExploreTown_Temples : ObstacleType {
		public ObstacleExploreTown_Temples() : base("exploring the temple district", new RequireWrapper(RequirementType.HOLY_DAMAGE,RequirementType.UNHOLY_IMMUNE)) {//hero avoids the graveyard

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.SUCCESS - Math.Min(fails,partials);

			if(theQuest.testStrength(0)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result+1; //this is to avoid rerolling; crit-fail here isn't neccessarily bad
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.addTime(-60);
			switch(result-1) { //undo the fiddling
				case EnumResult.CRIT_FAIL: //no crit-fail
				case EnumResult.FAIL: //spend time digging graves; raise strength
					theQuest.addTime(60);
					theQuest.raiseStrength(1);
					break;
				case EnumResult.MIXED: //leave town
					if(theQuest.testCharisma(0)) { //get a ride
						theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Travel.HICH_RIDE, 0));
					}
					else { //get blessed at the temple
						theQuest.addTime(60);
						theQuest.raiseIntelligence(1);
					}
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.GRAVEYARD, 0));
					break;
			}
		}
	}
}
