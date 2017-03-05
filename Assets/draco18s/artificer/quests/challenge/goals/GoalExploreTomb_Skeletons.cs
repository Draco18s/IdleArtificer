using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalExploreTomb_Skeletons : ObstacleType, IQuestGoal, IMonsterChallenge {
		protected int skeletonsMaxHealth;
		public GoalExploreTomb_Skeletons() : base("putting down skeletons", new RequireWrapper(RequirementType.DISRUPTION), new RequireWrapper(RequirementType.WEAPON)) {
			skeletonsMaxHealth = 200;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) {
				result = EnumResult.FAIL;
			}
			else result = EnumResult.MIXED;

			int mod = theQuest.doesHeroHave(RequirementType.HEALING) ? 2 : 0;

			if(theQuest.testStrength(mod)) {
				result += 1;
			}
			if(theQuest.testStrength(mod)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(30, DamageType.UNHOLY);
					theQuest.harmHero(10, DamageType.GENERIC);
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.UNHOLY);
					theQuest.harmHero(10, DamageType.GENERIC);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(20, DamageType.GENERIC);
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					break;
			}
		}

		public string relicNames(ItemStack stack) {
			return "";
		}

		public string relicDescription(ItemStack stack) {
			return "";
		}

		public int getNumTotalEncounters() {
			return 7;
		}

		public int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem) {
			return 0;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			int bonus = 0;
			if(meleeItem != null && meleeItem.doesStackHave(RequirementType.HOLY_DAMAGE)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.MIXED:
					return 25 + questBonus * 5;
				case EnumResult.SUCCESS:
					return 50 + questBonus * 2 + bonus;
				case EnumResult.CRIT_SUCCESS:
					return 100 + questBonus * 5 + bonus;
			}
			return 0;
		}

		public int getMonsterTotalHealth() {
			return skeletonsMaxHealth;
		}

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			Item i = Items.MUMMY_WRAPPING;
			int s = i.minStackSize;
			//Main.instance.player.addItemToInventory(new ItemStack(i, s));
			ChallengeTypes.Loot.AddStack(theQuest, new ItemStack(i, s));
		}
	}
}
