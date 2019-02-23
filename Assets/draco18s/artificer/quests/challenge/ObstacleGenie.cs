using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleGenie : ObstacleType,IRelicMaker {
		public ObstacleGenie() : base("talking to a genie") {
			setRewardScalar(12);
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL + theQuest.testLuck(2);

			int mod = questBonus + (theQuest.doesHeroHave(RequirementType.CHARISMA)?2:0);

			if(theQuest.testCharisma(mod)) {
				result += 1;
			}
			if(theQuest.testCharisma(0)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.HOLY);
					theQuest.harmHero(10, DamageType.PETRIFY, true);
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.MIXED:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					enchantRandomItem(theQuest);
					break;
			}
		}

		private void enchantRandomItem(Quest theQuest) {
			ItemStack newRelic = theQuest.determineRelic();
			if(newRelic != null) {
				//Debug.Log("Genie is enchanting an item");
				newRelic = QuestManager.makeRelic(newRelic, this, theQuest.heroName);
				//Debug.Log("Enchanting a(n) " + newRelic.item.name);
				Item item;// = Items.getRandom(theQuest.questRand);
				bool ret = true;
				int loopcount = 0;
				do {
					loopcount++;
					item = Items.getRandom(theQuest.questRand);
					Enchantment ench = GameRegistry.GetEnchantmentByItem(item);
					//Debug.Log("Trying... " + ench.name);
					if(ench == null) continue;
					if(newRelic.enchants.Count > 0) {
						bool maxed = false;
						if(newRelic.enchants.Contains(ench)) {
							int count = 0;
							foreach(Enchantment ench1 in newRelic.enchants) {
								if(ench1 == ench) {
									count++;
								}
							}
							maxed = count >= ench.maxConcurrent;
						}
						if(maxed || newRelic.relicData == null || newRelic.relicData.Count < newRelic.enchants.Count) {
							//Debug.Log("This item is maxed out already. Weird.");
							goto baditem;
						}
					}
					else if((newRelic.item.equipType & ench.enchantSlotRestriction) > 0) {
						//Debug.Log("Applied!");
						newRelic.applyEnchantment(ench);
						ret = false;
					}
				} while(ret && loopcount < 30); //shouldn't need to try *30* times.
				if(loopcount >= 30) {
					goto baditem;
				}

				if(!theQuest.inventory.Contains(newRelic))
					theQuest.inventory.Add(newRelic);
				//allRelics.Add(newRelic);
				//availableRelics.Add(newRelic);
				StatisticsTracker.relicsMade.addValue(1);
				if(StatisticsTracker.relicsMade.value == 1) {
					StatisticsTracker.maxQuestDifficulty.addValue(1);
				}
				if(!StatisticsTracker.relicFromGenie.isAchieved()) {
					StatisticsTracker.relicFromGenie.setAchieved();
				}
				return;
				baditem:
				ChallengeTypes.Loot.AddRareResource(theQuest);
				ChallengeTypes.Loot.AddRareResource(theQuest);
			}
			else {
				ChallengeTypes.Loot.AddRareResource(theQuest);
				ChallengeTypes.Loot.AddRareResource(theQuest);
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Enchanted by a genie";
		}

		public string relicNames(ItemStack stack) {
			return "Wished";
		}
	}
}
