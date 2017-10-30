using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleDrunkenFight : ObstacleType {
		public ObstacleDrunkenFight() : base("in a drunken fight", new RequireWrapper(RequirementType.WEAPON),new RequireWrapper(RequirementType.CLUMSINESS)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;
			result -= fails;
			if(theQuest.testStrength(questBonus)) {
				result++;
			}
			if(theQuest.testAgility(questBonus)) {
				result++;
			}
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.harmHero(30, DamageType.GENERIC);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(60);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.THIEF, questBonus - 1));
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseStrength(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.raiseStrength(1);
					theQuest.raiseAgility(1);
					break;
			}
		}
	}
}
