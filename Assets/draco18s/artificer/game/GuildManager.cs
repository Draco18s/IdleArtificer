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
		private static Text buyVendTxt;
		private static Text buyAppTxt;
		private static Text vendeffTxt;
		private static Text appeffTxt;
		private static Transform cashList;
		private static Transform renownList;
		private static Transform relicList;
		private static List<Upgrade> cashUpgradeList = new List<Upgrade>();
		private static bool hasListChanged = false;
		public static readonly string RENOWN_SYMBOL = "ℛ";
		private static BigInteger lastMoney = 0;
		private static Dictionary<ItemStack, GameObject> relicsList = new Dictionary<ItemStack, GameObject>();

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
			relicList = GuiManager.instance.guildArea.transform.FindChild("RelicsList").GetChild(0).GetChild(0);
			buyVendTxt = GuiManager.instance.buyVendorsArea.transform.FindChild("BuyOne").GetChild(0).GetComponent<Text>();
			buyAppTxt = GuiManager.instance.buyApprenticesArea.transform.FindChild("BuyOne").GetChild(0).GetComponent<Text>();

			numVend1 = GuiManager.instance.buyVendorsArea.transform.FindChild("OwnedTxt").GetComponent<Text>();
			numVend2 = GuiManager.instance.buyVendorsArea.transform.FindChild("AvailableTxt").GetComponent<Text>();
			numApp1 = GuiManager.instance.buyApprenticesArea.transform.FindChild("OwnedTxt").GetComponent<Text>();
			numApp2 = GuiManager.instance.buyApprenticesArea.transform.FindChild("AvailableTxt").GetComponent<Text>();

			vendeffTxt = GuiManager.instance.buyVendorsArea.transform.FindChild("EffectivenessTxt").GetComponent<Text>();//.text = Mathf.RoundToInt(Main.instance.player.GetVendorValue()*100) + "%";
			appeffTxt = GuiManager.instance.buyApprenticesArea.transform.FindChild("EffectivenessTxt").GetComponent<Text>();//.text = Main.instance.GetClickRate() + "sec / sec";

			relicList.transform.hierarchyCapacity = 100 * 20;

			int i = 0;
			FieldInfo[] fields = typeof(Upgrades.Cash).GetFields();
			cashList.transform.hierarchyCapacity = (fields.Length+1)*60;
			foreach(FieldInfo field in fields) {
				//buildButtons.Add(it);
				Upgrade item = (Upgrade)field.GetValue(null);
				if(!item.getIsPurchased()) {
					GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM, cashList) as GameObject;
					item.upgradListGui = it;
					cashUpgradeList.Add(item);
					it.name = item.displayName;
					//it.transform.SetParent(cashList);
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
			lastMoney = Main.instance.player.money;
		}
		public static void setupUI() {
			int i = 0;
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				if(stack.relicData != null) {
					//Debug.Log("i: " + i);
					//Debug.Log(stack.item.name);
					GameObject go;
					relicsList.TryGetValue(stack, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM, relicList) as GameObject;
						//go.transform.SetParent(relicList);
						relicsList.Add(stack, go);
					}
					go.transform.localPosition = new Vector3((i % 4) * 98 + 5, ((i / 4) * -125) - 5, 0);
					//Debug.Log(go.transform.parent.name + ":" + go.transform.localPosition);
					Text tx = go.transform.FindChild("Title").GetComponent<Text>();
					tx.text = Main.ToTitleCase(stack.getDisplayName());
					go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
					go.transform.FindChild("Quantity").GetComponent<Text>().text = "";
					ItemStack s = stack;
					Button btn = go.GetComponent<Button>();
					btn.AddHover(delegate (Vector3 p) {
						string str = "";
						foreach(RelicInfo ri in s.relicData) {
							str += ri.questDescription + " (" + ri.notoriety + ")\n";
						}
						if(s.enchants.Count > 0) {
							str += "Enchanted:\n";
							foreach(Enchantment en in s.enchants) {
								str += en.name + "\n";
							}
						}
						GuiManager.ShowTooltip(btn.transform.position, str);
					});
					int req_num = 1;
					long ty = (long)stack.getAllReqs();
					bool abort = false;
					for(int r = 1; r <= 5; r++) {
						if(ty == 0) abort = true;
						while((ty & 1) == 0 && ty > 0) {
							req_num++;
							ty = ty >> 1;
							if(ty == 0) abort = true;
						}
						if(abort) {
							go.transform.FindChild("Req" + r).gameObject.SetActive(false);
						}
						else {
							go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
							ty = ty >> 1;
							ty = ty << 1;
						}
					}
					i++;
				}
			}
			((RectTransform)relicList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((i/4) * 100 + 10));
			relicList.localPosition = Vector3.zero;
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
			buyVendTxt.text = "+1 ($" + Main.AsCurrency(getVendorCost()) + ")";
			buyAppTxt.text = "+1 (" + Main.AsCurrency(getApprenticeCost()) + RENOWN_SYMBOL + ")";

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
					if(!item.getIsPurchased()) {
						if(item.upgradListGui == null) {
							GameObject it = Main.Instantiate(PrefabManager.instance.UPGRADE_GUI_LISTITEM);
							item.upgradListGui = it;
						}
						item.upgradListGui.name = item.displayName;
						item.upgradListGui.transform.SetParent(cashList);
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

		protected static BigInteger getVendorCost() {
			BigInteger c = 27000;
			int vend = Main.instance.player.maxVendors - 5;
			for(;vend>0;vend--) {
				c *= 3;
			}
			return c;
		}

		protected static BigInteger getApprenticeCost() {
			BigInteger c = 100;
			int vend = Main.instance.player.maxApprentices;
			for(; vend > 0; vend--) {
				c *= 100;
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
			Debug.Log(Main.instance.player.renown + ">=" + cost);
			if(Main.instance.player.renown >= cost) {
				Main.instance.player.renown -= cost;
				Main.instance.player.maxApprentices += 1;
			}
		}

		public static void writeSaveData(ref SerializationInfo info, ref StreamingContext context) {
			foreach(Upgrade item in cashUpgradeList) {
				info.AddValue("upgrade_" + item.saveName, item.getIsPurchased());
			}
		}

		public static void readSaveData(ref SerializationInfo info, ref StreamingContext context) {
			if(Main.saveVersionFromDisk >= 4) {
				foreach(Upgrade item in cashUpgradeList) {
					item.setIsPurchased(info.GetBoolean("upgrade_" + item.saveName));
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
