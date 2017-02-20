using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleTrappedPassage : ObstacleType {
		DamageType damage;
		public ObstacleTrappedPassage(DamageType trapDamageType) : base("in a trapped passage", trapDamageType.getImmunityType().ToString(), new RequireWrapper(RequirementType.DETECTION, RequirementType.AGILITY)) {
			damage = trapDamageType;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			if(theQuest.testAgility(questBonus)) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL:
					theQuest.harmHero(15, damage);
					break;
				case EnumResult.MIXED: //nothing bad, nothing good
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseIntelligence(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-60);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					break;
			}
		}
	}
}
