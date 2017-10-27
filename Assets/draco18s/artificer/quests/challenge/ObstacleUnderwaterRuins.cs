using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleUnderwaterRuins : ObstacleType {
		public ObstacleUnderwaterRuins() : base("in some underwater ruins", new RequireWrapper(RequirementType.WATER_BREATH)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;
			if(fails > 0) result = EnumResult.CRIT_FAIL;
			else result = EnumResult.MIXED;

			long v = 0;
			if(!Main.instance.player.questTypeCompletion.TryGetValue(ChallengeTypes.Goals.Bonus.KRAKEN.name, out v)) {
				v = 0;
			}
			if(v == 0 && StatisticsTracker.maxQuestDifficulty.value >= 15) {
				Quest q = Quest.GenerateNewQuest(ChallengeTypes.Goals.Bonus.KRAKEN);
				QuestManager.availableQuests.Add(q);
				QuestManager.updateLists();
				return EnumResult.CRIT_FAIL;
			}

			if(result == EnumResult.CRIT_FAIL) {
				if(theQuest.testAgility(questBonus) || theQuest.testStrength(questBonus)) {
					result += 1;
				}
			}
			else {
				if(theQuest.doesHeroHave(RequirementType.FREE_MOVEMENT)) {
					result = EnumResult.CRIT_SUCCESS;
				}
				else {
					bool anyFail = false;
					if(theQuest.testAgility(0)) {
						result += 1;
					}
					else {
						anyFail = true;
					}
					if(theQuest.testStrength(0)) {
						result += 1;
					}
					else {
						anyFail = true;
					}
					if(anyFail) result -= 1;
				}
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(70, DamageType.DROWN);
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(10, DamageType.DROWN);
					break;
				case EnumResult.MIXED:
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					theQuest.raiseStrength(1);
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.TREASURE, 0));
					break;
			}
		}
	}
}