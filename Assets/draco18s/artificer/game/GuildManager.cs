using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;
using Koopakiller.Numerics;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.masters;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.config;
using System.Collections;

namespace Assets.draco18s.artificer.game {
	public class GuildManager {
		private static Text moneyDisp;
		private static Text renownDisp;
		private static Text skillDisp;
		private static Text newRenownDisp;
		private static Text numVend1;
		private static Text numVend2;
		private static Text numApp1;
		private static Text numApp2;
		private static Text numJour1;
		//private static Text numJour2;
		private static Text buyVendTxt;
		private static Text buyAppTxt;
		private static Text buyJourTxt;
		private static Text vendeffTxt;
		private static Text appeffTxt;
		private static Text joureffTxt;
		private static Transform cashList;
		private static Transform renownList;
		private static Transform premiumList;
		private static List<Upgrade> cashUpgradeList = new List<Upgrade>();
		private static List<Upgrade> renownUpgradeList = new List<Upgrade>();
		//private static List<Upgrade> premiumUpgradeList = new List<Upgrade>();
		private static bool hasListChanged = false;
		public static readonly string RENOWN_SYMBOL = "ℛ";
		private static BigInteger lastMoney = 0;
		private static Master[] availableMasters = new Master[3];

		public static void OneTimeSetup() {
			moneyDisp = GuiManager.instance.guildHeader.transform.Find("MoneyArea").GetChild(0).GetComponent<Text>();
			renownDisp = GuiManager.instance.guildHeader.transform.Find("GuildRenownArea").GetChild(0).GetComponent<Text>();
			Transform t = GuiManager.instance.guildHeader.transform.Find("RenownOnReset");
			newRenownDisp = t.GetChild(0).GetComponent<Text>();
			t.GetComponent<Button>().AddHover(delegate (Vector3 p) {
				/*BigInteger spentRenown = Main.instance.player.totalRenown - Main.instance.player.renown;
				BigInteger totalRenown = BigInteger.CubeRoot(Main.instance.player.lifetimeMoney);
				totalRenown /= 10000;
				BigInteger renown = totalRenown - spentRenown;*/
				BigInteger renown = Main.instance.getCachedNewRenown();

				GuiManager.ShowTooltip(p, "Renown from cash earned: " + Main.AsCurrency(renown) + RENOWN_SYMBOL + "\nRenown from completed quests: " + Main.AsCurrency(Main.instance.player.questsCompletedRenown) + RENOWN_SYMBOL, 5f);
			});
			skillDisp = GuiManager.instance.guildHeader.transform.Find("SkillPts").GetChild(0).GetComponent<Text>();
			cashList = GuiManager.instance.guildArea.transform.Find("CashUpgrades").GetChild(0).GetChild(0);
			renownList = GuiManager.instance.guildArea.transform.Find("RenownUpgrades").GetChild(0).GetChild(0);
			Transform tr = GuiManager.instance.guildArea.transform.Find("PremiumOpenAera");
			tr.gameObject.AddComponent<Button>().onClick.AddListener(delegate { HidePremium(); });
			premiumList = tr.GetChild(0).GetChild(1).GetChild(0).GetChild(0);
			GuiManager.instance.guildArea.transform.Find("PremiumBtn").GetComponent<Button>().onClick.AddListener(delegate { ShowPremium(); });
			buyVendTxt = GuiManager.instance.buyVendorsArea.transform.Find("BuyOne").GetChild(0).GetComponent<Text>();
			buyAppTxt = GuiManager.instance.buyApprenticesArea.transform.Find("BuyOne").GetChild(0).GetComponent<Text>();
			buyJourTxt = GuiManager.instance.buyJourneymenArea.transform.Find("BuyOne").GetChild(0).GetComponent<Text>();

			numVend1 = GuiManager.instance.buyVendorsArea.transform.Find("OwnedTxt").GetComponent<Text>();
			numVend2 = GuiManager.instance.buyVendorsArea.transform.Find("AvailableTxt").GetComponent<Text>();
			numApp1 = GuiManager.instance.buyApprenticesArea.transform.Find("OwnedTxt").GetComponent<Text>();
			numApp2 = GuiManager.instance.buyApprenticesArea.transform.Find("AvailableTxt").GetComponent<Text>();
			numJour1 = GuiManager.instance.buyJourneymenArea.transform.Find("OwnedTxt").GetComponent<Text>();
			//numJour2 = GuiManager.instance.buyJourneymenArea.transform.FindChild("AvailableTxt").GetComponent<Text>();

			vendeffTxt = GuiManager.instance.buyVendorsArea.transform.Find("EffectivenessTxt").GetComponent<Text>();//.text = Mathf.RoundToInt(Main.instance.player.GetVendorValue()*100) + "%";
			appeffTxt = GuiManager.instance.buyApprenticesArea.transform.Find("EffectivenessTxt").GetComponent<Text>();//.text = Main.instance.GetClickRate() + "sec / sec";
			joureffTxt = GuiManager.instance.buyJourneymenArea.transform.Find("EffectivenessTxt").GetComponent<Text>();//.text = Main.instance.GetClickRate() + "sec / sec";

			int i = 0;
			List<Upgrade> upgrades = Upgrades.AllCashUps;
			cashList.transform.hierarchyCapacity = (upgrades.Count + 1) * 5 + 1350;
			foreach(Upgrade item in upgrades) {
				GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, cashList) as GameObject;
				item.upgradListGui = it;
				cashUpgradeList.Add(item);
				it.name = item.displayName;
				it.transform.localPosition = new Vector3(6, i * -100 - 5, 0);

				it.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(item.displayName);
				it.transform.Find("Cost").GetComponent<Text>().text = "$" + Main.AsCurrency(item.cost);
				it.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.getIconName());
				Upgrade _item = item;
				Button btn = it.GetComponent<Button>();
				btn.onClick.AddListener(delegate { buyUpgrade(_item); });
				if(item.cost > Main.instance.player.money) {
					btn.interactable = false;
				}
				Upgrade up = item;
				btn.AddHover(delegate (Vector3 p) { GuiManager.ShowTooltip(btn.transform.position + Vector3.right * 90 + Vector3.down * 45,up.getTooltip(), 4f); }, false);

