using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalRecruitAllies : ObstacleType, IQuestGoal {
		public GoalRecruitAllies() : base("recruiting allies", new RequireWrapper(RequirementType.CHARISMA)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.FAIL - fails;
			for(int i=0;i<4;i++) {
				if(theQuest.testCharisma(questBonus)) {
					result++;
				}
			}
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					break;
				case EnumResult.FAIL:
				case EnumResult.MIXED:
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided in recruitment";
		}

		public string relicNames(ItemStack stack) {
			return "Recruiter";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
