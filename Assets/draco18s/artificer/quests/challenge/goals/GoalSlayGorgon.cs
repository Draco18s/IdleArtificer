using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalSlayGorgon : ObstacleType, IQuestGoal, IMonsterChallenge {
		protected int gorgonMaxHealth;
		public GoalSlayGorgon() : base("slaying the gorgon", new RequireWrapper(RequirementType.MIRRORED)) {
			gorgonMaxHealth = 100;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) {
				result = EnumResult.CRIT_FAIL;
			}
			else result = EnumResult.SUCCESS;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(100, DamageType.PETRIFY);
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(50, DamageType.PETRIFY);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(10, DamageType.GENERIC);
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					break;
			}
		}

		public int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem) {
			int bonus = 0;
			if(rangedItem != null && rangedItem.doesStackHave(RequirementType.UNHOLY_DAMAGE)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
				case EnumResult.MIXED:
					return 5 + questBonus * 1;
				case EnumResult.SUCCESS:
					return 5 + questBonus * 2 + bonus;
				case EnumResult.CRIT_SUCCESS:
					return 10 + questBonus * 5 + bonus;
			}
			return 0;
		}

		public string relicNames(ItemStack stack) {
			return "Mirrorguard";
		}

		public string relicDescription(ItemStack stack) {
			return "Aided a hero in slaying the gorgon";
		}

		public int getNumTotalEncounters() {
			return 17;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			int bonus = 0;
			if(meleeItem != null && meleeItem.doesStackHave(RequirementType.UNHOLY_DAMAGE)) {
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
			return gorgonMaxHealth;
		}

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			/*Item i = Items.DRAGON_SCALES;
			int s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
			Main.instance.player.addItemToInventory(new ItemStack(i, s));*/
		}
	}
}
