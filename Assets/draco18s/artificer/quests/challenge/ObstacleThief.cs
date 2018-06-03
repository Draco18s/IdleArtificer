using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleThief : ObstacleType {
		public ObstacleThief() : base("dealing with a thief", new RequireWrapper(RequirementType.DETECTION, RequirementType.LIGHT)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL:
					ItemStack item = theQuest.getRandomItem();
					if(item != null)
						theQuest.removeItemFromInventory(item);
					else
						theQuest.harmHero(15, DamageType.GENERIC);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(15, DamageType.GENERIC);
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_HEALTH, 1));
					break;
			}
		}
	}
}
