using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.hero;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	class ObstacleHeroDuel : ObstacleType {
		protected static Random rand = new Random();

		public ObstacleHeroDuel() : base("dueling another hero") {

		}

		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED;
			if(theQuest.inventory.Count == 0) return EnumResult.CONTINUE;

			HarrowDeck opponent = new HarrowDeck();

			int opScore = 0;
			int heroScore = 0;

			if(theQuest.testStrength(questBonus)) {
				heroScore += 1;
			}
			if(theQuest.testAgility(questBonus)) {
				heroScore += 1;
			}
			if(testAttribute(opponent.STR.tokens)) {
				opScore += 1;
			}
			if(testAttribute(opponent.AGL.tokens)) {
				opScore += 1;
			}
			if((result + heroScore - opScore) == EnumResult.MIXED && theQuest.heroCurHealth < 30) {
				return EnumResult.CRIT_FAIL;
			}
			return result + heroScore - opScore;
		}

		protected bool testAttribute(int value) {
			return rand.Next(20) <= value;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.addTime(-30);
			theQuest.hastenQuestEnding(-30);
			switch(result) {
				case EnumResult.CONTINUE: //hero HAS NO ITEMS, this adventure node is basically skipped.
					theQuest.addTime(-30);
					theQuest.hastenQuestEnding(-30);
					//add replacement (odds of repeating this task are low)
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Unexpected.getRandom(theQuest.questRand), questBonus));
					break;
				case EnumResult.CRIT_FAIL: //no real difference
				case EnumResult.FAIL:
					ItemStack item = theQuest.getRandomItem();
					if(item != null) //just in case
						theQuest.removeItemFromInventory(item);
					else
						theQuest.harmHero(25, DamageType.GENERIC);
					break;
				case EnumResult.MIXED:
					theQuest.harmHero(5, DamageType.PETRIFY); //we don't want to use up items
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS: //no real difference
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRelic(theQuest);
					break;
			}
		}
	}
}
