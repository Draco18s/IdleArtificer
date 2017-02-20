using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleMonster : ObstacleType,IMonsterChallenge {
		DamageType atkDamage;
		RequirementType weakTo;
		public ObstacleMonster(string name, DamageType monsterAttackType, RequirementType monsterWeakness) : base("fighting a monster", name, new RequireWrapper(monsterAttackType.getImmunityType()), new RequireWrapper(monsterWeakness)) {
			atkDamage = monsterAttackType;
			weakTo = monsterWeakness;
		}

		public int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem) {
			int bonus = 0;
			if(rangedItem != null && rangedItem.doesStackHave(weakTo)) {
				bonus += 10;
			}
			switch(result) {
				case EnumResult.MIXED:
					return 5 + questBonus * 1;
				case EnumResult.SUCCESS:
					return 5 + questBonus * 2 + bonus;
				case EnumResult.CRIT_SUCCESS:
					return 10 + questBonus * 5 + bonus;
			}
			return 0;
		}

		public int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem) {
			bool rightAtk = theQuest.doesHeroHave(weakTo);
			
			if(result < EnumResult.MIXED)
				return 0 + (rightAtk ? 20 : 0);
			else
				return ((int)(result-1) * 5) + (rightAtk?20:0);
		}

		public int getMonsterTotalHealth() {
			return 200;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 1) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.addTime(-30);
			theQuest.hastenQuestEnding(-30);
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
					theQuest.harmHero(20, atkDamage);
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, atkDamage);
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

		public void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus) {
			ChallengeTypes.Loot.AddUncommonResource(theQuest);
		}
	}
}
