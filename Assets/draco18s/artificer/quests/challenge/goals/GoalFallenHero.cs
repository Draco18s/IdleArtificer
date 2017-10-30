using System;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.items;
using RPGKit.FantasyNameGenerator;
using RPGKit.FantasyNameGenerator.Generators;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalFallenHero : ObstacleType, IQuestGoal, IMonsterChallenge, IDescriptorData {
		protected int fallenHeroMaxHealth;

		public GoalFallenHero() : base("slaying the fallen hero", new RequireWrapper(RequirementType.WEAPON), new RequireWrapper(RequirementType.HOLY_DAMAGE), new RequireWrapper(RequirementType.WEAKNESS)) {
			fallenHeroMaxHealth = 1000;
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

			int mod = questBonus + (theQuest.doesHeroHave(RequirementType.WEAKNESS,false) ? 4 : 0);

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
					theQuest.harmHero(10, DamageType.ACID);
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

		public int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem) {
			int bonus = 0;
			if(rangedItem != null && rangedItem.doesStackHave(RequirementType.HOLY_DAMAGE)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.UNHOLY);
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
			return "Redeamer";
		}

		public string relicDescription(ItemStack stack) {
			return "Aided a hero in slaying the fallen hero {0}";
		}

		public string getDescValue() {
			return "fallenHeroName";
		}

		public int getNumTotalEncounters() {
			return 19;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			int bonus = 0;
			if(meleeItem != null && meleeItem.doesStackHave(RequirementType.HOLY_DAMAGE)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.MIXED:
					return 25 + questBonus * 1;
				case EnumResult.SUCCESS:
					return 50 + questBonus * 2 + bonus;
				case EnumResult.CRIT_SUCCESS:
					return 100 + questBonus * 5 + bonus;
			}
			return 0;
		}

		public int getMonsterTotalHealth() {
			return fallenHeroMaxHealth;
		}

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			
		}
	}
}
