using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleRiver : ObstacleType {
		public ObstacleRiver() : base("river rafting", new RequireWrapper(RequirementType.AGILITY)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testAgility(questBonus)) {
				if(theQuest.testLuck(questBonus + 1) != 0) {
					result += 1;
				}
			}
			else {
				if(!theQuest.testAgility(2)) {
					result -= 1;
				}
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(15, DamageType.DROWN);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					theQuest.hastenQuestEnding(-180);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-240);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}
	}
}
