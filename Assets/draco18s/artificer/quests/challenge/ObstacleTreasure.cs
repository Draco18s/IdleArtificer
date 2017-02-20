using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleTreasure : ObstacleType {
		public ObstacleTreasure() : base("opening a locked chest") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
				if(theQuest.testAgility(0)) {
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
					theQuest.harmHero(10, DamageType.POISON);
					break;
				case EnumResult.FAIL:
					ChallengeTypes.Loot.AddRandomStatPotion(theQuest, 1);
					break;
				case EnumResult.MIXED: //not sure possible
					ChallengeTypes.Loot.AddRandomStatPotion(theQuest, 2);
					break;
				case EnumResult.SUCCESS:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_HEALTH, 3));
					theQuest.addItemToInventory(new ItemStack(Industries.POT_MANA, 2));
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_HEALTH, 4));
					theQuest.addItemToInventory(new ItemStack(Industries.POT_MANA, 2));
					ChallengeTypes.Loot.AddRareResource(theQuest);
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}

		
	}
}