				i++;
			}
			((RectTransform)cashList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));
			cashList.localPosition = Vector3.zero;

			i = 0;
			upgrades = Upgrades.AllRenownUps;
			renownList.transform.hierarchyCapacity = (upgrades.Count + 1) * 5 + 1375;
			foreach(Upgrade item in upgrades) {
				GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, renownList) as GameObject;
				item.upgradListGui = it;
				renownUpgradeList.Add(item);
				it.name = item.displayName;
				it.transform.localPosition = new Vector3(6, i * -100 - 5, 0);

				it.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(item.displayName);
				it.transform.Find("Cost").GetComponent<Text>().text = Main.AsCurrency(item.cost) + RENOWN_SYMBOL;
				it.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.getIconName());
				Upgrade _item = item;
				Button btn = it.GetComponent<Button>();
				btn.onClick.AddListener(delegate { buyUpgradeRenown(_item); });
				if(item.cost > Main.instance.player.renown) {
					btn.interactable = false;
				}
				Upgrade up = item;
				btn.AddHover(delegate (Vector3 p) { GuiManager.ShowTooltip(btn.transform.position + Vector3.right * 90 + Vector3.down * 45, up.getTooltip(), 4f); }, false);

				i++;
			}
			((RectTransform)renownList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));
			renownList.localPosition = Vector3.zero;

			lastMoney = Main.instance.player.money;

			Button btn2 = GuiManager.instance.guildmasterArea.transform.Find("BuyOne").GetComponent<Button>();
			btn2.onClick.AddListener(delegate {
				if(Main.instance.player.totalRenown >= 100000)
					NewGuildmaster();
			});
			btn2.AddHover(delegate(Vector3 p) {
				if(Main.instance.player.totalRenown < 100000) {
					GuiManager.ShowTooltip(btn2.transform.position + Vector3.up * 60, "You need to have earned at least 100,000 renown to attract a new guildmaster.", 2.3f);
				}
			});

			int pts = 15 + SkillList.GuildmasterRating.getMultiplier();
			availableMasters[0] = Master.createRandomMaster(pts);
			availableMasters[1] = Master.createRandomMaster(pts);
			availableMasters[2] = Master.createRandomMaster(pts);

			for(int j = 1; j < availableMasters.Length+1; j++) {
				Transform gmb = GuiManager.instance.resetGuildWindow.transform.GetChild(1).Find("Guildmaster" + j);
				int q = j-1;
				gmb.GetComponent<Button>().onClick.AddListener(delegate { electGuildmaster(availableMasters[q]); });
			}

			i = 0;
			IEnumerator<Skill> list = SkillList.getSkillList();
			Transform skillListParent = GuiManager.instance.skillPanel.transform;
			while(list.MoveNext()) {
				Skill sk = list.Current;
				GameObject go = Main.Instantiate(PrefabManager.instance.SKILL_LISTITEM, skillListParent) as GameObject;
				sk.guiItem = go;
				go.transform.localPosition = new Vector3(5, i * -110 -5, 5);
				((RectTransform)go.transform).anchorMax = new Vector2(1,1);
				((RectTransform)go.transform).offsetMax = new Vector2(-3, ((RectTransform)go.transform).offsetMax.y);
				go.transform.Find("Name").GetComponent<Text>().text = Localization.translateToLocal(sk.name);
				go.transform.Find("Description").GetComponent<Text>().text = string.Format(Localization.translateToLocal(sk.description), sk.getMultiplierForDisplay());
				go.transform.Find("Ranks").GetComponent<Text>().text = "" + sk.getRanks();
				Transform t1 = go.transform.Find("BuyOne");
				t1.GetComponent<Button>().onClick.AddListener(delegate {
					doBuySkill(sk);
				});
				t1.GetChild(0).GetComponent<Text>().text = Main.AsCurrency(sk.getCost(1)) + " pts";
				i++;
			}
			((RectTransform)skillListParent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 110 + 10));
			renownList.localPosition = Vector3.zero;
			GuiManager.instance.resetGuildWindow.transform.GetChild(1).Find("CloseBtn").GetComponent<Button>().onClick.AddListener(closeNewGuildmaster);
			GuiManager.instance.resetGuildWindow.transform.GetChild(1).Find("CurrentMaster").GetComponent<Button>().onClick.AddListener(closeNewGuildmaster);
		}

		private static bool hasBeenSetup = false;
		public static void PremiumSetup(int divisor, string preIdent, string postIdent) {
			PremiumSetup(divisor, preIdent, postIdent, true);
		}
		public static void PremiumSetup(int divisor, string preIdent, string postIdent, bool enabled) {
			if(hasBeenSetup) return;
			hasBeenSetup = true;
			int i = 0;
			List<Upgrade> upgrades = PremiumUpgrades.AllPremiumUps;
			//premiumList.transform.hierarchyCapacity = (upgrades.Count + 1) * 5 + 1375;
			foreach(Upgrade item in upgrades) {
				GameObject go = Main.Instantiate(PrefabManager.instance.SKILL_LISTITEM, premiumList) as GameObject;
				item.upgradListGui = go;
				go.transform.localPosition = new Vector3(5, i * -110 - 5, 5);
				((RectTransform)go.transform).anchorMax = new Vector2(1, 1);
				((RectTransform)go.transform).offsetMax = new Vector2(-3, ((RectTransform)go.transform).offsetMax.y);
				go.transform.Find("Name").GetComponent<Text>().text = item.displayName;
				go.transform.Find("Description").GetComponent<Text>().text = Localization.translateToLocal("premium." + item.displayName + ".desc");
				go.transform.Find("Ranks").GetComponent<Text>().text = "";
				go.transform.Find("Label").GetComponent<Text>().text = (item.getIsPurchased() ? "Purchased!" : "");
				Transform t1 = go.transform.Find("BuyOne");
				if(enabled) {
					t1.GetComponent<Button>().onClick.AddListener(delegate {
						doBuyPremium(item);
					});
				}
				t1.gameObject.SetActive(!item.getIsPurchased());
				if(divisor == 100) {
					string cstr = Main.AsCurrency(item.cost);
					t1.GetChild(0).GetComponent<Text>().text = preIdent + (item.cost < 100 ? "0" : cstr.Substring(0, cstr.Length - 2)) + "." + cstr.Substring(cstr.Length - 2) + postIdent;
				}
				else {
					string cstr = Main.AsCurrency(BigRational.Round(new BigRational(item.cost)/divisor));
					t1.GetChild(0).GetComponent<Text>().text = preIdent + cstr + postIdent;
				}
				i++;
			}
			if(!enabled) {
				showPremiumError();
			}
			((RectTransform)premiumList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));
			premiumList.localPosition = Vector3.zero;
		}

		private static void showPremiumError() {
			GuiManager.instance.guildArea.transform.Find("PremiumOpenAera/PurchaseError").gameObject.SetActive(true);
		}

		private static void doBuySkill(Skill sk) {
			if((BigInteger)sk.getCost(1) <= Main.instance.player.skillPoints) {
				Main.instance.player.skillPoints -= (BigInteger)sk.getCost(1);
				sk.increaseRank(1);
				sk.guiItem.transform.Find("Ranks").GetComponent<Text>().text = "" + sk.getRanks();
				sk.guiItem.transform.Find("BuyOne").GetChild(0).GetComponent<Text>().text = Main.AsCurrency(sk.getCost(1)) + " pts";
				skillDisp.text = Main.AsCurrency(Main.instance.player.skillPoints);
			}
		}

		private static void doBuyPremium(Upgrade item) {
			if(KongregateAPI.instance.isLoaded) {
				KongregateAPI.doPurchase(item.saveName);
			}
		}

		private static void closeNewGuildmaster() {
			GuiManager.instance.resetGuildWindow.SetActive(false);
		}

		private static void electGuildmaster(Master master) {
			foreach(Industry ind in Main.instance.player.builtItems) {
				ind.apprentices = 0;
			}
			Main.instance.player.reset();
			BigInteger renown = Main.instance.player.totalRenown;
			Debug.Log(Main.instance.player.totalRenown + " | " + Main.instance.player.renown);
			Main.instance.player.skillPoints += renown / 10000;
			Main.instance.player.totalSkillPoints += renown / 10000;
			Main.instance.player.currentGuildmaster = master;
			Main.instance.player.totalRenown = 0;
			Main.instance.player.renown = 0;
			Main.instance.player.journeymen = 0;
			Main.instance.player.maxApprentices = 0;

			List<ItemStack> allRelics = new List<ItemStack>();
			allRelics.AddRange(QuestManager.availableRelics);
			QuestManager.availableRelics.Clear();
			allRelics.AddRange(Main.instance.player.unidentifiedRelics);
			Main.instance.player.unidentifiedRelics.Clear();
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				if(stack.relicData != null) allRelics.Add(stack);
			}
			Main.instance.player.miscInventory.Clear();
			List<ItemStack> specialItems = new List<ItemStack>();
			specialItems.AddRange(allRelics.FindAll(x => x.isSpecial()));
			allRelics.RemoveAll(x => x.isSpecial());
			allRelics.Sort((a, b) => {
				int besta = 0;
				int vala = a.antiquity * 10;
				foreach(RelicInfo ri in a.relicData) {
					besta = Math.Max(besta, ri.notoriety);
					vala += ri.notoriety;
				}
				int bestb = 0;
				int valb = b.antiquity * 10;
				foreach(RelicInfo ri in b.relicData) {
					bestb = Math.Max(bestb, ri.notoriety);
					valb += ri.notoriety;
				}
				besta += vala;
				bestb += valb;
				return bestb.CompareTo(besta);
			});
			allRelics.RemoveRange(Math.Min(10, allRelics.Count-1), allRelics.Count - Math.Min(10, allRelics.Count - 1));
			allRelics.AddRange(specialItems);
			int best = 0;
			foreach(ItemStack stack in allRelics) {
				best = Math.Max(++stack.antiquity, best);
				if(stack.antiquity >= 50) {
					StatisticsTracker.impressiveAntiquity.setAchieved();
				}
				if(stack.antiquity <= 0) {
					Debug.Log("How the hell did this happen?");
					Debug.Log(stack.getDisplayName() + " has 0 antiquity.");
					ItemStack s = stack;
					List<string> strList = new List<string>();
					if(s.enchants.Count > 0) {
						//strList.Add("Enchanted:");
						foreach(string en in s.enchants.Select(x => x.name).Distinct()) {
							int num = s.enchants.Count(x => x.name == en);
							if(num > 1) {
								strList.Add(en + " (x" + num + ")");
							}
							else {
								strList.Add(en);
							}
						}
					}
					string str = string.Join(", ", strList.ToArray());
					strList.Clear();
					strList.Add(s.antiquity + " Antiquity");
					if(s.enchants.Count > 0) {
						strList.Add("Enchanted:");
						strList.Add(" - " + str);
					}
					foreach(RelicInfo inf in s.relicData) {
						strList.Add(inf.heroName + "\n   " + inf.questDescription + " (" + inf.notoriety + ")");
						if(strList.Count > 3) break;
					}
					str = string.Join("\n", strList.ToArray());

					Debug.Log(str);
				}
				stack.isIDedByPlayer = false;
				if(stack.relicData.Any(x => x.relicName == "Lost")) {
					stack.relicData = null;
					QuestManager.makeRelic(stack, new Main.FirstRelics(), 1, "Unknown");
				}
				else {
					stack.relicData = null;
					QuestManager.availableRelics.Add(QuestManager.makeRelic(stack, new AntiquityRelics(), 1, "Unknown"));
				}
			}
			StatisticsTracker.relicAntiquity.resetValue();
			StatisticsTracker.relicAntiquity.setValue(best);

			int pts = 15 + SkillList.GuildmasterRating.getMultiplier();
			availableMasters[0] = Master.createRandomMaster(pts);
			availableMasters[1] = Master.createRandomMaster(pts);
			availableMasters[2] = Master.createRandomMaster(pts);

			GuiManager.instance.guildmasterArea.transform.Find("OwnedTxt").GetComponent<Text>().text = Main.instance.player.currentGuildmaster.getDisplay();
			if(!StatisticsTracker.firstGuildmaster.isAchieved()) {
				StatisticsTracker.firstGuildmaster.setAchieved();
				StatisticsTracker.maxQuestDifficulty.addValue(2);
				StatisticsTracker.minQuestDifficulty.addValue(1);
			}
			closeNewGuildmaster();
			GuiManager.instance.guildArea.transform.Find("SkillPanel").Find("Skills").gameObject.SetActive(Main.instance.player.totalSkillPoints > 0);
			skillDisp.transform.parent.gameObject.SetActive(Main.instance.player.totalSkillPoints > 0);
			skillDisp.text = Main.AsCurrency(Main.instance.player.skillPoints);
			StatisticsTracker.guildmastersElected.addValue(1);
			closeNewGuildmaster();
		}

		public static void setupUI() {
			hasListChanged = true;
			Transform gmb = GuiManager.instance.resetGuildWindow.transform.GetChild(1).Find("CurrentMaster");
			gmb.GetChild(0).GetComponent<Text>().text = "";
			gmb.GetChild(1).GetComponent<Text>().text = Main.instance.player.currentGuildmaster.getDisplay();
			for(int i = 0; i < availableMasters.Length; i++) {
				gmb = GuiManager.instance.resetGuildWindow.transform.GetChild(1).Find("Guildmaster" + (i+1));
				gmb.GetChild(0).GetComponent<Text>().text = "";
				gmb.GetChild(1).GetComponent<Text>().text = availableMasters[i].getDisplay();
			}
			GuiManager.instance.guildmasterArea.transform.Find("BuyOne").GetComponent<Button>().interactable = Main.instance.player.totalRenown >= 100000;
			GuiManager.instance.guildmasterArea.transform.Find("OwnedTxt").GetComponent<Text>().text = Main.instance.player.currentGuildmaster.getDisplay();
			GuiManager.instance.guildArea.transform.Find("SkillPanel").Find("Skills").gameObject.SetActive(Main.instance.player.totalSkillPoints > 0);
			skillDisp.transform.parent.gameObject.SetActive(Main.instance.player.totalSkillPoints > 0);
			skillDisp.text = Main.AsCurrency(Main.instance.player.skillPoints);

			IEnumerator<Skill> list = SkillList.getSkillList();
			Transform skillListParent = GuiManager.instance.skillPanel.transform;
			while(list.MoveNext()) {
				Skill sk = list.Current;
				GameObject go = sk.guiItem;
				go.transform.Find("Ranks").GetComponent<Text>().text = "" + sk.getRanks();
			}
			if(hasBeenSetup) {
				List<Upgrade> upgrades = PremiumUpgrades.AllPremiumUps;
				foreach(Upgrade item in upgrades) {
					Transform t1 = item.upgradListGui.transform.Find("BuyOne");
					item.upgradListGui.transform.Find("Label").GetComponent<Text>().text = (item.getIsPurchased() ? "Purchased!" : "");
					t1.gameObject.SetActive(!item.getIsPurchased());
				}
			}
			else {
				PremiumSetup(100, "$", "", false);
			}
		}

		public static void update() {
			renownDisp.text = Main.AsCurrency(Main.instance.player.renown)+ RENOWN_SYMBOL;//₹

			/*BigInteger spentRenown = Main.instance.player.totalRenown - Main.instance.player.renown;
			BigInteger totalRenown = BigInteger.CubeRoot(Main.instance.player.lifetimeMoney);
			totalRenown /= 10000;
			BigInteger renown = totalRenown - spentRenown + Main.instance.player.questsCompleted;*/
			BigInteger renown = Main.instance.getCachedNewRenown() + Main.instance.player.questsCompletedRenown;// + Main.instance.player.totalRenown - spentRenown;

			newRenownDisp.text = Main.AsCurrency(renown + Main.instance.player.renown) + RENOWN_SYMBOL;//𐍈☼

			moneyDisp.text = "$" + Main.AsCurrency(Main.instance.player.money);

			numVend1.text = "" + Main.instance.player.maxVendors;
			numVend2.text = "" + (Main.instance.player.maxVendors-Main.instance.player.currentVendors);
			numApp1.text = "" + Main.instance.player.maxApprentices;
			numApp2.text = "" + (Main.instance.player.maxApprentices - Main.instance.player.currentApprentices);
			numJour1.text = "" + Main.instance.player.journeymen;
			buyVendTxt.text = "+1 ($" + Main.AsCurrency(getVendorCost()) + ")";
			buyAppTxt.text = "+1 (" + Main.AsCurrency(getApprenticeCost()) + RENOWN_SYMBOL + ")";
			buyJourTxt.text = "+1 (" + Main.AsCurrency(getJourneymenCost()) + RENOWN_SYMBOL + ")";
			joureffTxt.text = (2 * Main.instance.player.journeymen) + " Items / " + Main.SecondsToTime(QuestManager.getEquipRate() / Main.instance.player.currentGuildmaster.journeymenRateMultiplier());
			
			vendeffTxt.text = (Main.instance.player.GetVendorValue() * 100).ToDecimalString(0) + "%";
			if(Main.instance.player.currentGuildmaster.apprenticeRateMultiplier() != 1) {
				float v = Main.instance.player.GetApprenticeClickAmount();
				v *= Main.instance.player.GetApprenticeClickSpeedMultiplier();
				appeffTxt.text = Main.instance.GetClickRate() + "sec / sec, (app: " + Mathf.Round(v*100)/100 + "sec)";
			}
			else {
				appeffTxt.text = Main.instance.GetClickRate() + "sec / sec";
			}
			BigInteger mon = Main.instance.player.money;
			BigInteger diff = BigInteger.Abs((lastMoney - mon));
			if(!hasListChanged && diff >= (0.005 * (BigRational)mon)) {
				int j;
				bool b;
				for(j = 0, b = true; j < cashUpgradeList.Count && b; j++) {
					if(!cashUpgradeList[j].getIsPurchased()) {
						b = false;
					}
				}
				j--;
				if(!b) {
					BigInteger c = cashUpgradeList[j].cost;
					if((c > lastMoney && c <= mon) || (c <= lastMoney && c > mon)) {
						hasListChanged = true;
					}
				}
				lastMoney = mon;
			}
			if(hasListChanged) {
				hasListChanged = false;
				int i = 0;
				List<Industry> haveHalfDoubleFor = new List<Industry>();
				foreach(Upgrade item in cashUpgradeList) {
					if(!item.getIsPurchased() && (item.cost < Main.instance.player.money * 10 || i < 10)) {
						if(item.upgradListGui == null) {
							GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, cashList) as GameObject;
							item.upgradListGui = it;
							it.name = item.displayName;
							it.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(item.displayName);
							it.transform.Find("Cost").GetComponent<Text>().text = "$" + Main.AsCurrency(item.cost);
							it.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.getIconName());
							Upgrade _item = item;
							Button btn = it.GetComponent<Button>();
							btn.onClick.AddListener(delegate { buyUpgrade(_item); });
							Upgrade up = item;
							btn.AddHover(delegate (Vector3 p) { GuiManager.ShowTooltip(btn.transform.position + Vector3.right * 90 + Vector3.down * 45,up.getTooltip(), 4f); }, false);
						}
						item.upgradListGui.name = item.displayName;
						item.upgradListGui.transform.localPosition = new Vector3(6, i * -100 - 5, 0);

						if(item.cost > Main.instance.player.money) {
							item.upgradListGui.GetComponent<Button>().interactable = false;
							//item.upgradListGui.GetComponent<Image>().color = Color.red;
						}
						else {
							item.upgradListGui.GetComponent<Button>().interactable = true;
							//item.upgradListGui.GetComponent<Image>().color = Color.white;
						}
						//Hide half-doubles until the industry is level 100
						if(item is UpgradeHalveDouble) {
							UpgradeHalveDouble upgrade = (UpgradeHalveDouble)item;
							if(upgrade.affectedIndustry.level < 100 || haveHalfDoubleFor.Contains(upgrade.affectedIndustry)) {
								i--;
								Main.Destroy(item.upgradListGui);
							}
							haveHalfDoubleFor.Add(upgrade.affectedIndustry);
						}

						i++;
					}
					else {
						if(item.upgradListGui != null) {
							Main.Destroy(item.upgradListGui);
						}
					}
				}
				if(i == 0) {
					StatisticsTracker.allCashUpgrades.setAchieved();
				}
				((RectTransform)cashList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));

				i = 0;
				foreach(Upgrade item in renownUpgradeList) {
					//Debug.Log(item.cost + " < " + (Main.instance.player.renown * 10) + " || " + i + " < 10");
					if(!item.getIsPurchased() && (item.cost < Main.instance.player.renown * 10 || i < 10)) {
						if(item.upgradListGui == null) {
							GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM,renownList) as GameObject;
							item.upgradListGui = it;
							it.name = item.displayName;
							it.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(item.displayName);
							it.transform.Find("Cost").GetComponent<Text>().text = Main.AsCurrency(item.cost) + RENOWN_SYMBOL;
							it.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.getIconName());
							Upgrade _item = item;
							Button btn = it.GetComponent<Button>();
							btn.onClick.AddListener(delegate { buyUpgradeRenown(_item); });
							Upgrade up = item;
							btn.AddHover(delegate (Vector3 p) { GuiManager.ShowTooltip(btn.transform.position + Vector3.right * 90 + Vector3.down * 45, up.getTooltip(), 4f); }, false);
						}
						item.upgradListGui.name = item.displayName;
						item.upgradListGui.transform.localPosition = new Vector3(6, i * -100 - 5, 0);

						if(item.cost > Main.instance.player.renown) {
							item.upgradListGui.GetComponent<Button>().interactable = false;
							//item.upgradListGui.GetComponent<Image>().color = Color.red;
						}
						else {
							item.upgradListGui.GetComponent<Button>().interactable = true;
							//item.upgradListGui.GetComponent<Image>().color = Color.white;
							if(item.cost > Main.instance.player.renown / 4 * 3) {
								item.upgradListGui.GetComponent<Image>().color = ColorHelper.ORANGE;
							}
							else if(item.cost > Main.instance.player.renown / 4) {
								item.upgradListGui.GetComponent<Image>().color = Color.yellow;
							}
							else {
								item.upgradListGui.GetComponent<Image>().color = Color.white;
							}
						}

						i++;
					}
					else {
						if(item.upgradListGui != null) {
							Main.Destroy(item.upgradListGui);
						}
					}
				}
				if(i == 0) {
					StatisticsTracker.allRenownUpgrades.setAchieved();
				}
				((RectTransform)renownList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));
				//listGui.localPosition = Vector3.zero;
			}
		}

		public static void buyUpgrade(Upgrade item) {
			if(item.cost <= Main.instance.player.money) {
				Main.instance.player.money -= item.cost;
				item.applyUpgrade();
				Main.instance.writeCSVLine("Bought " + item.displayName);
				hasListChanged = true;
			}
		}

		public static void buyUpgradeRenown(Upgrade item) {
			if(item.cost > Main.instance.player.renown / 4) {
				//TODO: confirmation
			}
			if(item.cost <= Main.instance.player.renown) {
				Main.instance.player.renown -= item.cost;
				item.applyUpgrade();
				Main.instance.writeCSVLine("Bought " + item.displayName);
				hasListChanged = true;
			}
		}

		public static void resetAllUpgrades() {
			int i = 0;
			List<Upgrade> upgrades = Upgrades.AllCashUps;
			foreach(Upgrade item in upgrades) {
				if(item.getIsPurchased()) {
					item.revokeUpgrade();
				}
			}
			upgrades = Upgrades.AllRenownUps;
			foreach(Upgrade item in upgrades) {
				if(item.getIsPurchased()) {
					item.revokeUpgrade();
				}
			}
		}

		protected static BigInteger getVendorCost() {
			BigInteger c = 27000;
			int vend = Main.instance.player.maxVendors - 5;
			for(;vend>0;vend--) {
				c *= 3;
			}
			return c;
		}

		protected static BigInteger getApprenticeCost() {
			BigInteger c = 10;
			int vend = Main.instance.player.maxApprentices;
			for(; vend > 0; vend--) {
				c *= 4;
			}
			return c;
		}

		protected static BigInteger getJourneymenCost() {
			BigInteger c = 25;
			int vend = Main.instance.player.journeymen;
			for(; vend > 0; vend--) {
				c *= 5;
			}
			return c;
		}

		public static void BuyVendor() {
			BigInteger cost = getVendorCost();
			if(Main.instance.player.money >= cost) {
				Main.instance.player.money -= cost;
				Main.instance.player.maxVendors += 1;
				StatisticsTracker.vendorsPurchased.setValue(Main.instance.player.maxVendors);
			}
		}

		public static void BuyApprentice() {
			BigInteger cost = getApprenticeCost();
			if(Main.instance.player.renown >= cost) {
				Main.instance.player.renown -= cost;
				Main.instance.player.maxApprentices += 1;
				StatisticsTracker.apprenticesPurchased.addValue(1);
			}
		}

		public static void BuyJourneyman() {
			BigInteger cost = getJourneymenCost();
			if(Main.instance.player.renown >= cost) {
				Main.instance.player.renown -= cost;
				Main.instance.player.journeymen += 1;
				StatisticsTracker.journeymenPurchased.addValue(1);
			}
		}

		public static void NewGuildmaster() {
			GuiManager.instance.resetGuildWindow.SetActive(true);
		}

		private static void ShowPremium() {
			GuiManager.instance.guildArea.transform.Find("PremiumOpenAera").gameObject.SetActive(true);
		}

		private static void HidePremium() {
			GuiManager.instance.guildArea.transform.Find("PremiumOpenAera").gameObject.SetActive(false);
		}

		public static void writeSaveData(ref SerializationInfo info, ref StreamingContext context) {
			foreach(Upgrade item in cashUpgradeList) {
				info.AddValue("upgrade_" + item.saveName, item.getIsPurchased());
			}
			foreach(Upgrade item in renownUpgradeList) {
				info.AddValue("renown_upgrade_" + item.saveName, item.getIsPurchased());
			}
			for(int i=0; i < availableMasters.Length; i++) {
				info.AddValue("availableMasters_"+i,availableMasters[i]);
			}
			SkillList.writeSaveData(ref info, ref context);
		}

		public static void readSaveData(ref SerializationInfo info, ref StreamingContext context) {
			SerializationInfoEnumerator infoEnum = info.GetEnumerator();
			Hashtable values = new Hashtable();
			while(infoEnum.MoveNext()) {
				SerializationEntry val = infoEnum.Current;
				values.Add(val.Name, val.Value);
			}
			if(Main.saveVersionFromDisk >= 11) {
				foreach(Upgrade item in cashUpgradeList) {
					if(values.Contains("upgrade_" + item.saveName) && (bool)values["upgrade_" + item.saveName]) {
						item.applyUpgrade();
					}
				}
				foreach(Upgrade item in renownUpgradeList) {
					if(values.Contains("renown_upgrade_" + item.saveName) && (bool)values["renown_upgrade_" + item.saveName]) {
						item.applyUpgrade();
					}
				}
			}
			else if(Main.saveVersionFromDisk >= 4) {
				foreach(Upgrade item in cashUpgradeList) {
					try {
						if(info.GetBoolean("upgrade_" + item.saveName)) {
							item.applyUpgrade();
						}
						//item.setIsPurchased(info.GetBoolean("upgrade_" + item.saveName));
					}
					catch (SerializationException e) {
						Debug.Log(e);
					}
				}
				if(Main.saveVersionFromDisk >= 6) {
					foreach(Upgrade item in renownUpgradeList) {
						try {
							if(info.GetBoolean("renown_upgrade_" + item.saveName)) {
								item.applyUpgrade();
							}
							//item.setIsPurchased(info.GetBoolean("renown_upgrade_" + item.saveName));
						}
						catch(SerializationException e) {
							Debug.Log(e);
						}
					}
				}
			}
			else {
				int i = 0;
				foreach(Upgrade item in cashUpgradeList) {
					if(i < 16) {
						item.setIsPurchased(info.GetBoolean("upgrade_" + i));
					}
					i++;
				}
			}
			if(Main.saveVersionFromDisk >= 8) {
				for(int i = 0; i < availableMasters.Length; i++) {
					availableMasters[i] = (Master)info.GetValue("availableMasters_" + i, typeof(Master));
				}
			}
			hasListChanged = true;
			SkillList.readSaveData(ref info, ref context);
		}
		private class AntiquityRelics : IRelicMaker {
			public string relicDescription(ItemStack stack) {
				return "This relic predates the current age.";
			}

			public string relicNames(ItemStack stack) {
				return "Ancient";
			}
		}
	}
}
