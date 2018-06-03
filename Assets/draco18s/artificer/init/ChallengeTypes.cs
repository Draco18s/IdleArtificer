//using UnityEngine;
using System.Collections;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Reflection;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.util;
using Assets.draco18s.artificer.upgrades;

namespace Assets.draco18s.artificer.init {
	public static class ChallengeTypes {

		public static class Goals {
			#region easy
			//Adding more quests requires increasing maxQuestDifficulty somewhere
			//wood, leather, armor, weapon, torches--all basic stuff
			public static ObstacleType DELIVERY = new GoalDelivery().setRewardScalar(1).setReqScalar(0.2f);//nothing at all (just don't get theft'd)
			public static ObstacleType FIND_COWS = new GoalFindCows().setRewardScalar(1).setReqScalar(0.5f);//mana
			public static ObstacleType REPAIR_DAMN = new GoalRepairDam().setRewardScalar(1).setReqScalar(1.5f);//wood
			public static ObstacleType SCARY_CAVE = new GoalExploreCave().setRewardScalar(1).setReqScalar(0.75f);//torches
			public static ObstacleType PATROLS = new GoalPatrolDuty().setRewardScalar(1);//light + healing
			public static ObstacleType ESTABLISH_HOME = new GoalBuildHome().setRewardScalar(1).setReqScalar(1.5f);//wood + leather
			public static ObstacleType TRAINING = new GoalCombatTraining().setRewardScalar(1);//weapon + armor
			public static ObstacleType TARGET_PRACTICE = new GoalTargetPractice().setRewardScalar(1);//ranged weapon + (agl)
			#endregion

			#region moderate
			//Adding more quests requires increasing maxQuestDifficulty somewhere
			//can be done with relatively simple products, but has semi-optional higher teir requirement
			public static ObstacleType RAT_INFESTATION = new GoalClearRatInfestation().setRewardScalar(2);//poison
			public static ObstacleType PLANT_GARDEN = new GoalPlantGarden().setRewardScalar(2).setReqScalar(1.5f);//herbs*3
			public static ObstacleType ALCHEMY_LAB = new GoalAlchemyLab().setRewardScalar(2).setReqScalar(1.5f);//herb + healing + mana + poison
			public static ObstacleType TOWN_INFRASTRUCTURE = new GoalUpgradeTown().setRewardScalar(2).setReqScalar(2f);//wood + iron
			public static ObstacleType GUARD_TEMPLE = new GoalGuardTemple().setRewardScalar(2).setReqScalar(0.75f);//armor + detection
			public static ObstacleType WOLF_PACK = new GoalWolves().setRewardScalar(2);//poison, weapon, armor
			#endregion

			#region semi-advanced
			//Adding more quests requires increasing maxQuestDifficulty somewhere
			//require several basic or mid-level items
			public static ObstacleType RECRUIT = new GoalRecruitAllies().setRewardScalar(4).setReqScalar(2f);//cha
			public static ObstacleType DEFEND_VILLAGE = new GoalDefendVillage().setRewardScalar(4); //armor*2 + weapon*2
			public static ObstacleType DUKE = new GoalPetitionTheDuke().setRewardScalar(4).setReqScalar(2f);//cha
			public static ObstacleType SPHINX = new GoalSphynxRiddles().setRewardScalar(4).setReqScalar(2f);//int + mana
			public static ObstacleType INFILTRATE_CASTLE = new GoalInfiltrateCastle().setRewardScalar(4);//stealth + agl
			public static ObstacleType OBSERVE_ENEMY = new GoalObserveTroopMovements().setRewardScalar(4);//stealth + int
			#endregion

			#region advanced
			//Adding more quests requires increasing maxQuestDifficulty somewhere
			//require high-level potions
			public static ObstacleType MAYOR = new GoalRunForOffice().setRewardScalar(4).setReqScalar(2f);//cha + firm resolve
			public static ObstacleType CLIMB_MOUNTAIN = new GoalReachSummit().setRewardScalar(8); //endurance + featherfall
			public static ObstacleType SETTLE_TOWN = new GoalSettleTown().setRewardScalar(8).setReqScalar(16);//tons o' resources
			public static ObstacleType EXPLORE_TOMB = new GoalExploreTomb().setRewardScalar(8).setReqScalar(1.5f);//danger sense + fire damage
			public static ObstacleType DRAGON = new GoalKillDragon().setRewardScalar(8).setReqScalar(1.5f);//fire immunity + cold damage
			#endregion
			#region very advanced
			//Adding more quests requires increasing maxQuestDifficulty somewhere
			//all require some kind of enchantment
			public static ObstacleType FREE_SLAVES = new GoalFreeSlaves().setRewardScalar(12);//firm resolve
			public static ObstacleType SKELETON_INFESTATION = new GoalSkeletons().setRewardScalar(12);//disruption
			public static ObstacleType LITCH = new GoalKillLitch().setRewardScalar(12);//spell resist
			public static ObstacleType GORGON = new GoalSlayGorgon().setRewardScalar(12);//mirrored
			public static ObstacleType HYDRA = new GoalBeheadHydra().setRewardScalar(12);//vorpal, free move
			public static ObstacleType EVIL_BARD = new GoalEvilBard().setRewardScalar(12);//negCha pot, mind-shield
			public static ObstacleType ATLANTIS = new GoalFindAtlantis().setRewardScalar(15);//water breathing, freedom of movement
			#endregion

