using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.items {
	public class ItemPowerStone : Item {
		private AidType[] healingTypes = { AidType.RESSURECTION, AidType.HEALING_LARGE, AidType.HEALING_MEDIUM, AidType.HEALING_SMALL, AidType.HEALING_TINY, AidType.MANA_LARGE, AidType.MANA_MEDIUM, AidType.MANA_SMALL, AidType.MANA_TINY };
		public ItemPowerStone():base("stone_of_power") {
			
		}

		public override bool hasReqType(RequirementType type) {
			if(type == 0) return false;
			return type != RequirementType.MANA && type != RequirementType.HEALING; //(reqProperties & type) > 0;
		}

		public override bool hasAidType(AidType type) {
			if(type == 0) return false;
			if(healingTypes.Contains(type)) {
				return false;
			}
			return true; //(aidProperties & type) > 0;
		}

		public override void onUsedDuringQuest(Quest quest, ItemStack itemStack) {
			UnityEngine.Debug.Log("Curse advances");
			if(quest.miscData == null)
				quest.miscData = new Dictionary<string, object>();
			object corwrap;
			int cor = 0;
			if(quest.miscData.TryGetValue("corruption", out corwrap)) {
				cor = (int)corwrap;
				quest.miscData.Remove("corruption");
			}
			object fallwrap;
			bool fallen = false;
			if(quest.miscData.TryGetValue("fallen", out fallwrap)) {
				fallen = (bool)fallwrap;
			}
			int c = -cor;
			if(!quest.testCharisma(c))
				cor++;
			if(!quest.testCharisma(c))
				cor++;
			quest.miscData.Add("corruption", Math.Max(cor, 1));
			quest.harmHero(Math.Max(cor, 1), DamageType.PETRIFY);
			if(quest.heroCurHealth <= 0 && cor >= 8 && !fallen) {
				quest.miscData.Add("fallen", true);
				//new corrupted hero quest goal
				QuestManager.availableRelics.Add(QuestManager.makeRelic(itemStack, new Curse(quest.heroName), 1, this.name));
				if(StatisticsTracker.maxQuestDifficulty.value >= 20) {
					Quest q = Quest.GenerateNewQuest(ChallengeTypes.Goals.Sub.FALLEN_HERO);
					QuestManager.availableQuests.Add(q);
					QuestManager.updateLists();
					Main.instance.debugMode = false;
				}
			}
			if(quest.miscData.TryGetValue("corruption", out corwrap)) {
				cor = (int)corwrap;
				UnityEngine.Debug.Log("Corruption so far: " + cor);
			}
		}

		public override bool isSpecial() {
			return true;
		}

		private class Curse : IRelicMaker {
			private string cursedHero;
			public Curse(string hero) {
				cursedHero = hero;
			}

			public string relicNames(ItemStack stack) {
				return "Cursed";
			}

			public string relicDescription(ItemStack stack) {
				return "Caused " + cursedHero + " to fall";
			}
		}
	}
}
