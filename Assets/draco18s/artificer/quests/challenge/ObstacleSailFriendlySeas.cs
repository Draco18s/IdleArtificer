using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleSailFriendlySeas : ObstacleType {
		public ObstacleSailFriendlySeas() : base("sailing the high seas") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			result = EnumResult.MIXED;

			if(theQuest.testIntelligence(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}
			if(theQuest.testCharisma(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.hastenQuestEnding(-30);

			if(result <= EnumResult.FAIL) {
				long v = 0;
				if(!Main.instance.player.questTypeCompletion.TryGetValue(ChallengeTypes.Goals.Bonus.KRAKEN, out v)) {
					v = 0;
				}
				if(v == 0 && StatisticsTracker.maxQuestDifficulty.value >= 15) {
					Quest q = Quest.GenerateNewQuest(ChallengeTypes.Goals.Bonus.KRAKEN);
					QuestManager.availableQuests.Add(q);
					QuestManager.updateLists();
				}
				else {
					if(result == EnumResult.CRIT_FAIL)
						theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.Pirates.SAIL_PIRATE_WATERS, 0));
					else
						theQuest.repeatTask();
				}
				return;
			}

			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.Pirates.SAIL_PIRATE_WATERS, 0));
					break;
				case EnumResult.FAIL:
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					if(theQuest.testLuck(25) == 0) {
						theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Scenario.SIRENS, questBonus-1));
					}
					else {
						questBonus += 1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseStrength(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.raiseAgility(1);
					break;
			}
		}
	}
}
