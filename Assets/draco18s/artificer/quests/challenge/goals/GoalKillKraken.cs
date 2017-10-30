using System;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.items;
using RPGKit.FantasyNameGenerator;
using RPGKit.FantasyNameGenerator.Generators;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.statistics;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalKillKraken : ObstacleType, IQuestGoal, IMonsterChallenge {
		protected int dragonMaxHealth;
		//private static FantasyNameSettings fantasyNameSettings = new FantasyNameSettings(Classes.Warrior, Race.Dragon, true, true, Gender.Male);
		//private static IFantasyNameGenerator fantasyNameGenerator = FantasyNameGenerator.FromSettingsInfo(fantasyNameSettings);
		public GoalKillKraken() : base("fighting a kraken", new RequireWrapper(RequirementType.ACID_IMMUNE), new RequireWrapper(RequirementType.FIRE_DAMAGE), new RequireWrapper(RequirementType.WATER_BREATH)) {
			dragonMaxHealth = 1250;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			result = EnumResult.MIXED - fails;

			int mod = questBonus + (theQuest.doesHeroHave(AidType.WEAPON) ? 2 : 0);

			if(theQuest.testStrength(mod)) {
				result += 1;
			}
			if(theQuest.testStrength(mod-2)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(30, DamageType.ACID);
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
			if(rangedItem != null && rangedItem.doesStackHave(RequirementType.COLD_DAMAGE)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.ACID);
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
			if(stack.item.hasAidType(AidType.WEAPON))
				return "Kraken Breaker";
			return "Kraken Ward";
		}

		public string relicDescription(ItemStack stack) {
			//FantasyName[] names = fantasyNameGenerator.GetFantasyNames(1);
			//string dragonName = names[0].FirstName + " " + names[0].LastName + " " + names[0].Postfix;
			return "Aided a hero in slaying the kraken";
		}

		public int getNumTotalEncounters() {
			return 9;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			switch(result) {
				case EnumResult.MIXED:
					return 25 + questBonus * 1;
				case EnumResult.SUCCESS:
					return 50 + questBonus * 2;
				case EnumResult.CRIT_SUCCESS:
					return 100 + questBonus * 5;
			}
			return 0;
		}

		public int getMonsterTotalHealth() {
			return dragonMaxHealth;
		}

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			StatisticsTracker.defeatKraken.setAchieved();
			StatisticsTracker.maxQuestDifficulty.addValue(1);

			Item i = Items.DRAGON_SCALES;
			int s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
			ChallengeTypes.Loot.AddStack(theQuest, new ItemStack(i, s));
			i = Items.SLIME_GOO;
			s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
			ChallengeTypes.Loot.AddStack(theQuest, new ItemStack(i, s));
			ChallengeTypes.Loot.AddRareResource(theQuest);
		}
	}
}
