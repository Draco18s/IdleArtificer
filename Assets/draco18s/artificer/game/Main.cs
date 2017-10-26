using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.masters;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.hero;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.config;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.game {
	public class Main : MonoBehaviour {
		public static int saveVersionFromDisk;
		public static Main instance;
		public PlayerInfo player;

		public bool debugMode;
		public bool debugAutoBuild;
		public GameObject errorWindow;
		public float mouseDownTime;
		private float autoClickTime;

		/*autobuild debugging*/
		public float timeSinceLastPurchase;
		public float timeTotal;
		//private StreamWriter csv_st;
		//private string lastPurchase = "";
		//private int lastTime = -1;
		public bool close_file = false;

		void Start() {
			Profiler.maxNumberOfSamplesPerFrame = -1;
			/*string path = "E:\\Users\\Major\\Desktop\\time_data.csv";
			if(File.Exists(path)) {
				File.Delete(path);
			}
			csv_st = File.CreateText(path);
			csv_st.WriteLine("TotalTime,TimeToNextBuilding,Income/Sec,CashOnHand,LastPurchase");*/
			instance = this;
			Application.runInBackground = true;
			player = new PlayerInfo();
			//money = new BigInteger(10000);
			GuiManager.instance.mainCanvas.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.FacilityUnselected(); });
			Localization.initialize();
			EnchantingManager.OneTimeSetup();
			QuestManager.setupUI();
			GuildManager.OneTimeSetup();
			ResearchManager.OneTimeSetup();
			AchievementsManager.OneTimeSetup();
			Configuration.loadCurrentDirectory();

			InfoPanel panel = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			panel.ConsumeToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleAllowConsume(); });
			panel.ProduceToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleAllowProduction(); });
			panel.SellToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleSellStores(); });
			panel.BuildToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleAutoBuild(); });
			
			Button btn;
			GuiManager.instance.infoPanel.transform.FindChild("Output").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.AdvanceTimer(); });
			btn = GuiManager.instance.infoPanel.transform.FindChild("SellAll").GetComponent<Button>();
			btn.onClick.AddListener(delegate { CraftingManager.SellAll(); });
			//string veryLong = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam pharetra tincidunt mi, sed volutpat est elementum id. Etiam eleifend arcu vitae sem efficitur, ut congue massa ultricies. Ut facilisis, leo id tincidunt viverra, sem metus accumsan neque, ac tristique erat quam id lacus. Vivamus quis augue eros. Maecenas non laoreet ligula. Nunc id tellus consectetur ipsum volutpat convallis nec a arcu. Phasellus fermentum sapien eget porttitor convallis.";
			btn.AddHover(delegate (Vector3 p) {
				GuiManager.ShowTooltip(GuiManager.instance.infoPanel.transform.FindChild("SellAll").transform.position + Vector3.right * 45, "Sell all " + AsCurrency(CraftingManager.GetQuantity()) + " " + CraftingManager.GetName() + " for $" + AsCurrency(CraftingManager.SellAllValue()));
			});

			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			info.transform.FindChild("Input1").GetComponent<Button>().onClick.AddListener(delegate () { CraftingManager.SelectInput(1); });
			info.transform.FindChild("Input2").GetComponent<Button>().onClick.AddListener(delegate () { CraftingManager.SelectInput(2); });
			info.transform.FindChild("Input3").GetComponent<Button>().onClick.AddListener(delegate () { CraftingManager.SelectInput(3); });
			info.UpgradeBtn.onClick.AddListener(delegate { CraftingManager.UpgradeCurrent(); });
			info.DowngradeBtn.onClick.AddListener(delegate { CraftingManager.DowngradeCurrent(); });
			info.ConfDowngradeBtn.onClick.AddListener(delegate { CraftingManager.Do_DowngradeCurrent(); });
			info.VendNum.transform.FindChild("IncVend").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseVendors(); });
			info.VendNum.transform.FindChild("DecVend").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseVendors(); });
			GuiManager.instance.infoPanel.transform.FindChild("NumAppr").FindChild("IncAppr").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseApprentices(); });
			GuiManager.instance.infoPanel.transform.FindChild("NumAppr").FindChild("DecAppr").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseApprentices(); });
			info.BuildNum.transform.FindChild("IncBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseAutoBuild(); });
			info.BuildNum.transform.FindChild("DecBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseAutoBuild(); });
			info.VendText.transform.GetChild(0).GetComponent<Button>().AddHover(delegate (Vector3 p) { GuiManager.ShowTooltip(info.VendText.transform.position + Vector3.right * 45, "Your vendors currently sell " + AsCurrency(CraftingManager.NumberSoldByVendors()) + " units per cycle.\nConsidering the Vendor Effectiveness multiplier, you'll earn $" + AsCurrency(CraftingManager.ValueSoldByVendors()) + " per cycle.", 1.61f); }, false);
			info.MagnitudeNum.transform.FindChild("IncBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseBuildMagnitude(); });
			info.MagnitudeNum.transform.FindChild("DecBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseBuildMagnitude(); });

			GuiManager.instance.craftTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.craftTab, GuiManager.instance.craftArea, GuiManager.instance.craftHeader); });
			GuiManager.instance.enchantTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.enchantTab, GuiManager.instance.enchantArea, GuiManager.instance.enchantHeader); });
			GuiManager.instance.questTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.questTab, GuiManager.instance.questArea, GuiManager.instance.questHeader); });
			GuiManager.instance.guildTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.guildTab, GuiManager.instance.guildArea, GuiManager.instance.guildHeader); });
			GuiManager.instance.researchTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.researchTab, GuiManager.instance.researchArea, GuiManager.instance.researchHeader); });
			GuiManager.instance.achievementsTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.achievementsTab, GuiManager.instance.achievementsArea, GuiManager.instance.achievementsHeader); });

			GuiManager.instance.craftArea.GetComponent<Canvas>().enabled = true;
			GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.questArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.guildArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.researchArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.achievementsArea.GetComponent<Canvas>().enabled = false;

			btn = GuiManager.instance.craftHeader.transform.FindChild("ResetBtn").GetComponent<Button>();
			btn.onClick.AddListener(delegate { player.reset(); });
			btn.AddHover(delegate (Vector3 p) {
				/*BigInteger spentRenown = Main.instance.player.totalRenown - Main.instance.player.renown;
				BigInteger totalRenown = BigInteger.CubeRoot(Main.instance.player.lifetimeMoney);
				totalRenown /= 10000;
				BigInteger renown = totalRenown - spentRenown + Main.instance.player.questsCompleted;*/
				BigInteger renown = getCachedNewRenown() + Main.instance.player.questsCompleted;
				GuiManager.ShowTooltip(GuiManager.instance.craftHeader.transform.FindChild("ResetBtn").transform.position, "You will gain " + Main.AsCurrency(renown) + " Renown if you reset now.", 2.3f);
			});
			btn = GuiManager.instance.craftHeader.transform.FindChild("SyncBtn").GetComponent<Button>();
			btn.onClick.AddListener(delegate { CraftingManager.SynchronizeInustries(); });
			Toggle tog = GuiManager.instance.craftHeader.transform.FindChild("AutoToggle").GetComponent<Toggle>();
			tog.onValueChanged.AddListener(delegate { debugAutoBuild = !debugAutoBuild; });
			tog.AddHover(delegate (Vector3 pos) {
				GuiManager.ShowTooltip(tog.transform.position + Vector3.right * 50, "Builds industries in order of best cost:benefit ratio, up to the specified autobuild limits.", 3);
			}, false);

			GuiManager.instance.buyVendorsArea.transform.FindChild("BuyOne").GetComponent<Button>().onClick.AddListener(delegate { GuildManager.BuyVendor(); });
			GuiManager.instance.buyApprenticesArea.transform.FindChild("BuyOne").GetComponent<Button>().onClick.AddListener(delegate { GuildManager.BuyApprentice(); });
			GuiManager.instance.buyJourneymenArea.transform.FindChild("BuyOne").GetComponent<Button>().onClick.AddListener(delegate { GuildManager.BuyJourneyman(); });
			
			GuiManager.instance.topPanel.transform.FindChild("SaveBtn").GetComponent<Button>().onClick.AddListener(delegate {
				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					DataAccess.Save(player);
				}
				else {
					writeDataToSave();
				}
			});