			public static class Bonus {
				public static ObstacleType FALLEN_HERO = new GoalFallenHero().setRewardScalar(12);
				public static ObstacleType KRAKEN = new GoalKillKraken().setRewardScalar(21);
			}

			public static class Sub {
				public static ObstacleType MUMMY = new GoalExploreTomb_Mummy();
				public static ObstacleType SKELETONS = new GoalExploreTomb_Skeletons();
			}

			public static class DeepGoalSpecial {
				public static ObstacleType SOW_CHAOS = new GoalSowChaos().setRewardScalar(3);
				public static ObstacleType HEART_TREE = new GoalHeartTree().setRewardScalar(21);
				public static ObstacleType BURN_FOREST = new GoalClearBackWoods().setRewardScalar(18);
				public static ObstacleType RESCUE_TAKEN = new GoalRescueCaptured().setRewardScalar(21);
				public static ObstacleType CLEANS_CORRUPTION = new GoalCleansCorruption().setRewardScalar(18);

				public static ObstacleType EQUIP_ARMY = new GoalEquipArmy().setRewardScalar(4);
				public static ObstacleType COMBAT = new GoalEngageInWar().setRewardScalar(6);

				public static ObstacleType HUNT_SPIRITS = new GoalHuntSpirits().setRewardScalar(8);
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Goals).GetFields();
				int min = StatisticsTracker.minQuestDifficulty.value;
				int max = StatisticsTracker.maxQuestDifficulty.value;
				if(max >= fields.Length-1) {
					StatisticsTracker.allQuestsUnlocked.setAchieved();
				}
				max = Math.Min(max, fields.Length-1);
				min = Math.Min(min, Math.Max(max - 7,0));
				int v = rand.Next(max - min) + min;
				FieldInfo field = fields[v];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}

		public static class Initial {
			public static ObstacleType AT_HOME = new ObstacleHome();
			//cover of darkness [light]
			public static ObstacleType BUY_EQUIPMENT = new ObstacleBuyEquipment("starting");
			public static ObstacleType TOWN_OUTSKIRTS = new ObstacleOutskirts();
			public static ObstacleType DECYPHER_MAP = new ObstacleDecypherMap();
			public static ObstacleType CHURCHYARD = new ObstacleChurchyard();
			public static ObstacleType EXPLORE_TOWN = new ObstacleExploreTown();
			public static ObstacleType TAVERN = new ObstacleTavern();
			public static ObstacleType DETAINED = new ObstacleDetained();
			public static class Sub {
				public static ObstacleType BAR_BRAWL = new ObstacleDrunkenFight();
			}

			public static class Town {
				public static ObstacleType HARBOR = new ObstacleExploreTown_Harbor();
				public static ObstacleType MARKET = new ObstacleExploreTown_Market();
				public static ObstacleType GARDENS = new ObstacleExploreTown_Gardens();
				public static ObstacleType TEMPLE = new ObstacleExploreTown_Temples();
				public static ObstacleType GRAVEYARD = new ObstacleGraveyard();
				public static ObstacleType SHOPPING = new ObstacleBuyEquipment("shopping");
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Initial).GetFields();

				int min = UnityEngine.Mathf.FloorToInt(StatisticsTracker.minQuestDifficulty.value / 20f * (fields.Length - 1));
				int max = UnityEngine.Mathf.FloorToInt(StatisticsTracker.maxQuestDifficulty.value / 20f * (fields.Length - 1));
				if(min == max) {
					if(min > 0) min--;
					else max++;
				}
				int v = rand.Next(max - min) + min;

