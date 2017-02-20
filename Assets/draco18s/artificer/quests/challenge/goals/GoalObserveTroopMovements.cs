using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalObserveTroopMovements : ObstacleType, IQuestGoal {
		public GoalObserveTroopMovements() : base("observing the enemy", new RequireWrapper(RequirementType.STEALTH),new RequireWrapper(RequirementType.INTELLIGENCE)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;
			int mod = questBonus;// + (partials > 0 ? 4 : 0);
			if(theQuest.testAgility(mod)) {
				result += 1;
			}
			if(theQuest.testIntelligence(mod)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.AMBUSH, 0));
					break;
				case EnumResult.FAIL:
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					QuestManager.availableQuests.Add(Quest.GenerateNewQuest(ChallengeTypes.Goals.DEFEND_VILLAGE, theQuest.heroName));
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					QuestManager.availableQuests.Add(Quest.GenerateNewQuest(ChallengeTypes.Goals.DEFEND_VILLAGE, theQuest.heroName));
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in gathering intel";
		}

		public string relicNames(ItemStack stack) {
			return "Observant";
		}

		public int getNumTotalEncounters() {
			return 12;
		}
	}
}
