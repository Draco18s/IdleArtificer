using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalEvilBard : ObstacleType, IQuestGoal, IMonsterChallenge {
		protected int bardHealth;

		public GoalEvilBard() : base("fighting an evil bard", new RequireWrapper(RequirementType.UGLINESS), new RequireWrapper(RequirementType.MIND_SHIELD)) {
			bardHealth = 10;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL;// = EnumResult.CRIT_FAIL;
			
			if(theQuest.doesHeroHave(RequirementType.UGLINESS, false)) {
				result += 1;
			}
			if(theQuest.doesHeroHave(RequirementType.MIND_SHIELD, false)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			int mod = questBonus + (theQuest.doesHeroHave(RequirementType.CHARISMA) ? 4 : 0);

			if(theQuest.testCharisma(mod)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(result < EnumResult.CRIT_FAIL) result = EnumResult.CRIT_FAIL;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			int i = 0;
			switch(result) {
				case EnumResult.CRIT_FAIL:
					i = theQuest.raiseIntelligence(-1);
					theQuest.raiseCharisma(-1);
					if(i <= 0) {
						theQuest.harmHero(25, DamageType.POISON, true);
						theQuest.harmHero(25, DamageType.PETRIFY, true);
					}
					break;
				case EnumResult.FAIL:
					i = theQuest.raiseIntelligence(-1);
					if(i <= 0) {
						theQuest.harmHero(25, DamageType.POISON, true);
						theQuest.harmHero(25, DamageType.PETRIFY, true);
					}
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					questBonus += 1;
					break;
			}
		}

		public int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem) {
			return 0;
		}

		public string relicNames(ItemStack stack) {
			return "Bardic";
		}

		public string relicDescription(ItemStack stack) {
			if(stack.item.hasReqType(RequirementType.MIND_SHIELD))
				return "Shielded against an evil bard's mind control";
			return "Stopped an evil bard";
		}

		public int getNumTotalEncounters() {
			return 17;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			switch(result) {
				case EnumResult.SUCCESS:
					return 1;
				case EnumResult.CRIT_SUCCESS:
					return 2;
			}
			return 0;
		}

		public int getMonsterTotalHealth() {
			return bardHealth;
		}

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			Item i = Items.DIAMONDS;
			int s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
			//Main.instance.player.addItemToInventory(new ItemStack(i, s));
			ChallengeTypes.Loot.AddStack(theQuest, new ItemStack(i, s));
		}
	}
}
