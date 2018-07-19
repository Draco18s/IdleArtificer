using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.quests.challenge.goals.DeepGoals;
using Assets.draco18s.artificer.quests.hero;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using RPGKit.FantasyNameGenerator;
using RPGKit.FantasyNameGenerator.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.quests {
	[Serializable]
	public class Quest : ISerializable {
		private static System.Random rand = new System.Random();
		private static FantasyNameSettings fantasyNameSettings = new FantasyNameSettings(Classes.Cleric, Race.None, true, true, Gender.Male);
		private static IFantasyNameGenerator fantasyNameGenerator = FantasyNameGenerator.FromSettingsInfo(fantasyNameSettings);
		public static Quest GenerateNewQuest() {
			ObstacleType goal = ChallengeTypes.Goals.getRandom(rand);
			IDeepGoal deepGoal = Main.instance.player.getActiveDeepGoal();
			if(!deepGoal.isActive()) deepGoal = DeepGoalsTypes.NONE;
			if(rand.Next(4) == 0 && deepGoal.minQuestDifficulty() <= StatisticsTracker.maxQuestDifficulty.value) {
				//Debug.Log(Main.instance.player.getActiveDeepGoal().name);
				if(!QuestManager.availableQuests.Any(x => x.originalGoal == deepGoal.getQuestType())
					&& !QuestManager.activeQuests.Any(x => x.originalGoal == deepGoal.getQuestType()))
						goal = deepGoal.getQuestType();
			}
			FantasyName[] names = fantasyNameGenerator.GetFantasyNames(1);
			Quest quest = GenerateNewQuest(goal, names[0].FirstName + " " + names[0].Land);
			if(goal == deepGoal.getQuestType()) {
				deepGoal.modifyQuest(quest);
			}
			return quest;
		}

		public static Quest GenerateNewQuest(string heroName) {
			ObstacleType goal = ChallengeTypes.Goals.getRandom(rand);
			if(rand.Next(4) == 0) {
				ObstacleType agoal = Main.instance.player.getActiveDeepGoal().getQuestType();
				//bool exists = false;
				foreach(Quest q in QuestManager.availableQuests) {
					if(q.originalGoal == agoal) {
						//exists = true;
						goto alreadyExists;
					}
				}
				goal = agoal;
				alreadyExists:;
			}
			return GenerateNewQuest(goal, heroName);
		}

		public static Quest GenerateNewQuest(ObstacleType withGoal) {
			FantasyName[] names = fantasyNameGenerator.GetFantasyNames(1);
			return GenerateNewQuest(withGoal, names[0].FirstName + " " + names[0].Land);
		}

		public static Quest GenerateNewQuest(ObstacleType withGoal, string heroName) {
			Debug.Log("Creating new quest with goal" + withGoal);
			if(withGoal == null) {
				throw new Exception("Error creating quest wtih null goal!");
			}
			//TODO: moar detail
			List<QuestChallenge> arr = new List<QuestChallenge>();

			QuestChallenge goal = new QuestChallenge(withGoal, 0);
			
			int total = ((IQuestGoal)goal.type).getNumTotalEncounters();
			if(total < 7) total = 7;

			arr.Add(new QuestChallenge(ChallengeTypes.Initial.getRandom(rand), 0));
			//begin
			for(int n = 0; n < total; n++) {
				if(n % 2 == 1) {
					if(n % 4 == 3) {
						arr.Add(new QuestChallenge(ChallengeTypes.Loot.getRandom(rand), 0));
					}
					else {
						arr.Add(new QuestChallenge(ChallengeTypes.Unexpected.getRandom(rand), 0));
					}
				}
				else {
					if(n % 4 == 2) {
						arr.Add(new QuestChallenge(ChallengeTypes.Travel.getRandom(rand), 0));
					}
					else {
						arr.Add(new QuestChallenge(ChallengeTypes.General.getRandom(rand), 0));
					}
				}
			}
			//end
			arr.Add(goal);
			
			QuestChallenge[] list = arr.ToArray();
			long v;
			if(!Main.instance.player.questTypeCompletion.TryGetValue(list[list.Length - 1].type.name, out v)) {
				v = 0;
			}
			Quest q = new Quest(v, rand.Next(), list);
			int min = 0 + (StatisticsTracker.minQuestDifficulty.value / 3);
			int max = 4 + (StatisticsTracker.minQuestDifficulty.value / 3);
			Item i = Items.getRandom(rand, min, max);
			q.rewards[0] = new ItemStack(i, rand.Next(i.maxStackSize - i.minStackSize) + i.minStackSize);
			i = Items.getRandom(rand, 4, 7);
			q.rewards[1] = new ItemStack(i, rand.Next(i.maxStackSize - i.minStackSize) + i.minStackSize);

			q.heroName = heroName;

			//Name debugging
			//string debugName = names[0].FirstName + " " + names[0].LastName + " " + names[0].Postfix;
			//Debug.Log(debugName);
			q.timeUntilQuestExpires = 18000; //5 hours
			return q;
		}

		public static float GetQuestMaxTime() {
			return 2000 + (float)SkillList.QuestTime.getMultiplier() + ((UpgradeIntValue)Main.instance.player.upgrades[UpgradeType.HERO_STAMINA]).value;
		}

		public QuestChallenge[] obstacles {
			get {
				return _obstacles;
			}
		}
		public GameObject guiItem;
		protected long[] knownRequirements;

		public string heroName;
		public long numQuestsBefore;
		public readonly int heroMaxHealth;
		public readonly List<ItemStack> inventory;
		public readonly ItemStack[] rewards;

		public Dictionary<string, object> miscData;
		public int heroCurHealth;
		public bool questComplete = false;
		public float timeUntilQuestExpires;
		public readonly System.Random questRand;
		public EnumResult finalResult = EnumResult.CONTINUE;
		
		protected QuestChallenge[] _obstacles;
		protected ObstacleType originalGoal;
		protected int questStep;
		protected float questTimer;
		protected float questTotalTime;
		protected float timeAftercomplete;
		protected int AGL;
		protected int STR;
		protected int CHA;
		protected int INT;

		public Quest(long prior, int seed, params QuestChallenge[] challenges) {
			_obstacles = challenges;
			if(challenges.Length > 0)
				originalGoal = challenges[challenges.Length - 1].type;
			numQuestsBefore = prior;
			heroMaxHealth = 100 + ((UpgradeIntValue)Main.instance.player.upgrades[UpgradeType.HERO_HEALTH]).value;
			heroCurHealth = heroMaxHealth;
			inventory = new List<ItemStack>();
			questStep = 0;
			questTimer = 60;
			questTotalTime = GetQuestMaxTime();
			questRand = new System.Random(seed);
			HarrowDeck deck = new HarrowDeck();
			STR = deck.STR.tokens;
			AGL = deck.AGL.tokens;
			INT = deck.INT.tokens;
			CHA = deck.CHA.tokens;
			if(challenges.Length > 0) {
				List<long> reqList = new List<long>();
				//Debug.Log(Upgrades.AllRenownUps.Find(x => x.saveName == "QUEST_REQS"));
				if(Upgrades.AllRenownUps.Find(x => x.saveName == "QUEST_REQS").getIsPurchased()) {
					getBetterRequirements(ref reqList, challenges);
				}
				else {
					getBasicRequirements(ref reqList, challenges);
				}
				knownRequirements = new long[6];
				for(int ki = 0; ki < 6 && ki < reqList.Count; ki++) {
					knownRequirements[ki] = reqList[ki];
				}
				/*knownRequirements[0] = (long)challenges[0].getReq(0);
				knownRequirements[1] = (long)challenges[0].getReq(1);
				knownRequirements[2] = (long)challenges[0].getReq(2);
				knownRequirements[3] = (long)challenges[challenges.Length - 1].getReq(0);
				knownRequirements[4] = (long)challenges[challenges.Length - 1].getReq(1);
				knownRequirements[5] = (long)challenges[challenges.Length - 1].getReq(2);
				Array.Sort(knownRequirements);
				Array.Reverse(knownRequirements);*/
			}
			rewards = new ItemStack[2];
			//Debug.Log(challenges[challenges.Length - 1].type.name);
		}

		private void getBasicRequirements(ref List<long> reqList, QuestChallenge[] challenges) {
			long kr = 1;
			for(int ki = 0; ki < 6 && kr > 0; ki++) {
				kr = (long)challenges[0].getReq(ki);
				if(kr != 0)
					reqList.Add(kr);
			}
			kr = 1;
			for(int ki = 0; ki < 6 && kr > 0; ki++) {
				kr = (long)challenges[challenges.Length - 1].getReq(ki);
				if(kr != 0)
					reqList.Add(kr);
			}
			if(challenges.Length > 12) {
				bool foundReq = false;
				int count = 0;
				do {
					int r = questRand.Next(challenges.Length - 4) + 2;
					kr = 1;
					//Debug.Log(seed + " (" + count + ")    > " + challenges[r].type.desc);
					for(int ki = 0; ki < 6 && kr > 0; ki++) {
						kr = (long)challenges[r].getReq(ki);
						if(kr != 0) {
							foundReq = true;
							reqList.Add(kr);
						}
					}
					count++;
				} while(!foundReq && count < 3);
			}
			if(reqList.Count > 6) {
				while(reqList.Count > 6) {
					reqList.RemoveAt(0);
					if((reqList.Count > 6)) {
						reqList.RemoveAt(reqList.Count - 1);
					}
				};
			}
		}

		private class IntWrapper {
			public long KRval;
			public int count;
		}
		private void getBetterRequirements(ref List<long> reqList, QuestChallenge[] challenges) {
			long kr = 1;
			Dictionary<long, IntWrapper> counts = new Dictionary<long, IntWrapper>();
			int q = 0;
			foreach(QuestChallenge chall in challenges) {
				kr = 1;
				for(int ki = 0; ki < 6 && kr > 0; ki++) {
					kr = (long)chall.getReq(ki);
					if(kr != 0) {
						IntWrapper v;
						if(!counts.TryGetValue(kr, out v)) {
							v = new IntWrapper();
							v.KRval = kr;
							v.count = 0;
							counts.Add(kr, v);
						}
						v.count++;
					}
					kr = (long)chall.getAltReq(ki);
					if(kr != 0) {
						IntWrapper v;
						if(!counts.TryGetValue(kr, out v)) {
							v = new IntWrapper();
							v.KRval = kr;
							v.count = 0;
							counts.Add(kr, v);
						}
						v.count++;
					}
				}
				q++;
			}
			List<IntWrapper> wrappers = new List<IntWrapper>();
			foreach(IntWrapper count in counts.Values) {
				if(count.count > 1)
					wrappers.Add(count);
			}
			wrappers.Sort((x, y) => y.count.CompareTo(x.count));
			for(int i = 0; i < 6 && i < wrappers.Count; i++) {
				reqList.Add(wrappers[i].KRval);
			}
			if(reqList.Count < 3) {
				getBasicRequirements(ref reqList, challenges);
			}
		}

		public long getReq(int r) {
			//Debug.Log((RequirementType)knownRequirements[r] + " " + Main.BitNum(knownRequirements[r]) + "(" + knownRequirements[r] + ")");
			return knownRequirements[r];
		}

		public long getCurrentReq(int v) {
			if(questStep >= _obstacles.Length) return 0;
			return (long)_obstacles[questStep].getReq(v);
		}

		public bool testAgility(int mod) {
			return testAttribute(AGL + mod, RequirementType.AGILITY);
		}

		public bool testStrength(int mod) {
			return testAttribute(STR + mod, RequirementType.STRENGTH);
		}

		public bool testCharisma(int mod) {
			return testAttribute(CHA + mod, RequirementType.CHARISMA);
		}

		internal bool testIntelligence(int mod) {
			return testAttribute(INT + mod, RequirementType.INTELLIGENCE);
		}

		protected bool testAttribute(int value, RequirementType type) {
			bool baseVal = questRand.Next(20) <= value;
			if(!baseVal && type > 0) {
				if(doesHeroHave(type, questRand.Next(3)==0)) {
					return true;
				}
			}
			return baseVal;
		}

		protected bool testAttribute(int value) {
			return questRand.Next(20) <= value;
		}

		public int testLuck(int v) {
			return questRand.Next(v);
		}

		public int raiseStrength(int v) {
			STR += v;
			return STR;
		}

		public int raiseAgility(int v) {
			AGL += v;
			return AGL;
		}

		public int raiseCharisma(int v) {
			CHA += v;
			return CHA;
		}

		public int raiseIntelligence(int v) {
			INT += v;
			return INT;
		}

		public ItemStack getRandomItem() {
			if(inventory.Count > 0) {
				return inventory[questRand.Next(inventory.Count)];
			}
			return null;
		}

		public EnumResult doQuestStep(float t) {
			EnumResult result;
			if(t > 1) {
				do {
					float v = Mathf.Min(0.1f, t);
					t -= v;
					result = _doQuestStep(v);
				} while(result == EnumResult.CONTINUE && t > 0);
				return result;
			}
			else {
				return _doQuestStep(t);
			}
		}

		private EnumResult _doQuestStep(float t) {
			if(questComplete) {
				timeAftercomplete += t;
				return EnumResult.CONTINUE;
			}
			questTotalTime -= (1.5f * t);
			if(questTimer > 0) {
				questTimer -= t;
				return EnumResult.CONTINUE;
			}
			int initQuestStep = questStep;
			if(questStep >= obstacles.Length || questStep < 0) {
				Debug.Log(this.heroName);
				foreach(QuestChallenge o in obstacles) {
					Debug.Log("  " + o.type.name);
				}
			}
			QuestChallenge ob = obstacles[questStep];
			List<ItemStack> used = new List<ItemStack>();
			while(questTotalTime <= 0 && doesHeroHave(RequirementType.MANA)) {
				if(doesHeroHave(AidType.MANA_LARGE, ref used)) {
					questTotalTime += 360;
				}
				else if(doesHeroHave(AidType.MANA_MEDIUM, ref used)) {
					questTotalTime += 270;
				}
				else if(doesHeroHave(AidType.MANA_SMALL, ref used)) {
					questTotalTime += 180;
				}
				else if(doesHeroHave(AidType.MANA_TINY, ref used)) {
					questTotalTime += 90;
				}
				else {
					break;
				}
			}
			int healing = 0;
			foreach(ItemStack st in used) {
				healing += (st.doesStackHave(AidType.HEALING_TINY) ? 5 : 0);
				healing += (st.doesStackHave(AidType.HEALING_SMALL) ? 7 : 0);
				healing += (st.doesStackHave(AidType.HEALING_MEDIUM) ? 15 : 0);
				healing += (st.doesStackHave(AidType.HEALING_LARGE) ? 25 : 0);
			}
			heal(healing);
			if(questTotalTime <= 0 || heroCurHealth <= 0) {
				//hero dies
				Debug.Log("FAIL " + heroName + ": " + ob.type.name + " | " + questTotalTime + ", " + heroCurHealth);
				object corwrap;
				int cor = 0;
				if(miscData != null && miscData.TryGetValue("cursed_corruption", out corwrap)) {
					cor = (int)corwrap;
					Debug.Log("Corrupted: " + doesHeroHave(RequirementType.SPELL_RESIST) + ", "  + cor);
				}
				Main.instance.player.getActiveDeepGoal().onFailedQuest(this);
				//Debug.Log("     " + obstacles[questStep-1].type.name);
				return EnumResult.FAIL;
			}

			//Debug.Log("Hero is " + ob.type.desc);

			int fails = 0;
			int partials = 0;

			RequirementType lastType = 0;
			ItemStack lastStack = null;
			ItemStack altStack = null;
			//RequirementType statPot = RequirementType.AGILITY | RequirementType.STRENGTH | RequirementType.CHARISMA | RequirementType.INTELLIGENCE;
			foreach(RequireWrapper rw in ob.type.requirements) {
				bool foundAlt = false;
				bool altIsConsumed = true;
				foreach(ItemStack stack in inventory) {
					//forces requiring two *different* stacks for same-type requirements if not consumable
					if(lastType == rw.req && (stack == lastStack /*&& stack.item.isConsumable*/)) continue;
					lastStack = stack;
					if(stack.item.hasReqType(rw.req) && stack.stackSize > 0) {
						if(stack.item.isConsumable)
							stack.stackSize--;
						stack.onUsedDuringQuest(this);
						goto ObsList;
					}
					if(stack.item.hasReqType(rw.alt) && stack.stackSize > 0) {
						foundAlt = true; //alt items are ok, but we'd rather find the required one
						altStack = stack;
						altIsConsumed = rw.req != 0;
					}
				}
				//this looks so janky
				//if the loop is exited normally, inc failure or partials
				if(!foundAlt) {
					fails++;
				}
				else {
					//decrement used alt-type stack, as it was used
					if(altStack.item.isConsumable && altIsConsumed)
						altStack.stackSize--;
					altStack.onUsedDuringQuest(this);
					partials++;
				}
				//go here if success
				ObsList:
				//then check next requirement
				;
				lastType = rw.req;
			}

			EnumResult result = ob.MakeAttempt(this, fails, partials);

			if(result == EnumResult.CRIT_FAIL && this.doesHeroHave(AidType.RETRY_FAILURE)) {
				result = ob.MakeAttempt(this, fails, partials);
			}
			ob.OnAttempt(result, this);

			questTimer += 60;
			questStep++;

			if(questStep > initQuestStep) {
				heal(5);
			}
			inventory.RemoveAll(x => x.stackSize <= 0);
			if(questStep >= obstacles.Length) {
				if(result < EnumResult.MIXED && ob.type is IQuestGoal) {
					//rare ending
					Debug.Log("QUEST FAILURE " + ob.type.name + "|" + questTotalTime + "," + heroCurHealth);
					Main.instance.player.getActiveDeepGoal().onFailedQuest(this);
					return EnumResult.FAIL;
				}
				Debug.Log("SUCCESS " + ob.type.name + "|" + questTotalTime + "," + heroCurHealth);
				Main.instance.player.getActiveDeepGoal().onSuccessfulQuest(this);
				return EnumResult.SUCCESS;
			}
			return EnumResult.CONTINUE;
		}

		public ItemStack determineRelic() {
			List<ItemStack> candidates = new List<ItemStack>();
			foreach(ItemStack stack in inventory) {
				if(stack.numTimesUsedOnQuest > 0 && stack.item.isViableRelic && !stack.item.isConsumable) {
					candidates.Add(stack);
				}
			}
			if(candidates.Count == 0) {
				foreach(ItemStack stack in inventory) {
					if(!stack.item.isConsumable) {
						candidates.Add(stack);
					}
				}
			}
			return candidates.OrderByDescending(o => o.numTimesUsedOnQuest).FirstOrDefault();
		}

		public void addItemToInventory(ItemStack stack) {
			inventory.Add(stack);
		}

		public void removeItemFromInventory(ItemStack stack) {
			inventory.Remove(stack);
		}

		public void addSubTask(QuestChallenge newTask) {
			addSubTask(newTask, false);
		}

		public void addSubTask(QuestChallenge newTask, bool immediate) {
			List<QuestChallenge> list = obstacles.ToList<QuestChallenge>();
			list.Insert(questStep+(immediate?0:1), newTask);
			_obstacles = list.ToArray();
		}

		public void harmHero(int amt, DamageType damage) {
			harmHero(amt, damage, false);
		}

		public void harmHero(int amt, DamageType damage, bool isMagic) {
			if(amt <= 0) return;
			if(!damage.getBypassesArmor()) {
				float reduction = 0;

				float bestArmorV = 0, bestShieldV = 0, bestMagicV = 0;
				ItemStack bestArmor = null, bestShield = null, bestMagic = null;

				foreach(ItemStack stack in inventory) {
					float ar = getArmorValueFor(stack, EnumRestriction.ARMOR);
					if(ar > bestArmorV) {
						bestArmorV = ar;
						bestArmor = stack;
					}
					ar = getArmorValueFor(stack, EnumRestriction.SHIELD);
					if(ar > bestShieldV) {
						bestShieldV = ar;
						bestShield = stack;
					}
					ar = getArmorValueFor(stack, EnumRestriction.MAGIC);
					if(ar > bestMagicV) {
						bestMagicV = ar;
						bestMagic = stack;
					}
				}
				
				if(bestArmor != null) {
					if(bestArmor.item.isConsumable) bestArmor.stackSize--;
					bestArmorV *= bestArmor.getEffectiveness(RequirementType.ARMOR);
				}
				if(bestShield != null) {
					if(bestShield.item.isConsumable) bestShield.stackSize--;
					bestShieldV *= bestShield.getEffectiveness(RequirementType.ARMOR);
				}
				if(bestMagic != null) {
					if(bestMagic.item.isConsumable) bestMagic.stackSize--;
					bestMagicV *= bestMagic.getEffectiveness(RequirementType.ARMOR);
				}

				reduction = bestArmorV + bestShieldV + bestMagicV;
				if((damage.isMagical() || isMagic) && this.doesHeroHave(RequirementType.COUNTERSPELL)) {
					reduction = Math.Max(reduction, 0.4f) * 1.2f;
				}
				amt -= Mathf.FloorToInt(amt * reduction);
			}
			if(damage.getImmunityType() != 0) {
				float best = 0;
				ItemStack bestStack = null;
				foreach(ItemStack stack in inventory) {
					float f = stack.getEffectiveness(damage.getImmunityType());
					if(f > best && stack.stackSize > 0) {
						best = f;
						bestStack = stack;
					}
				}

				amt -= Mathf.FloorToInt(amt * best);
				if(bestStack != null && bestStack.item.isConsumable) {
					bestStack.stackSize--;
				}
			}
			if(amt > 0) {
				heroCurHealth -= amt;
				tryToHeal();
			}
			inventory.RemoveAll(x => x.stackSize == 0);
		}
		[Flags]
		protected enum EnumRestriction {
			ARMOR = (1 << 1),
			SHIELD = (1 << 2),
			MAGIC = (1 << 3)
		}

		protected float getArmorValueFor(ItemStack stack, EnumRestriction restrict) {
			float enhance = 0;
			if(stack.doesStackHave(Enchantments.ENHANCEMENT)) {
				foreach(Enchantment en in stack.enchants) {
					if(en == Enchantments.ENHANCEMENT)
						enhance += 0.05f;
				}
			}
			AidType p = stack.getAllAids();
			float best = 0;
			for(int i = 1; i < 32; i*=2) {
				float a = getArmorValue((p & (AidType)i), restrict);
				if(a > best) {
					best = a;
				}
			}
			if((restrict ^ EnumRestriction.MAGIC) == 0) {
				enhance = 0;
			}
			if((restrict ^ EnumRestriction.SHIELD) == 0) {
				enhance /= 5;
			}
			return ((best > 0)?enhance + best:0);
		}

		protected float getArmorValue(AidType type, EnumRestriction restrict) {
			switch(type) {
				case AidType.HEAVY_ARMOR:
					return ((restrict & EnumRestriction.ARMOR) > 0 ? 0.3f : 0);
				case AidType.MEDIUM_ARMOR:
					return ((restrict & EnumRestriction.ARMOR) > 0 ? 0.2f : 0);
				case AidType.LIGHT_ARMOR:
					return ((restrict & EnumRestriction.ARMOR) > 0 ? 0.1f : 0);
				case AidType.LIGHT_SHIELD:
					return ((restrict & EnumRestriction.SHIELD) > 0 ? 0.05f : 0);
				case AidType.HEAVY_SHIELD:
					return ((restrict & EnumRestriction.SHIELD) > 0 ? 0.1f : 0);
				case AidType.BARKSKIN:
					return ((restrict & EnumRestriction.MAGIC) > 0 ? 0.1f : 0);
			}
			return 0;
		}

		public bool doesHeroHave(AidType aid) {
			return doesHeroHave(aid, true);
		}
		public bool doesHeroHave(AidType aid, bool consume) {
			List<ItemStack> used = null;
			return doesHeroHave(aid, true, ref used);
		}

		public bool doesHeroHave(AidType aid, ref List<ItemStack> used) {
			return doesHeroHave(aid, true, ref used);
		}
		public bool doesHeroHave(AidType aid, bool consume, ref List<ItemStack> used) {
			foreach(ItemStack stack in inventory) {
				if(stack.doesStackHave(aid)) {
					if(used != null) used.Add(stack);
					if(consume && stack.item.isConsumable && stack.stackSize > 0) {
						stack.stackSize--;
					}
					else {
						stack.numTimesUsedOnQuest++;
						stack.onUsedDuringQuest(this);
						if(_obstacles[this.questStep] is IQuestGoal) {
							stack.numTimesUsedOnQuest += 2;
						}
					}
					return true;
				}
			}
			return false;
		}
		public ItemStack getHeroItemWith(Enchantment ench) {
			foreach(ItemStack stack in inventory) {
				if(stack.doesStackHave(ench) && stack.stackSize > 0) {
					return stack;
				}
			}
			return null;
		}

		public ItemStack getHeroItemWith(AidType aid) {
			foreach(ItemStack stack in inventory) {
				if(stack.doesStackHave(aid) && stack.stackSize > 0) {
					return stack;
				}
			}
			return null;
		}

		public ItemStack getHeroItemWith(RequirementType req) {
			foreach(ItemStack stack in inventory) {
				if(stack.doesStackHave(req) && stack.stackSize > 0) {
					return stack;
				}
			}
			return null;
		}

		public bool doesHeroHave(RequirementType req) {
			return doesHeroHave(req, true);
		}
		public bool doesHeroHave(RequirementType req, bool consume) {
			foreach(ItemStack stack in inventory) {
				if(stack.doesStackHave(req)) {
					if(consume && stack.item.isConsumable && stack.stackSize > 0) {
						stack.stackSize--;
					}
					else {
						stack.numTimesUsedOnQuest++;
						stack.onUsedDuringQuest(this);
						if(_obstacles[this.questStep] is IQuestGoal) {
							stack.numTimesUsedOnQuest+=2;
						}
					}
					return true;
				}
			}
			return false;
		}

		private void tryToHeal() {
			int needed = heroMaxHealth - heroCurHealth;
			List<ItemStack> used = new List<ItemStack>();
			do {
				if(needed >= 100)	 needed -= doesHeroHave(AidType.RESSURECTION, ref used)  ? heal(50) : heal(-needed);
				else if(needed >= 50) needed -= doesHeroHave(AidType.HEALING_LARGE, ref used)  ? heal(25) : heal(-20);
				else if(needed >= 20) needed -= doesHeroHave(AidType.HEALING_MEDIUM, ref used) ? heal(15) : heal(-10);
				else if(needed >= 10) needed -= doesHeroHave(AidType.HEALING_SMALL, ref used)  ? heal(7) : heal(-5);
				else if(needed > 0)  needed -= doesHeroHave(AidType.HEALING_TINY, ref used)   ? heal(5) : heal(-5);
				else {
					int stamina = 0;
					foreach(ItemStack st in used) {
						stamina += (st.doesStackHave(AidType.MANA_TINY) ? 90 : 0);
						stamina += (st.doesStackHave(AidType.MANA_SMALL) ? 180 : 0);
						stamina += (st.doesStackHave(AidType.MANA_MEDIUM) ? 270 : 0);
						stamina += (st.doesStackHave(AidType.MANA_LARGE) ? 360 : 0);
					}
					questTotalTime += stamina;
					return;
				}
			} while(true);
		}

		public int heal(int amt) {
			if(amt > 0) heroCurHealth += amt;
			else if(amt < 0) amt *= -1;
			heroCurHealth = Mathf.Clamp(heroCurHealth, 0, heroMaxHealth);
			return amt;
		}

		public void repeatTask() {
			questStep--;
		}

		public void addTime(int v) {
			questTimer += v;
		}

		public void hastenQuestEnding(int v) {
			questTotalTime -= v;
		}

		public void questStarted() {
			int high = 0;
			foreach(ItemStack stack in inventory) {
				if(stack.antiquity > high) {
					high = stack.antiquity;
				}
			}
			if(inventory.Count <= 0) {
				high -= 2;
			}
			foreach(QuestChallenge obs in _obstacles) {
				//every challenge is given a luck factor based on the antiquity of the best relic
				//and penalized by 2 for having no equipment at all

				//0     -> 0
				//1     -> 1
				//2-4   -> 2
				//5-12  -> 3
				//13-33 -> 4
				//34-90 -> 5

				obs.questBonus += high > 0 ? Mathf.RoundToInt(Mathf.Log(high) + 1) : high;
			}
			//Debug.Log(originalGoal);
		}

		public string getOriginalGoal() {
			if(originalGoal == null) return "null";
			return originalGoal.name;
		}

		public float QuestTimeLeft() {
			return questTotalTime;
		}

		public float GetCompletion() {
			return (float)questStep / _obstacles.Length;
		}

		public ObstacleType getGoal() {
			return originalGoal;
		}

		public bool isActive() {
			if(timeAftercomplete < 150) { //2.5 minutes
				return true;
			}
			return false;
		}

		internal string getStatus() {
			if(questStep >= _obstacles.Length) {
				if(finalResult >= EnumResult.MIXED)
					return "basking in glory";
				return "bemoaning failure";
			}
			if(heroCurHealth <= 0 && questComplete) {
				return "bleeding out";
			}
			if(questTotalTime <= 0 && questComplete) {
				return "wallowing in defeat";
			}
			return _obstacles[questStep].type.desc;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			/*public readonly List<ItemStack> inventory;*/
			info.AddValue("heroMaxHealth", heroMaxHealth);
			info.AddValue("heroCurHealth", heroCurHealth);
			info.AddValue("heroName", heroName);
			info.AddValue("STR", STR);
			info.AddValue("AGL", AGL);
			info.AddValue("CHA", CHA);
			info.AddValue("INT", INT);
			info.AddValue("questTimer", questTimer);
			info.AddValue("questTotalTime", questTotalTime);
			info.AddValue("questStep", questStep);
			info.AddValue("numObstacles", _obstacles.Length);
			for(int o = 0; o < _obstacles.Length; o++) {
				info.AddValue("obs_"+o, _obstacles[o], typeof(QuestChallenge));
			}
			info.AddValue("originalGoal", originalGoal.name);
			//inventory
			info.AddValue("inventorySize", inventory.Count);
			for(int i = 0; i < inventory.Count; i++) {
				info.AddValue("inven_" + i, inventory[i], typeof(ItemStack));
			}
			info.AddValue("timeUntilQuestExpires", timeUntilQuestExpires);
			info.AddValue("numQuestsBefore", numQuestsBefore);
			info.AddValue("reward_0", rewards[0]);
			info.AddValue("reward_1", rewards[1]);
			info.AddValue("hasMiscData", miscData != null);
			if(miscData != null)
				info.AddValue("miscData", miscData, typeof(Dictionary<string, object>));
			info.AddValue("finalResult", (int)finalResult);
		}

		public Quest(SerializationInfo info, StreamingContext context) {
			questRand = new System.Random();
			heroMaxHealth = info.GetInt32("heroMaxHealth");
			heroCurHealth = info.GetInt32("heroCurHealth");
			heroName = info.GetString("heroName");
			STR = info.GetInt32("STR");
			AGL = info.GetInt32("AGL");
			CHA = info.GetInt32("CHA");
			INT = info.GetInt32("INT");
			questTimer = (float)info.GetDouble("questTimer");
			questTotalTime = (float)info.GetDouble("questTotalTime");
			questStep = info.GetInt32("questStep");
			int num = info.GetInt32("numObstacles");
			questComplete = questStep >= num;
			_obstacles = new QuestChallenge[num];
			for(int o = 0; o < num; o++) {
				QuestChallenge temp = (QuestChallenge)info.GetValue("obs_" + o, typeof(QuestChallenge));
				//_obstacles[o] = (QuestChallenge)info.GetValue("obs_" + o, typeof(QuestChallenge));
				fromDisk.Add(new ChallengeLoadWrapper(temp));
			}
			originalGoal = GameRegistry.GetObstacleByID(info.GetString("originalGoal"));
			num = info.GetInt32("inventorySize");
			inventory = new List<ItemStack>();
			for(int o = 0; o < num; o++) {
				inventory.Add((ItemStack)info.GetValue("inven_" + o, typeof(ItemStack)));
			}
			timeUntilQuestExpires = (float)info.GetDouble("timeUntilQuestExpires");
			numQuestsBefore = info.GetInt64("numQuestsBefore");

			rewards = new ItemStack[2];
			rewards[0] = (ItemStack)info.GetValue("reward_0", typeof(ItemStack));
			rewards[1] = (ItemStack)info.GetValue("reward_1", typeof(ItemStack));
			if(Main.saveVersionFromDisk >= 9) {
				if(info.GetBoolean("hasMiscData")) {
					miscData = (Dictionary<string, object>)info.GetValue("miscData", typeof(Dictionary<string, object>));
				}
			}
			if(Main.saveVersionFromDisk >= 16) {
				finalResult = (EnumResult)info.GetInt32("finalResult");
			}
		}

		private List<ChallengeLoadWrapper> fromDisk = new List<ChallengeLoadWrapper>();

		public void FinishLoad() {
			for(int o = 0; o < fromDisk.Count; o++) {
				_obstacles[o] = fromDisk[o].challenge;
				//Debug.Log("Ob Type:  " + _obstacles[o].type);
				//Debug.Log("Req list: " + _obstacles[o].type.requirements);
				//_obstacles[o].FinishLoad();
			}
			if(_obstacles.Length > 0) {
				knownRequirements = new long[6];
				knownRequirements[0] = (long)_obstacles[0].getReq(0);
				knownRequirements[1] = (long)_obstacles[0].getReq(1);
				knownRequirements[2] = (long)_obstacles[0].getReq(2);
				knownRequirements[3] = (long)_obstacles[_obstacles.Length - 1].getReq(0);
				knownRequirements[4] = (long)_obstacles[_obstacles.Length - 1].getReq(1);
				knownRequirements[5] = (long)_obstacles[_obstacles.Length - 1].getReq(2);
				Array.Sort(knownRequirements);
				Array.Reverse(knownRequirements);
			}
		}

		private class ChallengeLoadWrapper {
			public readonly QuestChallenge challenge;
			public ChallengeLoadWrapper(QuestChallenge chal) {
				challenge = chal;
			}
		}
	}
}
