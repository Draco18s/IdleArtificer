using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstaclePirateShip : ObstacleType {
		public ObstaclePirateShip() : base("sneaking onto pirate ship", new RequireWrapper(RequirementType.STEALTH, RequirementType.MIND_SHIELD)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(result == EnumResult.MIXED) {
				if(theQuest.testCharisma(4)) {
					result += 2;
				}
				else {
					result += 1;
				}
			}
			else if(result == EnumResult.SUCCESS) {
				if(theQuest.testAgility(-2)) {
					result += 1;
				}
			}
			else {
				if(!theQuest.testStrength(2)) {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //the pirates aren't fooled and attack
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.AMBUSH, 0));
					break;
				case EnumResult.FAIL: //the disguise breaks and the hero is thrown overboard
					theQuest.harmHero(15, DamageType.DROWN);
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.PIRATE_SHIP, 0));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.PIRATE_SHIP, 1));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.PIRATE_SHIP, 2));
					break;
			}
		}
	}
}
