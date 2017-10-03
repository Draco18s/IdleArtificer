using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleAmbush : ObstacleType {
		public ObstacleAmbush() : base("being ambushed", new RequireWrapper(RequirementType.DANGER_SENSE)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			bool armor = theQuest.doesHeroHave(AidType.LIGHT_ARMOR) || theQuest.doesHeroHave(AidType.MEDIUM_ARMOR) || theQuest.doesHeroHave(AidType.HEAVY_ARMOR);
			int mod = questBonus + (theQuest.doesHeroHave(AidType.WEAPON) ? 3 : 0) + (armor ? 1 : 0);

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
				case EnumResult.FAIL:
					theQuest.addTime(30);
					theQuest.harmHero(30, DamageType.GENERIC);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(15, DamageType.GENERIC);
					break;
				case EnumResult.SUCCESS:
					theQuest.harmHero(5, DamageType.GENERIC);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_HEALTH, questBonus));
					break;
			}
		}
	}
}
