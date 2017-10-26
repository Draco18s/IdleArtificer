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
	public class ObstaclePriest : ObstacleType {
		public ObstaclePriest() : base("talking with a priest", new RequireWrapper(RequirementType.UNHOLY_IMMUNE, RequirementType.CHARISMA)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			if(theQuest.heroCurHealth < 60) {
				return EnumResult.FAIL;
			}
			if(theQuest.QuestTimeLeft() < 600) {
				return EnumResult.MIXED;
			}
			int mod = (partials > 0 ? 2 : 0);
			if(fails > 0 || !theQuest.testCharisma(questBonus + mod)) {
				return EnumResult.SUCCESS;
			}
			return EnumResult.CRIT_SUCCESS;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(-30);
			switch(result) {
				case EnumResult.CRIT_FAIL: //not possible
					theQuest.addTime(60);
					break;
				case EnumResult.FAIL:
					theQuest.heroCurHealth += 15;
					break;
				case EnumResult.MIXED:
					theQuest.hastenQuestEnding(-90);
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseCharisma(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					Bless(theQuest);
					break;
			}
		}

		private void Bless(Quest theQuest) {
			ItemStack newRelic = theQuest.determineRelic();
			if(newRelic != null) {
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
							goto baditem;
						}
					}
					else if((newRelic.item.equipType & ench.enchantSlotRestriction) > 0) {
						newRelic.applyEnchantment(ench);
						ret = false;
					}
				} while(ret && loopcount < 30); //shouldn't need to try *30* times.
				if(loopcount >= 30) {
					goto baditem;
				}

				if(!theQuest.inventory.Contains(newRelic))
					theQuest.inventory.Add(newRelic);
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
	}
}