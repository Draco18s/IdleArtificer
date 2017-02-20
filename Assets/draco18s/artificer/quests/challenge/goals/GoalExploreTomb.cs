using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using RPGKit.FantasyNameGenerator;
using RPGKit.FantasyNameGenerator.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	class GoalExploreTomb : ObstacleType, IQuestGoal {
		private static FantasyNameSettings fantasyNameSettings = new FantasyNameSettings(Classes.Rogue, Race.None, true, true, Gender.Male);
		private static IFantasyNameGenerator fantasyNameGenerator = FantasyNameGenerator.FromSettingsInfo(fantasyNameSettings);
		public GoalExploreTomb() : base("raiding a tomb", new RequireWrapper(RequirementType.DANGER_SENSE, RequirementType.LIGHT), new RequireWrapper(RequirementType.FIRE_DAMAGE), new RequireWrapper(RequirementType.DISRUPTION)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;

			if(theQuest.doesHeroHave(RequirementType.DANGER_SENSE, false)) {
				if(theQuest.testIntelligence(questBonus)) {
					result = EnumResult.CRIT_SUCCESS;
				}
				else {
					result = EnumResult.SUCCESS;
				}
			}
			else {
				result = EnumResult.FAIL;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			int newBonus = 0;
			switch(result) {
				case EnumResult.CRIT_FAIL:
					newBonus = -3;
					break;
				case EnumResult.FAIL:
					newBonus = -1;
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					newBonus = 1;
					break;
				case EnumResult.CRIT_SUCCESS:
					newBonus = 3;
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 1));
					break;
			}
			if(theQuest.doesHeroHave(RequirementType.FIRE_DAMAGE)) {
				//mummy
				theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Goals.Sub.MUMMY, newBonus));
			}
			else {
				//skeletons
				theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Goals.Sub.SKELETONS, newBonus));
			}
		}

		public string relicDescription(ItemStack stack) {
			FantasyName[] names = fantasyNameGenerator.GetFantasyNames(1);
			string tombName = names[0].FirstName + " " + names[0].Postfix;
			return "Aided in exploring " + tombName + " 's tomb";
		}

		public string relicNames(ItemStack stack) {
			return "Tomb Raider";
		}

		public int getNumTotalEncounters() {
			return 15;
		}
	}
}