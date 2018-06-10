using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleDecypherMap : ObstacleType {
		public ObstacleDecypherMap() : base("decyphering an old map", new RequireWrapper(RequirementType.INTELLIGENCE)) {//does basically nothing

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;
			int mod = (fails == 0 ? 2 : 0);
			if(theQuest.testIntelligence(questBonus+mod)) {
				result += 1;
				if(theQuest.testIntelligence(mod)) {
					result += 1;
				}
			}
			else {
				result -= 1;
				if(!theQuest.testIntelligence(mod)) {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.addTime(60);
					break;
				case EnumResult.MIXED:
					break;
				case EnumResult.SUCCESS:
					theQuest.addTime(-30);
					break;
				case EnumResult.CRIT_SUCCESS:
					//theQuest.addItemToInventory(new ItemStack(Industries.POT_HEALTH, 1));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE,0));
					break;
			}
		}
	}
}
