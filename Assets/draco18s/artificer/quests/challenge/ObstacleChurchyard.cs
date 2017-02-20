using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleChurchyard : ObstacleType {
		public ObstacleChurchyard() : base("in a churchyard", new RequireWrapper(RequirementType.UNHOLY_IMMUNE)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			int mod = questBonus + (theQuest.doesHeroHave(AidType.WEAPON) ? 2 : 0);

			if(theQuest.testLuck(2) == 0) {
				if(theQuest.testIntelligence(questBonus)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}
			else {
				if(theQuest.testStrength(mod)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL: //no actual fail possible
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.MIXED:
				case EnumResult.SUCCESS: //no actual success possible
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.EXPLORE_TOWN, 0));
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}
	}
}