				FieldInfo field = fields[rand.Next(v)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
		public static class Travel {
			public static ObstacleType MOUNTAIN_PATH = new ObstacleMountainPath(); //damage
			public static ObstacleType HICH_RIDE = new ObstacleHitchRide();
			public static ObstacleType SAIL_SEAS = new ObstacleSailFriendlySeas();
			public static ObstacleType OPEN_ROAD = new ObstacleOpenRoad();
			public static ObstacleType GOTO_TOWN = new ObstacleTravelToTown();
			public static ObstacleType DARK_FOREST = new ObstacleDarkWoods(); //damage
			public static ObstacleType RIVER = new ObstacleRiver();
			public static ObstacleType SWAMP = new ObstacleSwamp();
			public static ObstacleType WATER_TRANSPORT = new ObstacleWaterTransport();
			public static ObstacleType WEATHER = new ObstacleBadWeather(); //damage
			public static ObstacleType TELEPORT = new ObstacleTeleport();

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Travel).GetFields();
				FieldInfo field = fields[rand.Next(fields.Length)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
		public static class Unexpected {
			public static ObstacleType AMBUSH = new ObstacleAmbush();//damage
			public static ObstacleType LOST = new ObstacleLost();
			public static ObstacleType THIEF = new ObstacleThief();//damage
			public static ObstacleType MADE_MESS = new ObstacleCleanMess();
			public static ObstacleType QUICKSAND = new ObstacleQuicksand();//damage
			public static ObstacleType STEAL_STUFF = new ObstcaleStealSupplies();
			public static ObstacleType PERSUED = new ObstaclePersued();
			public static ObstacleType HEROIC_DUEL = new ObstacleHeroDuel();//damage
			public static ObstacleType BEGGAR = new ObstacleBeggar();

			public static class Sub {
				public static ObstacleType GENIE = new ObstacleGenie();
			}

			public static class Traps {
				public static ObstacleType TRAPPED_PASSAGE_FIRE = new ObstacleTrappedPassage(DamageType.FIRE);
				public static ObstacleType TRAPPED_PASSAGE_COLD = new ObstacleTrappedPassage(DamageType.COLD);
				public static ObstacleType TRAPPED_PASSAGE_ACID = new ObstacleTrappedPassage(DamageType.ACID);
				public static ObstacleType TRAPPED_PASSAGE_POISON = new ObstacleTrappedPassage(DamageType.POISON);
				public static ObstacleType TRAPPED_PASSAGE_GENERIC = new ObstacleTrappedPassage(DamageType.GENERIC);
				public static ObstacleType TRAPPED_PASSAGE_ARROWS = new ObstacleTrappedPassage(DamageType.ARROWS);

				public static ObstacleType MAGIC_TRAP_FIRE = new ObstacleMagicTrap(DamageType.FIRE);
				public static ObstacleType MAGIC_TRAP_COLD = new ObstacleMagicTrap(DamageType.COLD);
				public static ObstacleType MAGIC_TRAP_ACID = new ObstacleMagicTrap(DamageType.ACID);
				public static ObstacleType MAGIC_TRAP_POISON = new ObstacleMagicTrap(DamageType.POISON);
				public static ObstacleType MAGIC_TRAP_HOLY = new ObstacleMagicTrap(DamageType.HOLY);
				public static ObstacleType MAGIC_TRAP_UNHOLY = new ObstacleMagicTrap(DamageType.UNHOLY);
			}
			public static class Monsters {
				public static ObstacleType BANDIT = new ObstacleMonster("a bandit", DamageType.GENERIC, RequirementType.WEAPON);
				public static ObstacleType BRIGAND = new ObstacleMonster("a brigand", DamageType.ARROWS, RequirementType.WEAPON);

				public static ObstacleType HELLHOUND = new ObstacleMonster("a hellhound",DamageType.FIRE, RequirementType.COLD_DAMAGE);
				public static ObstacleType OOZE = new ObstacleMonster("an ooze", DamageType.ACID, RequirementType.FIRE_DAMAGE);
				public static ObstacleType WINTERWOLF = new ObstacleMonster("a winterwolf", DamageType.COLD, RequirementType.POISON_DAMAGE);
				public static ObstacleType ANIMANT_PLANT = new ObstacleMonster("an animated plant", DamageType.POISON, RequirementType.ACID_DAMAGE);
				public static ObstacleType UNDEAD = new ObstacleMonster("the undead", DamageType.UNHOLY, RequirementType.HOLY_DAMAGE);
				public static ObstacleType ARCHON = new ObstacleMonster("an archon", DamageType.HOLY, RequirementType.UNHOLY_DAMAGE);

				public static ObstacleType NYMPH = new ObstacleMonster("a nymph", DamageType.GENERIC, RequirementType.UGLINESS);
				public static ObstacleType VIPER = new ObstacleMonster("a sand viper", DamageType.POISON, RequirementType.CLUMSINESS);
				public static ObstacleType TROLL = new ObstacleMonster("a troll", DamageType.GENERIC, RequirementType.WEAKNESS);
				public static ObstacleType SPIDER = new ObstacleMonster("a giant spider", DamageType.POISON, RequirementType.STUPIDITY);
			}

			public static ObstacleType getRandomMonster(Random rand) {
				FieldInfo[] fields = fields = typeof(Monsters).GetFields();
				int r = rand.Next(fields.Length);
				if(r > 1) r = rand.Next(fields.Length); //less likely to roll thing other than bandits
				if(r > 7) r = rand.Next(fields.Length); //less likely to roll anit-attribute monsters
				FieldInfo field = fields[r];
				return (ObstacleType)field.GetValue(null);
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Unexpected).GetFields();
				int r = rand.Next(fields.Length + (2 + StatisticsTracker.minQuestDifficulty.value / 2));
				if(r < fields.Length) {
					FieldInfo field = fields[r];
					return (ObstacleType)field.GetValue(null);
				}
				else if(r == fields.Length) { //traps
					fields = typeof(Traps).GetFields();
					r = rand.Next(fields.Length);
					if(r >= fields.Length-6) r = rand.Next(fields.Length); //less likely to roll magic
					FieldInfo field = fields[r];
					return (ObstacleType)field.GetValue(null);
				}
				else if(r >= fields.Length + 1) { //monsters
					fields = typeof(Monsters).GetFields();
					r = rand.Next(fields.Length);
					if(r > 1) r = rand.Next(fields.Length); //less likely to roll thing other than bandits
					if(r > 7) r = rand.Next(fields.Length); //less likely to roll anit-attribute monsters
					FieldInfo field = fields[r];
					return (ObstacleType)field.GetValue(null);
				}
				return null;
			}
		}
		public static class Loot {
			public static ObstacleType TREASURE = new ObstacleTreasure();
			public static ObstacleType COMMON_ITEM = new ObstacleResourceCacheSimple();
			public static ObstacleType RARE_ITEM = new ObstacleResourceCache();
			public static ObstacleType TRAVELING_MERCHANT = new ObstacleBuyEquipment("traveling merchant");
			public static ObstacleType HOARD = new ObstacleHoard();

			//not the best category, but it'll do for now
			public static ObstacleType VAMPIRE_MARKET = new ObstacleVampireTradingHouse();

			public static ObstacleType getRandom(Random rand) {
				return getRandom(rand, false);
			}

			public static ObstacleType getRandom(Random rand, bool final) {
				FieldInfo[] fields = typeof(Loot).GetFields();
				int r = rand.Next(fields.Length);
				while(r == 3 && final) {
					r = rand.Next(fields.Length);
				}
				FieldInfo field = fields[r];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}

			public static void AddRandomStatPotion(Quest theQuest, int v) {
				int r;
				for(int i = 0; i < v; i++) {
					r = theQuest.testLuck(4);
					switch(r) {
						case 0:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_STRENGTH, 1));
							break;
						case 1:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_AGILITY, 1));
							break;
						case 2:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_INTELLIGENCE, 1));
							break;
						case 3:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_CHARISMA, 1));
							break;
					}
				}
			}

			public static void AddCommonResource(Quest theQuest) {
				Item i = Items.getRandom(theQuest.questRand, 0, 11);
				int s = ResourceQuantity(theQuest.questRand, i.minStackSize, i.maxStackSize);
				s += checkHerbalism(theQuest, i);
				//NotificationItem notify = new NotificationItem(theQuest.heroName, "Found: " + Main.ToTitleCase(i.name) + "\nAdded to your stocks", SpriteLoader.getSpriteForResource("items/" + i.name));
				Main.instance.player.addItemToInventory(new ItemStack(i, s), theQuest, true);
			}

			public static void AddUncommonResource(Quest theQuest) {
				Item i = Items.getRandom(theQuest.questRand, 11, 23);
				int s = ResourceQuantity(theQuest.questRand, i.minStackSize, i.maxStackSize);
				s += checkHerbalism(theQuest, i);
				//NotificationItem notify = new NotificationItem(theQuest.heroName, "Found: " + Main.ToTitleCase(i.name) + "\nAdded to your stocks", SpriteLoader.getSpriteForResource("items/" + i.name));
				Main.instance.player.addItemToInventory(new ItemStack(i, s), theQuest, true);
			}

			public static void AddRareResource(Quest theQuest) {
				Item i = Items.getRandom(theQuest.questRand, 23, 34);
				int s = ResourceQuantity(theQuest.questRand, i.minStackSize, i.maxStackSize);
				s += checkHerbalism(theQuest, i);
				//NotificationItem notify = new NotificationItem(theQuest.heroName, "Found: " + Main.ToTitleCase(i.name) + "\nAdded to your stocks", SpriteLoader.getSpriteForResource("items/" + i.name));
				Main.instance.player.addItemToInventory(new ItemStack(i, s), theQuest, true);
			}

			public static void AddResource(Quest theQuest, Items.ItemType type) {
				Item i = Items.getRandomType(theQuest.questRand, type);
				int s = ResourceQuantity(theQuest.questRand, i.minStackSize, i.maxStackSize);
				s += checkHerbalism(theQuest, i);
				//NotificationItem notify = new NotificationItem(theQuest.heroName, "Found: " + Main.ToTitleCase(i.name) + "\nAdded to your stocks", SpriteLoader.getSpriteForResource("items/" + i.name));
				Main.instance.player.addItemToInventory(new ItemStack(i, s), theQuest, true);
			}

			private static int ResourceQuantity(Random rand, int min, int max) {
				UpgradeValueWrapper wrap;
				Main.instance.player.upgrades.TryGetValue(upgrades.UpgradeType.QUEST_LOOT, out wrap);
				float f = ((UpgradeFloatValue)wrap).value * Main.instance.player.currentGuildmaster.ingredientIncomeMultiplier();
				min = UnityEngine.Mathf.RoundToInt(min * f);
				max = UnityEngine.Mathf.RoundToInt(max * f);
				return rand.Next(max - min + 1) + max;
			}

			private static int checkHerbalism(Quest theQuest, Item item) {
				if(item.maxStackSize == 1) return 1;
				bool hasHerbalism = false;
				foreach(ItemStack stack in theQuest.inventory) {
					if(stack.doesStackHave(Enchantments.HERBALISM)) {
						hasHerbalism = true;
					}
				}
				if(hasHerbalism) {
					return ResourceQuantity(theQuest.questRand, 1, 4);
				}
				return 0;
			}

			public static void AddRelic(Quest theQuest) {
				ItemStack stack = QuestManager.getRandomTreasure(theQuest);
				if(stack != null) {
					//NotificationItem notify = new NotificationItem(theQuest.heroName, "Found an unidentified relic.", SpriteLoader.getSpriteForResource("items/relic"));
					theQuest.addItemToInventory(stack);
				}
				else {
					AddRareResource(theQuest);
					AddRareResource(theQuest);
				}
			}

			public static void AddStack(Quest theQuest, ItemStack stack) {
				//NotificationItem notify = new NotificationItem(theQuest.heroName, "Found: " + Main.ToTitleCase(stack.item.name) + "\nAdded to your stocks", SpriteLoader.getSpriteForResource("items/" + stack.item.name));
				Main.instance.player.addItemToInventory(stack, theQuest, true);
			}
		}
		public static class Scenario {
			public static ObstacleType SIRENS = new ObstacleSirens();
			public static ObstacleType PIRATE_SHIP = new ObstaclePirateShip();
			public static class Pirates {
				public static ObstacleType MAROONED = new ObstacleMarooned();
				public static ObstacleType SAIL_PIRATE_WATERS = new ObstacleSailPirateWaters();
				public static ObstacleType UNDERWATER_RUINS = new ObstacleUnderwaterRuins();
				public static ObstacleType LOST_LAGOON = new ObstacleLostLagoon();
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Scenario).GetFields();
				FieldInfo field = fields[rand.Next(fields.Length)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
		public static class General {
			public static ObstacleType SNEAKING = new ObstacleSneaking();
			public static ObstacleType MAIDEN = new ObstacleFairMaiden();
			public static ObstacleType FESTIVAL = new ObstacleFestival();
			public static ObstacleType UNEVENTFUL = new ObstacleUneventful();
			public static ObstacleType WILDERNESS = new ObstacleWilderness();
			public static ObstacleType NIGHT_WATCH = new ObstacleNightWatch();
			public static ObstacleType PRIEST = new ObstaclePriest();

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(General).GetFields();
				FieldInfo field = fields[rand.Next(fields.Length)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
	}
}