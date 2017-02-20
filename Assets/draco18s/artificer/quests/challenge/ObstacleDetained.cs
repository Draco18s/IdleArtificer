using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleDetained : ObstacleType {
		public ObstacleDetained() : base("being detained", new RequireWrapper(RequirementType.ETHEREALNESS), new RequireWrapper(RequirementType.CHARISMA)) {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			//tweak to generate actual desired pattern
			//charisma as hard-require is only for display purposes
			fails = 0;
			partials = 0;
			if(!theQuest.doesHeroHave(RequirementType.ETHEREALNESS, false)) {
				fails = 1;
				if(theQuest.doesHeroHave(RequirementType.CHARISMA, false))
					partials = 1;
			}

			if(fails > 0) {
				if(partials > 0) result = EnumResult.MIXED;
				else result = EnumResult.FAIL;

				int mod = questBonus + (theQuest.doesHeroHave(RequirementType.CHARISMA) ? 4 : 0);
				if(theQuest.testCharisma(mod)) {
					result += 1;
				}
				else {
					result -= 1;
				}
			}
			else result = EnumResult.CRIT_SUCCESS;

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.addTime(60);
					questBonus -= 2;
					if(questBonus > -1) theQuest.repeatTask();
					break;
				case EnumResult.FAIL: //lose lead/get caught
					questBonus -= 1;
					if(questBonus > -1) theQuest.repeatTask();
					break;
				case EnumResult.MIXED: //gain lead
					questBonus += 1;
					if(questBonus < 5) theQuest.repeatTask(); //break away
					break;
				case EnumResult.SUCCESS: //get away
					break;
				case EnumResult.CRIT_SUCCESS: //get away and find a shortcut
					theQuest.hastenQuestEnding(-60);
					break;
			}
		}
	}
}
