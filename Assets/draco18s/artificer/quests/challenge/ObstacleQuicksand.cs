using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleQuicksand : ObstacleType {
		public ObstacleQuicksand() : base("floundering in quicksand", new RequireWrapper(RequirementType.DANGER_SENSE,RequirementType.FREE_MOVEMENT)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			int mod = questBonus;
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) {
				mod += 4;
				result = EnumResult.MIXED;
			}
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(theQuest.testStrength(mod)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(30, DamageType.DROWN);
					theQuest.addTime(60);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(30, DamageType.DROWN);
					theQuest.addTime(30);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(15, DamageType.DROWN);
					break;
				case EnumResult.SUCCESS:
					theQuest.hastenQuestEnding(30);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}
	}
}
