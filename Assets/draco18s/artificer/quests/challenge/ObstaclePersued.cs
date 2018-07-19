using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstaclePersued : ObstacleType {
		public ObstaclePersued() : base("being chased", new RequireWrapper(RequirementType.FREE_MOVEMENT, RequirementType.AGILITY)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(partials > 0) result = EnumResult.MIXED;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.SUCCESS;

			int mod = questBonus + (theQuest.doesHeroHave(RequirementType.CLUMSINESS) ? 4 : 0);
			if(theQuest.testAgility(mod)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL: //caught!
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.DETAINED, 0));
					break;
				case EnumResult.FAIL: //lose lead/get caught
					questBonus -= 1;
					if(questBonus > -1) theQuest.repeatTask();
					else theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.DETAINED, 0));
					break;
				case EnumResult.MIXED: //gain lead
					questBonus += 2;
					if(questBonus < 5) theQuest.repeatTask(); //break away
					break;
				case EnumResult.SUCCESS: //get away
					break;
				case EnumResult.CRIT_SUCCESS: //get away and find a shortcut
					theQuest.addTime(-30);
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
