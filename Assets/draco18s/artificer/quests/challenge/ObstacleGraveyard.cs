using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleGraveyard : ObstacleType {

		public ObstacleGraveyard() : base("having an encounter in a graveyard", new RequireWrapper(RequirementType.UNHOLY_IMMUNE, RequirementType.DISPELLING), new RequireWrapper(RequirementType.HOLY_DAMAGE, RequirementType.DISRUPTION)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;

			if(fails > 0) {
				if(fails > 1) {
					result = EnumResult.CRIT_FAIL;
				}
				else {
					result = EnumResult.FAIL;
				}
				result += partials > 0 ? 1 : 0;
			}
			//else if(partials > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			int mod = questBonus + (theQuest.doesHeroHave(AidType.WEAPON) ? 2 : 0);
			if(theQuest.testStrength(mod)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(30, DamageType.UNHOLY);
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(15, DamageType.UNHOLY);
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					theQuest.addTime(-15);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addTime(-30);
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