#pragma warning disable 0219
			//make sure these static class references have been created
			ObstacleType ob = ChallengeTypes.General.FESTIVAL;
			ob = ChallengeTypes.Goals.ALCHEMY_LAB;
			ob = ChallengeTypes.Goals.Sub.MUMMY;
			ob = ChallengeTypes.Initial.AT_HOME;
			ob = ChallengeTypes.Initial.Sub.BAR_BRAWL;
			ob = ChallengeTypes.Initial.Town.GARDENS;
			ob = ChallengeTypes.Loot.TREASURE;
			ob = ChallengeTypes.Scenario.Pirates.MAROONED;
			ob = ChallengeTypes.Travel.SAIL_SEAS;
			ob = ChallengeTypes.Unexpected.THIEF;
			ob = ChallengeTypes.Unexpected.Sub.GENIE;
			ob = ChallengeTypes.Unexpected.Traps.MAGIC_TRAP_ACID;
			ob = ChallengeTypes.Unexpected.Monsters.ANIMANT_PLANT;
			Item it11 = Items.SpecialItems.POWER_STONE;
#pragma warning restore 0219
			bool saveExists = false;
			if(Application.platform == RuntimePlatform.WebGLPlayer) {
				saveExists = DataAccess.Load();
			}
			else {
				saveExists = readDataFromSave();
			}
			instance.player.FinishLoad();

			if(!saveExists) {
				Debug.Log("No save data, generating quests");
				//QuestManager.tickAllQuests(3600);

				ItemStack newRelic = new ItemStack(Industries.IRON_SWORD, 1);
				QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				newRelic = new ItemStack(Industries.IRON_RING, 1);
				int loopcount = 0;
				bool ret = true;
				System.Random rand = new System.Random();
				do {
					loopcount++;
					Item item = Items.getRandom(rand);
					Enchantment ench = GameRegistry.GetEnchantmentByItem(item);
					if(ench != null)
						Debug.Log(ench.name);
					else if(item != Items.FOURFOIL) {
						//Debug.Log("Null enchant! " + item.name);
						throw new Exception("Null enchantment for " + item.name);
					}
					if(ench != null && (newRelic.item.equipType & ench.enchantSlotRestriction) > 0) {
						newRelic.applyEnchantment(ench);
						ret = false;
					}
				} while(ret && loopcount < 30);
				QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				newRelic = new ItemStack(Industries.IRON_HELMET, 1);
				QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				newRelic = new ItemStack(Industries.IRON_BOOTS, 1);
				QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				newRelic = new ItemStack(Industries.IMPROVED_CLOAK, 1);
				QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				newRelic = new ItemStack(Items.SpecialItems.POWER_STONE, 1);
				QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));

				GuiManager.instance.enchantTab.GetComponent<Button>().interactable = false;
				GuiManager.instance.guildTab.GetComponent<Button>().interactable = false;
				GuiManager.instance.questTab.GetComponent<Button>().interactable = false;
				GuiManager.instance.researchTab.GetComponent<Button>().interactable = false;
			}
			else {
				//fixes screwups with the cursed stone
				ItemStack badStack = null;
				bool hasCursedStone = false;
				foreach(ItemStack stack in player.miscInventory) {
					if(stack.item == Items.SpecialItems.POWER_STONE && stack.relicData == null) {
						badStack = stack;
					}
					else if(stack.item == Items.SpecialItems.POWER_STONE) {
						hasCursedStone = true;
					}
				}
				if(badStack != null)
					player.miscInventory.Remove(badStack);
				foreach(ItemStack stack in QuestManager.availableRelics) {
					if(stack.item == Items.SpecialItems.POWER_STONE && !hasCursedStone) {
						hasCursedStone = true;
					}
					else if(stack.item == Items.SpecialItems.POWER_STONE && hasCursedStone) {
						badStack = stack;
					}
				}
				if(badStack != null)
					QuestManager.availableRelics.Remove(badStack);
				if(!hasCursedStone) {
					ItemStack newRelic = new ItemStack(Items.SpecialItems.POWER_STONE, 1);
					QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				}
			}

			IEnumerator<StatAchievement> list = StatisticsTracker.getAchievementsList();
			while(list.MoveNext()) {
				StatAchievement ac = list.Current;
				ac.determineImage();
			}

			if(!StatisticsTracker.unlockedEnchanting.isAchieved()) GuiManager.instance.enchantTab.GetComponent<Button>().interactable = false;
			if(!StatisticsTracker.unlockedGuild.isAchieved()) GuiManager.instance.guildTab.GetComponent<Button>().interactable = false;
			if(!StatisticsTracker.unlockedQuesting.isAchieved()) GuiManager.instance.questTab.GetComponent<Button>().interactable = false;
			if(!StatisticsTracker.unlockedResearch.isAchieved()) GuiManager.instance.researchTab.GetComponent<Button>().interactable = false;

			CraftingManager.setupUI();
			GuildManager.update();
			long nl = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
			long ll = StatisticsTracker.lastDailyLogin.value * 5;
			if(ll > 0) {
				DateTime nowLogin = DateTime.ParseExact(nl.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
				DateTime lastLogin = DateTime.ParseExact(ll.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
				if((nowLogin - lastLogin) >= TimeSpan.FromHours(23)) {
					if((nowLogin - lastLogin) > TimeSpan.FromHours(48)) {
						StatisticsTracker.sequentialDaysPlayed.resetValue();
					}
					//DAILY BONUS
					StatisticsTracker.sequentialDaysPlayed.addValue(1);
					StatisticsTracker.lastDailyLogin.setValue((int)(nl / 5));
				}
			}
			else {
				StatisticsTracker.sequentialDaysPlayed.resetValue();
				StatisticsTracker.sequentialDaysPlayed.addValue(1);
				StatisticsTracker.lastDailyLogin.setValue((int)(nl / 5));
			}
		}

		private float TimeSinceLastRequest = 20;
		private BigInteger CachedNewRenown = 0;
		public BigInteger getCachedNewRenown() {
			if(TimeSinceLastRequest >= 10) {
				TimeSinceLastRequest -= 10;
				BigInteger newRenown = BigInteger.CubeRoot(StatisticsTracker.lifetimeMoney.value);
				newRenown /= 10000;
				CachedNewRenown = newRenown;
			}
			return CachedNewRenown;
		}

		private void test(Vector3 pos) {

		}

		public static bool readDataFromSave() {
			//GuiManager.ShowNotification(new NotificationItem("Loading...", "", GuiManager.instance.checkOn));
			string path2 = Configuration.currentDirectory + "Save/savedata.dat"; //"E:\\Users\\Major\\Desktop\\savedata.dat";
			System.Object readFromDisk;

			if(File.Exists(path2)) {
				FileStream fs = new FileStream(path2, FileMode.OpenOrCreate);
				BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
				try {
					readFromDisk = formatter.Deserialize(fs);
					instance.player = (PlayerInfo)readFromDisk;
				}
				catch(SerializationException e) {
					GuiManager.ShowNotification(new NotificationItem("Failed to load", e.Message, GuiManager.instance.checkOff));
					//Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
					throw;
				}
				finally {
					fs.Close();
				}
				//GuiManager.ShowNotification(new NotificationItem("Loaded data!", "", GuiManager.instance.checkOn));
				return true;
			}
			return false;
		}

		public static void writeDataToSave() {
			//GuiManager.ShowNotification(new NotificationItem("Saving...", "", GuiManager.instance.checkOn));
			string path2 = Configuration.currentDirectory + "Save/savedata.dat";
			if(File.Exists(path2)) {
				File.Delete(path2);
			}

			FileStream s = new FileStream(path2, FileMode.Create);
			IFormatter formatter = new BinaryFormatter();
			try {
				formatter.Serialize(s, instance.player);
			}
			catch(SerializationException e) {
				DataAccess.PlatformSafeMessage("Failed to save: " + e.Message);
				//Assets.draco18s.util.DataAccess l;
				//GuiManager.ShowNotification(new NotificationItem("Failed to save", e.Message, GuiManager.instance.checkOff));
				//Console.WriteLine("Failed to serialize. Reason: " + e.Message);
				throw;
			}
			finally {
				s.Close();
			}
			GuiManager.ShowNotification(new NotificationItem("Game saved!", "", GuiManager.instance.checkOn));
		}

		public static bool readDataFromCookie() {

			return false;
		}
		public static void writeDataToCookie() {

		}

		internal static int BitNum(long v) {
			int j = 0;
			while(v >= 1) {
				v = v >> 1;
				j++;
			}
			return j;
		}

		void Update() {
			float deltaTime = Time.deltaTime * GetSpeedMultiplier();
			autoClickTime += deltaTime;
			TimeSinceLastRequest += deltaTime;
			bool doAutoClick = false;
			if(autoClickTime >= 1) {
				doAutoClick = true;
				autoClickTime -= 1;
			}

			Profiler.BeginSample("Quest Manager");
			QuestManager.tickAllQuests(deltaTime);
			QuestManager.updateLists();
			Profiler.EndSample();
			Profiler.BeginSample("Enchant Manager");
			EnchantingManager.update();
			Profiler.EndSample();
			Profiler.BeginSample("Guild Manager");
			GuildManager.update();
			Profiler.EndSample();
			Profiler.BeginSample("Research Manager");
			ResearchManager.update(deltaTime);
			Profiler.EndSample();
			Profiler.BeginSample("Achievements Manager");
			AchievementsManager.update();
			Profiler.EndSample();
			Profiler.BeginSample("Tick Built Items");
			bool needSynchro = false;
			
			float industryTime = deltaTime * Main.instance.player.currentGuildmaster.industryRateMultiplier();
			foreach(Industry i in player.builtItems) {
				if(i.getTimeRemaining() > float.MinValue && !i.isProductionHalted) {
					needSynchro = i.addTime(-industryTime) || needSynchro;
					//i.timeRemaining -= deltaTime;
					if(doAutoClick) {
						i.tickApprentices();
					}
				}
				if(i.getTimeRemaining() <= 0) {
					//do {
					bool canExtract = true;
					foreach(IndustryInput input in i.inputs) {
						if(input.item.quantityStored < input.quantity * i.getTotalLevel() || input.item.isConsumersHalted) {
							canExtract = false;
						}
					}
					if(canExtract) {
						foreach(IndustryInput input in i.inputs) {
							input.item.quantityStored -= input.quantity * i.getTotalLevel();
						}
						if(i.getTimeRemaining() > float.MinValue) {
							i.didComplete++;
							i.addTimeRaw(10);
							i.quantityStored += i.ProduceOutput();
							//i.quantityStored += i.output * i.level;
						}
						else {
							i.setTimeRemaining(10);
						}
					}
					else {
						i.setTimeRemaining(float.MinValue);
					}
					//} while(i.getTimeRemaining() < 0 && i.getTimeRemaining() > float.MinValue);
				}
				//if(i.guiObj != null) {
				Image img = i.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Progress").GetComponent<Image>();
				img.material.SetFloat("_Cutoff", ((i.getTimeRemaining() >= 0 ? i.getTimeRemaining() : 10) / 10f));
				img.material.SetColor("_Color", i.productType.color);
				//}
			}
			//if(CraftingManager.doSynchronize)
			//Debug.Log(needSynchro);
			if(!needSynchro && CraftingManager.doSynchronize) {
				CraftingManager.doSynchronize = false;
				GuiManager.instance.craftHeader.transform.FindChild("SyncBtn").GetComponent<Button>().interactable = true;
			}
			Profiler.EndSample();
			Profiler.BeginSample("Sell Items");
			foreach(Industry i in player.builtItems) {
				if(i.didComplete > 0) {
					do {
						BigInteger maxSell = (i.isSellingStores ? -1 : (i.output * i.getTotalLevel()) - i.consumeAmount);
						BigInteger quant = i.quantityStored;
						BigInteger amt = i.getVendors() * GetVendorSize();
						if(maxSell >= 0)
							amt = MathHelper.Min(amt, maxSell);
						/*i.quantityStored -= amt;
						i.quantityStored = (i.quantityStored < 0 ? 0 : i.quantityStored);*/
						amt = MathHelper.Max(MathHelper.Min(amt, i.quantityStored), 0);
						i.quantityStored -= amt;
						quant -= i.quantityStored;
						BigRational val = ((BigRational)quant * GetVendorValue());
						val *= i.GetSellValue();
						AddMoney((BigInteger)val);
						//AddMoney(GetVendorValue() * (quant * i.GetSellValue()));
						i.didComplete--;
					} while(i.didComplete > 0);
				}
			}
			Profiler.EndSample();
			Profiler.BeginSample("Crafting Update");
			CraftingManager.update();
			Profiler.EndSample();
			if(Input.GetMouseButton(0)) {
				mouseDownTime += Time.deltaTime;
			}
			else {
				mouseDownTime = 0;
			}
			Profiler.BeginSample("Autobuild");
			if(debugAutoBuild) {
				autoBuildTimer += Time.deltaTime;
				if(autoBuildTimer > 5) {
					autoBuildTimer -= 5;
					bool ret = true;
					DateTime start = System.DateTime.Now;
					while(ret) {
						ret = SmartBuild(deltaTime);
						float timeTaken = (System.DateTime.Now - start).Milliseconds;
						//Debug.Log("Autobuild time used: " + timeTaken +"(cont? " + (ret?"yes":"no") + ")");
						if(timeTaken > 200) {
							ret = false;
						}
					}
					Debug.Log("Total autobuild time used: " + (System.DateTime.Now - start).Milliseconds);
				}
				Material mat = GuiManager.instance.autoBuildBar.GetComponent<Image>().material;
				mat.SetFloat("_Cutoff", 1 - (autoBuildTimer / 5f));
			}
			else {
				autoBuildTimer = 0;
				Material mat = GuiManager.instance.autoBuildBar.GetComponent<Image>().material;
				mat.SetFloat("_Cutoff", 1);
			}
			Profiler.EndSample();
			if(Mathf.FloorToInt(Time.time * 10) % 20 == 0) {
				foreach(Industry item in player.builtItems) {
					item.consumeAmount = 0;
				}
				foreach(Industry item in player.builtItems) {
					foreach(IndustryInput input in item.inputs) {
						if(!item.isProductionHalted && !input.item.isConsumersHalted) {
							float mod = (float)item.getHalveAndDouble() / input.item.getHalveAndDouble();
							input.item.consumeAmount += Mathf.RoundToInt((input.quantity * item.getTotalLevel()) * mod);
						}
					}
				}
			}
		}

		private static float autoBuildTimer = 0;

		private bool SmartBuild(float dt) {
			bool ret = false;
			List<CostBenefitRatio> compares = new List<CostBenefitRatio>();
			FieldInfo[] fields = typeof(Industries).GetFields();
			BigInteger currIncome = 0;
			foreach(Industry indu in player.builtItems) {
				currIncome += (BigInteger)indu.GetScaledCost() * indu.output;
			}
			//Debug.Log("start: "  + DateTime.Now.Millisecond);
			foreach(FieldInfo field in fields) {
				Industry ind = (Industry)field.GetValue(null);
				//player.itemData.TryGetValue(indBtn, out ind);
				BigInteger seconds = currIncome > 0 ? ((BigInteger)ind.GetScaledCost() - player.money) / currIncome : -1000000;
				if(seconds < 15 && ind.doAutobuild && ind.autoBuildLevel > ind.level && ind.autoBuildMagnitude <= (player.money.ToString().Length - 1)) {
					BigInteger inputCosts = 0;
					int bonus = (ind.GetScaledCost() <= player.money ? 3 : (seconds <= 5 ? 3 : 1));
					double penalty = 1;
					foreach(IndustryInput input in ind.inputs) {
						if(input.item.level == 0) {
							penalty = 1000;
						}
						else if(input.item.consumeAmount >= input.item.output * input.item.getTotalLevel()) {
							penalty *= 10 * Math.Pow(input.item.productType.amount, 1 + input.item.getTotalLevel() + ((input.item.consumeAmount - (input.item.output * input.item.getTotalLevel())) / input.item.output));
							//Debug.Log(ind.name + " (" + input.item.name + "):" + penalty);
						}
						inputCosts += (input.item.GetBaseSellValue() * input.quantity);
					}
					if(ind.consumeAmount > ind.output * ind.level) {
						bonus *= 2;
					}
					if(ind.level == 0 && ind.GetScaledCost() <= player.money) {
						compares.Add(new CostBenefitRatio(1, ((ind.GetBaseSellValue() * ind.output) - inputCosts) * bonus, ind));
					}
					else {
						compares.Add(new CostBenefitRatio((BigInteger)(penalty * (BigRational)ind.GetScaledCost()), ((ind.GetBaseSellValue() * ind.output) - inputCosts) * bonus, ind));
					}
				}
			}
			//Debug.Log("mid: " + DateTime.Now.Millisecond);
			compares.Sort();
			/*if(level == 8) {
				Debug.Log("1: " + compares[0].indust.name + " " + (compares[0].cost + "/" + compares[0].benefit));
				Debug.Log("2: " + compares[1].indust.name + " " + (compares[1].cost + "/" + compares[1].benefit));
				Debug.Log("3: " + compares[2].indust.name + " " + (compares[2].cost + "/" + compares[2].benefit));
				Debug.Log("4: " + compares[3].indust.name + " " + (compares[3].cost + "/" + compares[3].benefit));
				Debug.Log("5: " + compares[4].indust.name + " " + (compares[4].cost + "/" + compares[4].benefit));
				if(compares[0].indust != Industries.WOOD) {
					foreach(IndustryInput input in compares[0].indust.inputs) {
						Debug.Log("   :" + input.item.level);
					}
					throw new Exception();
				}
			}*/
			Industry indust = (compares.Count > 0 ? compares[0].indust : null);
			if(indust != null && indust.GetScaledCost() <= player.money) {
				timeSinceLastPurchase = 0;
				CraftingManager.BuildIndustry(indust);
				foreach(IndustryInput input in indust.inputs) {
					float mod = (float)indust.getHalveAndDouble() / input.item.getHalveAndDouble();
					input.item.consumeAmount += Mathf.RoundToInt(input.quantity * mod);
				}
				//compares[0].indust.AdjustVendors(Mathf.CeilToInt((float)((indust.output * indust.level) - indust.consumeAmount) / Main.instance.GetVendorSize()));
				//compares[0].indust.vendors = (compares[0].indust.vendors > 0)? compares[0].indust.vendors : compares[0].indust.vendors * -1;
				//lastPurchase += indust.name + " ";
				ret = true;
			}
			/*float dtt = dt;
			do {
				timeSinceLastPurchase += dtt>1?1:dtt;
				timeTotal += dtt > 1 ? 1 : dtt;
				int cur = (Mathf.FloorToInt(timeTotal));
				if(cur % 60 == 0 && lastTime != cur) {
					lastTime = cur;
					//csv += "\n" + Mathf.FloorToInt(timeTotal) + "," + Mathf.FloorToInt(timeSinceLastPurchase) + "," + ApproximateIncome(fields);
					
					//csv_st.WriteLine(Mathf.FloorToInt(timeTotal) + "," + Mathf.FloorToInt(timeSinceLastPurchase) + "," + ApproximateIncome(fields) + "," + player.money + "," + lastPurchase);
					//lastPurchase = "";
				}
				dtt -= 1;
			} while(dtt > 0);
			if(close_file) {
				//csv_st.Close();
			}*/
			//Debug.Log("end: " + DateTime.Now.Millisecond);
			return ret;
		}

		public void CompleteQuest(ObstacleType goal) {
			player.QuestComplete(goal);
		}

		public static int GetNeededVendors(Industry indust) {
			int j = Mathf.CeilToInt((float)((indust.output * indust.getTotalLevel()) - indust.consumeAmount) / Main.instance.GetVendorSize());

			return j >= 0 ? j : 0;
		}

		private BigInteger ApproximateIncome(FieldInfo[] fields) {
			BigInteger outval = 0;
			foreach(FieldInfo field in fields) {
				Industry ind = (Industry)field.GetValue(null);
				BigInteger o = ((ind.output * ind.getTotalLevel()) - ind.consumeAmount);
				outval += (BigInteger)(ind.GetSellValue() * (o >= 0 ? o : 0));
			}
			return outval;
		}

		public int GetQuestStackMultiplier(Industry selectedIndustry, long numAlreadyCompleted) {
			return Mathf.RoundToInt(1000 * player.GetQuestDifficultyMultiplier(numAlreadyCompleted));
		}

		public float GetSpeedMultiplier() {
			//TODO: Speed bonuses
			return (debugMode ? 1000 : 1);
		}

		public float GetClickRate() {
			return player.getClickRate();
		}

		public void AddMoney(BigInteger val) {
			//TODO: add various multipliers
			//money += val;
			player.AddMoney(val);
		}

		public int GetVendorSize() {
			//TODO: various bonuses
			return player.GetVendorSize();
		}

		public BigRational GetVendorValue() {
			return player.GetVendorValue();
		}

		public BigRational GetSellMultiplierFull() {
			return player.GetSellMultiplierFull();
		}

		public BigRational GetRelicSellMultiplier() {
			return player.GetRelicSellMultiplier();
		}

		private void switchTabImage(GameObject newTab, GameObject newArea, GameObject newHeader) {
			GuiManager.instance.craftTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.enchantTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.questTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.guildTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.researchTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.achievementsTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			newTab.GetComponent<Image>().sprite = GuiManager.instance.selTab;

			GuiManager.instance.craftArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.questArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.guildArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.researchArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.achievementsArea.GetComponent<Canvas>().enabled = false;
			newArea.GetComponent<Canvas>().enabled = true;

			GuiManager.instance.craftHeader.SetActive(false);
			GuiManager.instance.enchantHeader.SetActive(false);
			GuiManager.instance.questHeader.SetActive(false);
			GuiManager.instance.guildHeader.SetActive(false);
			GuiManager.instance.researchHeader.SetActive(false);
			GuiManager.instance.achievementsHeader.SetActive(false);
			newHeader.SetActive(true);

			if(newTab == GuiManager.instance.craftTab) {

			}
			if(newTab == GuiManager.instance.enchantTab) {
				EnchantingManager.setupUI();
			}
			if(newTab == GuiManager.instance.questTab) {
				QuestManager.setupUI();
			}
			if(newTab == GuiManager.instance.guildTab) {
				GuildManager.setupUI();
			}
			if(newTab == GuiManager.instance.researchTab) {
				ResearchManager.setupUI();
			}
			if(newTab == GuiManager.instance.achievementsTab) {
				AchievementsManager.setupUI();
			}
			GuiManager.instance.infoPanel.transform.localPosition = new Vector3(-1465, 55, 0);
			CraftingManager.FacilityUnselected(null);
		}

		public static string ToTitleCase(string stringToConvert) {
			bool convertNext = true;
			string output = "";
			foreach(char c in stringToConvert) {
				bool conv = convertNext;
				char newChar = (c == '_' ? ' ' : c);
				if(newChar == ' ') {
					convertNext = true;
				}
				else {
					convertNext = false;
				}
				if(!conv) {
					newChar = newChar.ToString().ToLower()[0];
				}
				else {
					newChar = newChar.ToString().ToUpper()[0];
				}
				output += newChar;
			}
			return output;
		}

		public static string AsCurrency(BigRational cost) {
			return AsCurrency(cost, 9);
		}

		public static string AsCurrency(BigRational cost, int maxDigits) {
			return AsCurrency(cost, maxDigits, false);
		}

		public static string AsCurrency(BigInteger cost) {
			return AsCurrency(cost, 9);
		}

		public static string AsCurrency(object cost) {
			if(cost is int || cost is float)
				return AsCurrency((int)cost, 9);
			if(cost is BigInteger || cost is BigRational)
				return AsCurrency((BigInteger)cost, 9);
			return cost.ToString();
		}

		public static string AsCurrency(object cost, int maxDigits) {
			if(cost is int || cost is float)
				return AsCurrency((int)cost, maxDigits);
			if(cost is BigInteger || cost is BigRational)
				return AsCurrency((BigInteger)cost, maxDigits);
			return cost.ToString();
		}

		public static string AsCurrency(BigInteger cost, int maxDigits) {
			return AsCurrency(cost, maxDigits, false);
		}

		public static string AsCurrency(BigRational valIn, int maxDigits, bool skipDecimal) {
			return AsCurrency((BigInteger)valIn, maxDigits, skipDecimal);
		}

		public static string AsCurrency(BigInteger val, int maxDigits, bool skipDecimal) {
			if(maxDigits < 4) maxDigits = 4;
			bool isNeg = val.IsNegative;
			BigInteger num = BigInteger.Abs(new BigInteger(val));
			string simple = num.ToString();
			string output = "";
			if(simple.Length > maxDigits) {
				int d = (simple.Length % 3);
				if(d == 0) d = 3;
				for(int i = 0; i < d; i++) {
					output += simple[i];
				}

				int m = 3;
				for(int i = d + 2; i >= d; i--) {
					if(simple[i].Equals('0')) {
						m--;
					}
					else {
						break;
					}
				}
				if(m == 0) {
					m = 1;
				}
				if(d + m > maxDigits) {
					m = maxDigits - d;
				}
				if(!skipDecimal) {
					output += ".";
					for(int i = d; i < d + m; i++) {
						output += simple[i];
					}
				}
				if(skipDecimal) {
					output += "e" + (simple.Length - d);
					if(output.Length > 6) { //this will fail at values greater than 9e99999
						int g = output.IndexOf('e');
						output = output.Substring(0, g - 1);
						output += "e" + (simple.Length - d + 1);
					}
				}
				else {
					output += " E" + (simple.Length - d);
				}
			}
			else {
				for(int i = 0; i < simple.Length; i++) {
					if(i % 3 == 0 && i != 0) {
						output = "," + output;
					}
					output = simple[(simple.Length - i - 1)] + output;
				}
			}
			if(isNeg) {
				output = "-" + output;
			}
			return output;
		}

		public static string SecondsToTime(float timeIn) {
			long time = (int)timeIn;
			float frac = timeIn - time;
			int fracInt = Mathf.RoundToInt(100 * frac);
			if(fracInt == 100) fracInt = 0;
			long seconds = (time % 60);
			long minutes = ((time - seconds) / 60) % 60;
			long hours = ((time - seconds - (minutes * 60)) / 3600);
			if(hours > 0) {
				if(seconds > 30) minutes++;
				if(minutes > 59) {
					minutes = 0;
					hours++;
				}
				return hours + ":" + (minutes < 10L ? "0" : "") + minutes + "h";
			}
			if(minutes > 0)
				return minutes + ":" + (seconds < 10L ? "0" : "") + seconds + "m";
			return seconds + (fracInt > 0 ? (fracInt >= 10 ? "." + fracInt : ".0" + fracInt) : ".00") + "s";
		}

		public void writeCSVLine(string text) {
			//FieldInfo[] fields = typeof(Industries).GetFields();
			//csv_st.WriteLine(Mathf.FloorToInt(timeTotal) + "," + Mathf.FloorToInt(timeSinceLastPurchase) + "," + ApproximateIncome(fields) + "," + player.money + "," + text);
		}
		private class FirstRelics : IRelicMaker {
			public string relicDescription(ItemStack stack) {
				return "This relic predates recorded history.";
			}

			public string relicNames(ItemStack stack) {
				return "Lost";
			}
		}
	}
}
 