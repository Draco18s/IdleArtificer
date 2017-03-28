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

namespace Assets.draco18s.artificer.game {
	class GuildManager {
		private static Text moneyDisp;
		private static Text renownDisp;
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
		private static List<Upgrade> cashUpgradeList = new List<Upgrade>();
		private static List<Upgrade> renownUpgradeList = new List<Upgrade>();
		private static bool hasListChanged = false;
		public static readonly string RENOWN_SYMBOL = "ℛ";
		private static BigInteger lastMoney = 0;

		public static void OneTimeSetup() {
			moneyDisp = GuiManager.instance.guildHeader.transform.FindChild("MoneyArea").GetChild(0).GetComponent<Text>();
			renownDisp = GuiManager.instance.guildHeader.transform.FindChild("GuildRenownArea").GetChild(0).GetComponent<Text>();
			Transform t = GuiManager.instance.guildHeader.transform.FindChild("RenownOnReset");
			newRenownDisp = t.GetChild(0).GetComponent<Text>();
			t.GetComponent<Button>().AddHover(delegate (Vector3 p) {
				/*BigInteger spentRenown = Main.instance.player.totalRenown - Main.instance.player.renown;
				BigInteger totalRenown = BigInteger.CubeRoot(Main.instance.player.lifetimeMoney);
				totalRenown /= 10000;
				BigInteger renown = totalRenown - spentRenown;*/
				BigInteger renown = Main.instance.getCachedNewRenown();

				GuiManager.ShowTooltip(p, "Renown from cash on hand: " + Main.AsCurrency(renown) + RENOWN_SYMBOL + "\nRenown from completed quests: " + Main.AsCurrency(Main.instance.player.questsCompleted) + RENOWN_SYMBOL, 5f);
			});
			cashList = GuiManager.instance.guildArea.transform.FindChild("CashUpgrades").GetChild(0).GetChild(0);
			renownList = GuiManager.instance.guildArea.transform.FindChild("RenownUpgrades").GetChild(0).GetChild(0);
			buyVendTxt = GuiManager.instance.buyVendorsArea.transform.FindChild("BuyOne").GetChild(0).GetComponent<Text>();
			buyAppTxt = GuiManager.instance.buyApprenticesArea.transform.FindChild("BuyOne").GetChild(0).GetComponent<Text>();
			buyJourTxt = GuiManager.instance.buyJourneymenArea.transform.FindChild("BuyOne").GetChild(0).GetComponent<Text>();

			numVend1 = GuiManager.instance.buyVendorsArea.transform.FindChild("OwnedTxt").GetComponent<Text>();
			numVend2 = GuiManager.instance.buyVendorsArea.transform.FindChild("AvailableTxt").GetComponent<Text>();
			numApp1 = GuiManager.instance.buyApprenticesArea.transform.FindChild("OwnedTxt").GetComponent<Text>();
			numApp2 = GuiManager.instance.buyApprenticesArea.transform.FindChild("AvailableTxt").GetComponent<Text>();
			numJour1 = GuiManager.instance.buyJourneymenArea.transform.FindChild("OwnedTxt").GetComponent<Text>();
			//numJour2 = GuiManager.instance.buyJourneymenArea.transform.FindChild("AvailableTxt").GetComponent<Text>();

			vendeffTxt = GuiManager.instance.buyVendorsArea.transform.FindChild("EffectivenessTxt").GetComponent<Text>();//.text = Mathf.RoundToInt(Main.instance.player.GetVendorValue()*100) + "%";
			appeffTxt = GuiManager.instance.buyApprenticesArea.transform.FindChild("EffectivenessTxt").GetComponent<Text>();//.text = Main.instance.GetClickRate() + "sec / sec";
			joureffTxt = GuiManager.instance.buyJourneymenArea.transform.FindChild("EffectivenessTxt").GetComponent<Text>();//.text = Main.instance.GetClickRate() + "sec / sec";

			int i = 0;
			FieldInfo[] fields = typeof(Upgrades.Cash).GetFields();
			cashList.transform.hierarchyCapacity = (fields.Length + 1) * 5 + 1350;
			foreach(FieldInfo field in fields) {
				//buildButtons.Add(it);
				Upgrade item = (Upgrade)field.GetValue(null);
				if(!item.getIsPurchased()) {
					GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, cashList) as GameObject;
					item.upgradListGui = it;
					cashUpgradeList.Add(item);
					it.name = item.displayName;
					it.transform.localPosition = new Vector3(6, i * -100 - 5, 0);

					it.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(item.displayName);
					it.transform.FindChild("Cost").GetComponent<Text>().text = "$" + Main.AsCurrency(item.cost);
					it.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.getIconName());
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
			}
			((RectTransform)cashList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));
			cashList.localPosition = Vector3.zero;

