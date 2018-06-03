using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleFestival : ObstacleType {
		public ObstacleFestival() : base("attending a festival", new RequireWrapper(RequirementType.CLUMSINESS, RequirementType.AGILITY), new RequireWrapper(RequirementType.STUPIDITY, RequirementType.INTELLIGENCE), new RequireWrapper(RequirementType.UGLINESS, RequirementType.CHARISMA)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails - partials == 3) result = EnumResult.FAIL;
			else result = EnumResult.MIXED;

			if(theQuest.testLuck(questBonus + 2) == 0) {
				result -= 1;
			}
			else {
				result += 1;
			}
			if(theQuest.testLuck(questBonus + 2) != 0) {
				result += 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(20, DamageType.POISON);
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(60);
					break;
				case EnumResult.MIXED:
					theQuest.addItemToInventory(new ItemStack(Industries.POT_SM_HEALTH, 1));
					break;
				case EnumResult.SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand, true), 0));
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					break;
			}
		}
	}
}
