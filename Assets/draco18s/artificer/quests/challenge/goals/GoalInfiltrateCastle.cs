using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalInfiltrateCastle : ObstacleType, IQuestGoal {
		public GoalInfiltrateCastle() : base("infiltrating a castle", new RequireWrapper(RequirementType.ETHEREALNESS, RequirementType.STEALTH),new RequireWrapper(RequirementType.AGILITY)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;
			int mod = questBonus + (partials>0 ? 4 : 0); 
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
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.AMBUSH, -5));
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.AMBUSH, -1));
					break;
				case EnumResult.MIXED:
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRelic(theQuest);
					//add new quest: kill king
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided infiltration";
		}

		public string relicNames(ItemStack stack) {
			return "Stealthy";
		}

		public int getNumTotalEncounters() {
			return 12;
		}
	}
}
