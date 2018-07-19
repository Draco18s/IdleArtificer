using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalHuntSpirits : ObstacleType, IQuestGoal, IMonsterChallenge {
		protected int dragonMaxHealth;
		public GoalHuntSpirits() : base("hunting spirits", new RequireWrapper(RequirementType.COUNTERSPELL),new RequireWrapper(RequirementType.DISPELLING), new RequireWrapper(RequirementType.SPELL_RESIST)) {
			dragonMaxHealth = 800;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) {
				result = EnumResult.FAIL;
				if(fails > 1) {
					result = EnumResult.CRIT_FAIL;
				}
			}
			else result = EnumResult.MIXED;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(30, DamageType.FIRE, true);
					theQuest.harmHero(10, DamageType.COLD, true);
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.ACID, true);
					theQuest.harmHero(10, DamageType.POISON, true);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(20, DamageType.GENERIC, true);
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					break;
			}
		}

		public int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem) {
			int bonus = 0;
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.HOLY);
					break;
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
			return "Banishing";
		}

		public string relicDescription(ItemStack stack) {
			return "Aided banishing spirits";
		}

		public int getNumTotalEncounters() {
			return 13;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			int bonus = 0;
			if(meleeItem != null && meleeItem.doesStackHave(RequirementType.FIRE_DAMAGE)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.CRIT_FAIL:
					return -50;
				case EnumResult.FAIL:
					return -25;
				case EnumResult.MIXED:
					return 20 + questBonus * 1 + bonus;
				case EnumResult.SUCCESS:
					return 40 + questBonus * 2 + bonus;
				case EnumResult.CRIT_SUCCESS:
					return 90 + questBonus * 5 + bonus;
			}
			return 0;
		}

		public int getMonsterTotalHealth() {
			return dragonMaxHealth;
		}

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			ChallengeTypes.Loot.AddRareResource(theQuest);
		}
	}
}
