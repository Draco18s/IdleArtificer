using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalEngageInWar : ObstacleType, IQuestGoal, IDescriptorData {
		public GoalEngageInWar() : base("fighting the enemy", new RequireWrapper(RequirementType.WEAPON), new RequireWrapper(RequirementType.ARMOR,RequirementType.HEALING)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL - fails;
			int mod = (theQuest.doesHeroHave(RequirementType.RANGED) ? 2 : 0) + questBonus;
			if(theQuest.testStrength(mod)) {
				result += 1;
			}
			if(theQuest.testAgility(mod)) {
				result += 1;
			}
			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.harmHero(15, DamageType.GENERIC);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(25, DamageType.GENERIC);
					theQuest.hastenQuestEnding(60);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.GENERIC);
					theQuest.hastenQuestEnding(30);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					if(questBonus < 7) {
						questBonus = +1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					QuestManager.availableQuests.Add(Quest.GenerateNewQuest(theQuest.heroName));
					QuestManager.updateLists();
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Fought in a war for the Kingdom of {0}";
		}

		public string relicNames(ItemStack stack) {
			return "Warmonger";
		}

		public int getNumTotalEncounters() {
			return 9;
		}

		public string getDescValue() {
			return "WarSide";
		}
	}
}
