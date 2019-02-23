using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleChurchyard : ObstacleType {
		public ObstacleChurchyard() : base("in a churchyard", new RequireWrapper(RequirementType.UNHOLY_IMMUNE, RequirementType.WEAPON)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.CRIT_FAIL;
			else result = EnumResult.SUCCESS;
			if(theQuest.doesHeroHave(RequirementType.WOOD)) {
				partials++;
			}
			int mod = questBonus + (theQuest.doesHeroHave(AidType.WEAPON) ? 2 : 0);

			if(theQuest.testLuck(2) == 0) {
				if(theQuest.testIntelligence(questBonus)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}
			else {
				if(theQuest.testStrength(mod)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}
			if(partials > 0 && result < EnumResult.SUCCESS) {
				result++;
			}
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(120);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.Monsters.UNDEAD, 0));
					break;
				case EnumResult.MIXED:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.EXPLORE_TOWN, 0));
					break;
				case EnumResult.SUCCESS:
					theQuest.heal(10);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.EXPLORE_TOWN, 0));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.heal(15);
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
			}
		}
	}
}
