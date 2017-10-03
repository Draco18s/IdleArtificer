using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.quests.hero;
using Assets.draco18s.artificer.quests.requirement;
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
			FantasyName[] names = fantasyNameGenerator.GetFantasyNames(1);
			return GenerateNewQuest(ChallengeTypes.Goals.getRandom(rand), names[0].FirstName + " " + names[0].Land);
		}

		public static Quest GenerateNewQuest(string heroName) {
			return GenerateNewQuest(ChallengeTypes.Goals.getRandom(rand), heroName);
		}

		public static Quest GenerateNewQuest(ObstacleType withGoal, string heroName) {
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
					arr.Add(new QuestChallenge(ChallengeTypes.General.getRandom(rand), 0));
				}
			}
			//end
			arr.Add(goal);

			//TODO: make reward items scale
			QuestChallenge[] list = arr.ToArray();
			Quest q = new Quest(list[list.Length-1].type.numOfTypeCompleted, rand.Next(), list);
			Item i = Items.getRandom(rand, 0, 4);
			q.rewards[0] = new ItemStack(i, rand.Next(i.maxStackSize - i.minStackSize) + i.minStackSize);
			i = Items.getRandom(rand, 4, 7);
			q.rewards[1] = new ItemStack(i, rand.Next(i.maxStackSize - i.minStackSize) + i.minStackSize);

			q.heroName = heroName;

			//Name debugging
			//string debugName = names[0].FirstName + " " + names[0].LastName + " " + names[0].Postfix;
			//Debug.Log(debugName);

			return q;
		}

		public QuestChallenge[] obstacles {
			get {
				return _obstacles;
			}
		}
		public GameObject guiItem;
		protected long[] knownRequirements;

		public string heroName;
		public readonly long numQuestsBefore;
		public readonly int heroMaxHealth;
		public readonly List<ItemStack> inventory;
		public readonly ItemStack[] rewards;

		public int heroCurHealth;
		public bool questComplete = false;
		public float timeUntilQuestExpires;
		public readonly System.Random questRand;
		
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
			heroMaxHealth = 100;
			heroCurHealth = heroMaxHealth;
			inventory = new List<ItemStack>();
			questStep = 0;
			questTimer = 60;
			questTotalTime = 0;
			questRand = new System.Random(seed);
			HarrowDeck deck = new HarrowDeck();
			STR = deck.STR.tokens;
			AGL = deck.AGL.tokens;
			INT = deck.INT.tokens;
			CHA = deck.CHA.tokens;
			if(challenges.Length > 0) {
				long kr = 1;
				List<long> reqList = new List<long>();
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

		public long getReq(int r) {
			//Debug.Log((RequirementType)knownRequirements[r] + " " + Main.BitNum(knownRequirements[r]) + "(" + knownRequirements[r] + ")");
			return knownRequirements[r];
		}

		public long getCurrentReq(int v) {
			if(questStep >= _obstacles.Length) return 0;
			return (long)_obstacles[questStep].getReq(v);
		}

		public bool testAgility(int mod) {
			return testAttribute(AGL+mod);
		}

		public bool testStrength(int mod) {
			return testAttribute(STR+mod);
		}

		public bool testCharisma(int mod) {
			return testAttribute(CHA + mod);
		}

		internal bool testIntelligence(int mod) {
			return testAttribute(INT + mod);
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
			questTotalTime += (2.5f * t);
			if(questTimer > 0) {
				questTimer -= t;
				return EnumResult.CONTINUE;
			}
			if(questStep >= obstacles.Length || questStep < 0) {
				Debug.Log(this.heroName);
				foreach(QuestChallenge o in obstacles) {
					Debug.Log("  " + o.type.name);
				}
			}
			QuestChallenge ob = obstacles[questStep];
			if(questTotalTime > 2000 || heroCurHealth <= 0) {
				//hero dies
				Debug.Log("FAIL " + ob.type.name + "|" + questTotalTime + "," + heroCurHealth);
				//Debug.Log("     " + obstacles[questStep-1].type.name);
				return EnumResult.FAIL;
			}

			//Debug.Log("Hero is " + ob.type.desc);

			int fails = 0;
			int partials = 0;

			RequirementType lastType = 0;
			ItemStack lastStack = null;
			ItemStack altStack = null;
			foreach(RequireWrapper rw in ob.type.requirements) {
				bool foundAlt = false;
				foreach(ItemStack stack in inventory) {
					//forces requiring two *different* stacks for same-type requirements if not consumable
					if(lastType == rw.req && (stack == lastStack /*&& stack.item.isConsumable*/)) continue;
					lastStack = stack;
					if(stack.item.hasReqType(rw.req) && stack.stackSize > 0) {
						if(stack.item.isConsumable)
							stack.stackSize--;
						goto ObsList;
					}
					if(stack.item.hasReqType(rw.alt) && stack.stackSize > 0) {
						foundAlt = true; //alt items are ok, but we'd rather find the required one
						altStack = stack;
					}
				}
				//this looks so janky
				//if the loop is exited normally, inc failure or partials
				if(!foundAlt) {
					fails++;
				}
				else {
					//decrement used alt-type stack, as it was used
					if(altStack.item.isConsumable)
						altStack.stackSize--;
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



			if(questStep >= obstacles.Length) {
				if(result < EnumResult.MIXED) {
					//rare ending
					Debug.Log("FAILURE " + ob.type.name + "|" + questTotalTime + "," + heroCurHealth);
					return EnumResult.FAIL;
				}
				Debug.Log("SUCCESS " + ob.type.name + "|" + questTotalTime + "," + heroCurHealth);
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
			List<QuestChallenge> list = obstacles.ToList<QuestChallenge>();
			list.Insert(questStep+1, newTask);
			_obstacles = list.ToArray();
		}

		public void harmHero(int amt, DamageType damage) {
			amt = 0;
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
			foreach(ItemStack stack in inventory) {
				if(stack.doesStackHave(aid)) {
					if(consume && stack.item.isConsumable && stack.stackSize > 0) {
						stack.stackSize--;
					}
					else {
						stack.numTimesUsedOnQuest++;
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
			do {
				if(needed >= 100)	 needed -= doesHeroHave(AidType.RESSURECTION)  ? heal(100) : heal(-needed);
				else if(needed >= 50) needed -= doesHeroHave(AidType.HEALING_LARGE)  ? heal(50) : heal(-30);
				else if(needed >= 20) needed -= doesHeroHave(AidType.HEALING_MEDIUM) ? heal(20) : heal(-10);
				else if(needed >= 10) needed -= doesHeroHave(AidType.HEALING_SMALL)  ? heal(10) : heal(-5);
				else if(needed > 0)  needed -= doesHeroHave(AidType.HEALING_TINY)   ? heal(5) : heal(-5);
				else return;
			} while(true);
		}

		private int heal(int amt) {
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
			if(doesHeroHave(AidType.MANA_LARGE) && v >= 120) {
				v = Math.Max(v - 120, 0);
			}
			else if(doesHeroHave(AidType.MANA_MEDIUM) && v >= 90) {
				v = Math.Max(v - 90, 0);
			}
			else if(doesHeroHave(AidType.MANA_SMALL) && v >= 60) {
				v = Math.Max(v - 60, 0);
			}
			else if(doesHeroHave(AidType.MANA_TINY) && v >= 30) {
				v = Math.Max(v - 30, 0);
			}
			questTotalTime += v;
		}

		public void questStarted() {
			int high = 0;
			foreach(ItemStack stack in inventory) {
				if(stack.antiquity > high) {
					high = stack.antiquity;
				}
			}
			if(inventory.Count <= 0) {
				high--;
			}
			foreach(QuestChallenge obs in _obstacles) {
				//every challenge is given a luck factor based on the antiquity of the best relic
				//and penalized by 1 for having no equipment at all
				obs.questBonus += high;
			}
		}

		public float QuestTimeLeft() {
			return 2000-questTotalTime;
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
				return "basking in glory";
			}
			if(heroCurHealth <= 0) {
				return "bleeding out";
			}
			if(questTotalTime > 2000) {
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
			questComplete = questStep >= num;
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
