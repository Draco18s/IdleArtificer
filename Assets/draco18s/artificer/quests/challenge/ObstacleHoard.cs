using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleHoard : ObstacleType {
		public ObstacleHoard() : base("raiding a hoard") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			int mod = ((theQuest.heroCurHealth < theQuest.heroMaxHealth) ? 2 : 0);
			if(theQuest.testIntelligence(mod + questBonus)) {
				result += 1;
				if(theQuest.testAgility(mod)) {
					result += 1;
				}
			}
			else {
				result -= 1;
				if(!theQuest.testIntelligence(0)) {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(10, DamageType.GENERIC);
					break;
				case EnumResult.FAIL:
					ChallengeTypes.Loot.AddRandomStatPotion(theQuest, 1);
					break;
				case EnumResult.MIXED: //not sure possible
					ChallengeTypes.Loot.AddRandomStatPotion(theQuest, 2);
					break;
				case EnumResult.SUCCESS:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_HEALTH, 2));
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_MANA, 1));
					ChallengeTypes.Loot.AddResource(theQuest, Items.ItemType.GEM);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_HEALTH, 3));
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_MANA, 2));
					ChallengeTypes.Loot.AddResource(theQuest, Items.ItemType.GEM);
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}
	}
}
