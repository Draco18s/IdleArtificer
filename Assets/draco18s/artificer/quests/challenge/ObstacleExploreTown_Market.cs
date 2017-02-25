using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleExploreTown_Market : ObstacleType {
		public ObstacleExploreTown_Market() : base("exploring the town market") {

		}

		//TODO:
		//MIXED is not possible here

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testCharisma(0) || theQuest.testIntelligence(0)) {
				result += 1;
			}
			else {
				result -= 1;
				if(!theQuest.testStrength(questBonus)) {
					result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(60);
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.THIEF, 0));
					break;
				case EnumResult.FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.LOST, 0));
					break;
				case EnumResult.MIXED:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.SHOPPING, 0));
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					theQuest.addItemToInventory(new ItemStack(Industries.POT_HEALTH, 1));
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Initial.Town.SHOPPING, 0));
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					if(theQuest.testLuck(3) != 0) {
						ChallengeTypes.Loot.AddRelic(theQuest);
					}
					else {
						ChallengeTypes.Loot.AddRareResource(theQuest);
					}
					break;
			}
		}
	}
}