			i = 0;
			fields = typeof(Upgrades.Renown).GetFields();
			renownList.transform.hierarchyCapacity = (fields.Length + 1) * 5 + 1375;
			foreach(FieldInfo field in fields) {
				//buildButtons.Add(it);
				Upgrade item = (Upgrade)field.GetValue(null);
				if(!item.getIsPurchased()) {
					GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, renownList) as GameObject;
					item.upgradListGui = it;
					renownUpgradeList.Add(item);
					it.name = item.displayName;
					it.transform.localPosition = new Vector3(6, i * -100 - 5, 0);

					it.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(item.displayName);
					it.transform.FindChild("Cost").GetComponent<Text>().text = Main.AsCurrency(item.cost) + RENOWN_SYMBOL;
					it.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.getIconName());
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
			}
			((RectTransform)renownList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));
			renownList.localPosition = Vector3.zero;

			lastMoney = Main.instance.player.money;
		}
		public static void setupUI() {
		}

		public static void update() {
			renownDisp.text = Main.AsCurrency(Main.instance.player.renown)+ RENOWN_SYMBOL;//₹

			/*BigInteger spentRenown = Main.instance.player.totalRenown - Main.instance.player.renown;
			BigInteger totalRenown = BigInteger.CubeRoot(Main.instance.player.lifetimeMoney);
			totalRenown /= 10000;
			BigInteger renown = totalRenown - spentRenown + Main.instance.player.questsCompleted;*/
			BigInteger renown = Main.instance.getCachedNewRenown() + Main.instance.player.questsCompleted;

			newRenownDisp.text = Main.AsCurrency(Main.instance.player.renown + renown)+ RENOWN_SYMBOL;//𐍈☼

			moneyDisp.text = "$" + Main.AsCurrency(Main.instance.player.money);

			numVend1.text = "" + Main.instance.player.maxVendors;
			numVend2.text = "" + (Main.instance.player.maxVendors-Main.instance.player.currentVendors);
			numApp1.text = "" + Main.instance.player.maxApprentices;
			numApp2.text = "" + (Main.instance.player.maxApprentices - Main.instance.player.currentApprentices);
			numJour1.text = "" + Main.instance.player.journeymen;
			buyVendTxt.text = "+1 ($" + Main.AsCurrency(getVendorCost()) + ")";
			buyAppTxt.text = "+1 (" + Main.AsCurrency(getApprenticeCost()) + RENOWN_SYMBOL + ")";
			buyJourTxt.text = "+1 (" + Main.AsCurrency(getJourneymenCost()) + RENOWN_SYMBOL + ")";
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.JOURNEYMAN_RATE, out wrap);
			joureffTxt.text = (2 * Main.instance.player.journeymen) + " Items / " + Main.SecondsToTime(ResearchManager.maxResearchTime / ((UpgradeFloatValue)wrap).value);

			vendeffTxt.text = Mathf.RoundToInt(Main.instance.player.GetVendorValue() * 100) + "%";
			appeffTxt.text = Main.instance.GetClickRate() + "sec / sec";
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
				foreach(Upgrade item in cashUpgradeList) {
					if(!item.getIsPurchased() && (item.cost < Main.instance.player.money * 10 || i < 10)) {
						if(item.upgradListGui == null) {
							GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, cashList) as GameObject;
							item.upgradListGui = it;
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

						i++;
					}
					else {
						if(item.upgradListGui != null) {
							Main.Destroy(item.upgradListGui);
						}
					}
				}
				((RectTransform)cashList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 100 + 10));

				i = 0;
				foreach(Upgrade item in renownUpgradeList) {
					Debug.Log(item.cost + " < " + (Main.instance.player.renown * 10) + " || " + i + " < 10");
					if(!item.getIsPurchased() && (item.cost < Main.instance.player.renown * 10 || i < 10)) {
						if(item.upgradListGui == null) {
							GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM,renownList) as GameObject;
							item.upgradListGui = it;
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
						}

						i++;
					}
					else {
						if(item.upgradListGui != null) {
							Main.Destroy(item.upgradListGui);
						}
					}
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
			if(item.cost <= Main.instance.player.renown) {
				Main.instance.player.renown -= item.cost;
				item.applyUpgrade();
				Main.instance.writeCSVLine("Bought " + item.displayName);
				hasListChanged = true;
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
				c *= 10;
			}
			return c;
		}

		protected static BigInteger getJourneymenCost() {
			BigInteger c = 25;
			int vend = Main.instance.player.journeymen;
			for(; vend > 0; vend--) {
				c *= 15;
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
			}
		}

		public static void BuyJourneyman() {
			BigInteger cost = getJourneymenCost();
			if(Main.instance.player.renown >= cost) {
				Main.instance.player.renown -= cost;
				Main.instance.player.journeymen += 1;
			}
		}

		public static void writeSaveData(ref SerializationInfo info, ref StreamingContext context) {
			foreach(Upgrade item in cashUpgradeList) {
				info.AddValue("upgrade_" + item.saveName, item.getIsPurchased());
			}
			foreach(Upgrade item in renownUpgradeList) {
				info.AddValue("renown_upgrade_" + item.saveName, item.getIsPurchased());
			}
		}

		public static void readSaveData(ref SerializationInfo info, ref StreamingContext context) {
			if(Main.saveVersionFromDisk >= 4) {
				foreach(Upgrade item in cashUpgradeList) {
					try {
						if(info.GetBoolean("upgrade_" + item.saveName)) {
							item.applyUpgrade();
						}
						//item.setIsPurchased(info.GetBoolean("upgrade_" + item.saveName));
					}
					catch (SerializationException e) {
						//Debug.Log(e);
					}
				}
				if(Main.saveVersionFromDisk >= 6) {
					foreach(Upgrade item in cashUpgradeList) {
						try {
							if(info.GetBoolean("renown_upgrade_" + item.saveName)) {
								item.applyUpgrade();
							}
							//item.setIsPurchased(info.GetBoolean("renown_upgrade_" + item.saveName));
						}
						catch(SerializationException e) {
							//Debug.Log(e);
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
			hasListChanged = true;
		}
	}
}
