using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleMagicTrap : ObstacleType {
		DamageType damage;
		public ObstacleMagicTrap(DamageType trapDamageType) : base("dealing with a magic trap", trapDamageType.getImmunityType().ToString(), new RequireWrapper(RequirementType.DANGER_SENSE, RequirementType.DETECTION)) {
			damage = trapDamageType;
		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;
			int mod = theQuest.doesHeroHave(RequirementType.TOOLS) ? 4 : 0;

			if(theQuest.testIntelligence(questBonus + mod)) {
				result += 1;
			}
			if(theQuest.doesHeroHave(RequirementType.SPELL_RESIST) || theQuest.doesHeroHave(RequirementType.COUNTERSPELL)) {
				result += 1;
			}
			if(result > EnumResult.CRIT_SUCCESS) result = EnumResult.CRIT_SUCCESS;
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //no crit fail
				case EnumResult.FAIL:
					theQuest.harmHero(25, damage, true);
					break;
				case EnumResult.MIXED: //nothing bad, nothing good
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseAgility(1);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-60);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					ChallengeTypes.Loot.AddRandomStatPotion(theQuest, 1);
					break;
			}
		}
	}
}
