using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleVampireTradingHouse : ObstacleType {
		public ObstacleVampireTradingHouse() : base("visiting vampire market", new RequireWrapper(RequirementType.UNHOLY_IMMUNE, RequirementType.HEALING), new RequireWrapper(RequirementType.CHARISMA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL;
			int mod = questBonus + (theQuest.doesHeroHave(RequirementType.UNHOLY_IMMUNE, false) ? 2 : 0);
			if(theQuest.testLuck(mod + 2) == 0) {
				result += 1;
			}
			mod += theQuest.doesHeroHave(RequirementType.HEALING) ? 2 : 0;
			if(theQuest.testLuck(mod + 2) == 0) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.doesHeroHave(RequirementType.CHARISMA,false)) {
				result += 1;
			}
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.harmHero(30, DamageType.UNHOLY);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.FAIL:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
				case EnumResult.MIXED:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddRandomStatPotion(theQuest, 1);
					theQuest.raiseStrength(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}
	}
}
