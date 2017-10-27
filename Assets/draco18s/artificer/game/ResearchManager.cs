using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.game {
	class ResearchManager {
		private static Transform relicList;
		private static Transform relicInfo;
		private static Text relicInfoText;
		private static Dictionary<ItemStack, GameObject> relicsList = new Dictionary<ItemStack, GameObject>();
		private static Material progressBarMat;
		private static Text timeLeftTxt;
		private static Text relicsLeftTxt;
		private static System.Random rand = new System.Random();
		private static ItemStack examinedStack = null;
		public static readonly int maxResearchTime = 7200;
		private static Text moneyDisp;

		public static void OneTimeSetup() {
			Transform trans = GuiManager.instance.researchArea.transform;
			relicList = trans.FindChild("RelicsList").GetChild(0).GetChild(0);
			relicList.transform.hierarchyCapacity = 200 * 20;
			progressBarMat = trans.FindChild("Research").FindChild("RelicProgress").GetComponent<Image>().material;
			trans.FindChild("Research").FindChild("Barbg").GetComponent<Button>().onClick.AddListener(delegate { IncrementResearch(); });
			timeLeftTxt = trans.FindChild("Research").FindChild("TimeLeft").GetComponent<Text>();
			relicsLeftTxt = trans.FindChild("Research").FindChild("NumUnidentified").GetComponent<Text>();

			relicInfo = trans.FindChild("RelicInfoOpen").GetChild(0);
			relicInfo.FindChild("CloseBtn").GetComponent<Button>().onClick.AddListener(delegate { CloseInfo(); });
			Button btn = relicInfo.FindChild("SellBtn").GetComponent<Button>();
			btn.onClick.AddListener(delegate { SellItem(); });
			btn.AddHover(delegate (Vector3 p) {
				if(examinedStack != null) {
					BigInteger val = BigRational.ToBigInt(GetRelicValue(examinedStack));
					GuiManager.ShowTooltip(btn.transform.position + Vector3.up * 30,"Sell for $" + Main.AsCurrency(val));
				}
			}, false);
			Button btn2 = relicInfo.FindChild("DiscardBtn").GetComponent<Button>();
			btn2.onClick.AddListener(delegate { DiscardItem(); });
			btn2.AddHover(delegate (Vector3 p) {
				if(examinedStack != null) {
					GuiManager.ShowTooltip(btn2.transform.position + Vector3.up * 30, "Discard this artifact back to the unidentified pile.", 3);
				}
			}, false);
			moneyDisp = GuiManager.instance.researchHeader.transform.FindChild("MoneyArea").GetChild(0).GetComponent<Text>();
			relicInfoText = relicInfo.FindChild("Info Scroll View").GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
			for(int r = 1; r <= 20; r++) {
				Image igm = relicInfo.transform.FindChild("Req" + r).GetComponent<Image>();
					igm.AddHover(delegate (Vector3 p) {
					GuiManager.ShowTooltip(igm.transform.position + Vector3.up * 20, displayReqDetails(r));
				},false);
			}
		}

		private static string displayReqDetails(int r) {
			if(examinedStack != null) {
				long ty = (long)examinedStack.getAllReqs();
				//ty &= 1 << r;
				//RequirementType rt = (RequirementType)ty;
				//return ty.ToString();//rt.ToString();
				long req_num = 1;
				do {
					while((ty & 1) == 0 && ty > 0) {
						req_num = req_num << 1;
						ty = ty >> 1;
					}
					r--;
				} while(r > 0);
				return ((RequirementType)req_num).ToString();
			}
			return "";
		}

		public static void setupUI() {
			int i = 0;
			int X = Mathf.FloorToInt((((RectTransform)relicList).rect.width - 10) / 98);
			Debug.Log("How many fit? " + X);
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				//Debug.Log("i: " + i);
				//Debug.Log(stack.item.name);
				if(stack.relicData != null) {
					//Debug.Log("Relic data");
					GameObject go;
					relicsList.TryGetValue(stack, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM_SELLABALE, relicList) as GameObject;
						//go.transform.SetParent(relicList);
						relicsList.Add(stack, go);
					}
					go.transform.localPosition = new Vector3((i % X) * 98 + 5, ((i / X) * -125) - 5, 0);
					//Debug.Log(go.transform.parent.name + ":" + go.transform.localPosition);
					Text tx = go.transform.FindChild("Title").GetComponent<Text>();
					tx.text = Main.ToTitleCase(stack.getDisplayName());
					go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
					//go.transform.FindChild("Quantity").GetComponent<Text>().text = "";
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
						GuiManager.ShowTooltip(btn.transform.position+Vector3.down*100, str);
					}, false);
					btn.onClick.AddListener(delegate { ShowInfo(s); });
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
			((RectTransform)relicList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((i / X) * 100 + 10));
			relicList.localPosition = Vector3.zero;
			relicsLeftTxt.text = Main.instance.player.unidentifiedRelics.Count + " unidentified";
		}

		public static void update(float dt) {
			if(Main.instance.player.unidentifiedRelics.Count > 0) {
				UpgradeValueWrapper wrap;
				Main.instance.player.upgrades.TryGetValue(UpgradeType.RESEARCH_RATE, out wrap);
				float multi = ((UpgradeFloatValue)wrap).value * Main.instance.player.currentGuildmaster.researchMultiplier() * (float)(1 + SkillList.ResearchRate.getMultiplier());
				Main.instance.player.researchTime += dt * multi;
				if(Main.instance.player.researchTime >= maxResearchTime) {
					Main.instance.player.researchTime -= maxResearchTime;
					ItemStack s = Main.instance.player.unidentifiedRelics[rand.Next(Main.instance.player.unidentifiedRelics.Count)];
					s.isIDedByPlayer = true;
					Main.instance.player.unidentifiedRelics.Remove(s);
					Main.instance.player.addItemToInventory(s);
					relicsLeftTxt.text = Main.instance.player.unidentifiedRelics.Count + " unidentified";
					setupUI();
				}
				timeLeftTxt.text = Main.SecondsToTime((maxResearchTime - Main.instance.player.researchTime) / ((UpgradeFloatValue)wrap).value * multi);
				progressBarMat.SetFloat("_Cutoff", 1-Main.instance.player.researchTime / maxResearchTime);
			}
			else {
				timeLeftTxt.text = "∞";
				progressBarMat.SetFloat("_Cutoff", 1);
			}
			moneyDisp.text = "$" + Main.AsCurrency(Main.instance.player.money);
		}

		public static void IncrementResearch() {
			StatisticsTracker.numClicks.addValue(1);
			UpgradeValueWrapper wrap1;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.RESEARCH_RATE, out wrap1);
			Main.instance.player.researchTime += 50 * ((UpgradeFloatValue)wrap1).value * Main.instance.GetClickRate() * Main.instance.player.currentGuildmaster.researchMultiplier();
		}

		private static void ShowInfo(ItemStack stack) {
			relicInfo.parent.gameObject.SetActive(true);
			examinedStack = stack;
			relicInfo.FindChild("Title").GetComponent<Text>().text = examinedStack.getDisplayName();
			relicInfo.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + examinedStack.item.name);
			relicInfoText.text = "";
			int req_num = 1;
			long ty = (long)stack.getAllReqs();
			bool abort = false;
			for(int r = 1; r <= 20; r++) {
				if(ty == 0) abort = true;
				while((ty & 1) == 0 && ty > 0) {
					req_num++;
					ty = ty >> 1;
					if(ty == 0) abort = true;
				}
				if(abort) {
					relicInfo.transform.FindChild("Req" + r).gameObject.SetActive(false);
				}
				else {
					relicInfo.transform.FindChild("Req" + r).gameObject.SetActive(true);
					relicInfo.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
					ty = ty >> 1;
					ty = ty << 1;
				}
			}
			List<string> strList = new List<string>();

			if(stack.enchants.Count > 0) {
				strList.Add("Enchanted:");
				foreach(Enchantment en in stack.enchants) {
					strList.Add("   " + en.name);
				}
			}

			foreach(RelicInfo inf in stack.relicData) {
				strList.Add(inf.heroName + " (" + inf.notoriety + ")\n   " + inf.questDescription);
			}
			relicInfoText.text = string.Join("\n", strList.ToArray());
			((RectTransform)relicInfoText.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, relicInfoText.preferredHeight + 1);
			((RectTransform)relicInfoText.transform.parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, relicInfoText.preferredHeight + 5);
		}

		private static void CloseInfo() {
			examinedStack = null;
			relicInfo.parent.gameObject.SetActive(false);
		}

		private static void SellItem() {
			Main.instance.player.miscInventory.Remove(examinedStack);
			GameObject go;
			relicsList.TryGetValue(examinedStack, out go);
			Main.Destroy(go);
			BigRational val = GetRelicValue(examinedStack);
			Main.instance.player.AddMoney(BigRational.ToBigInt(val));
			examinedStack.onSoldByPlayer();
			examinedStack = null;
			relicInfo.parent.gameObject.SetActive(false);
			setupUI();
		}

		private static BigRational GetRelicValue(ItemStack examinedStack) {
			if(examinedStack.item.industry == null) {
				return 1000;
			}
			BigRational val = examinedStack.item.industry.GetSellValue() * 10000 * BigRational.Pow(1.1f, examinedStack.enchants.Count);
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.QUEST_SCALAR, out wrap);
			RelicInfo ri = examinedStack.relicData.OrderByDescending(o => o.notoriety).First();
			if(ri != null) {
				BigRational b = BigRational.Pow(1.5f, ri.notoriety) * 1000;
				b *= ((UpgradeFloatValue)wrap).value;
				val += b;
				if(ri.relicName.Equals("Lost")) {
					val /= 100000;
				}
			}
			val *= Main.instance.GetRelicSellMultiplier();
			val = BigRational.Truncate(val, 6, false);
			return val;
		}

		private static void DiscardItem() {
			Main.instance.player.miscInventory.Remove(examinedStack);
			Main.instance.player.unidentifiedRelics.Add(examinedStack);
			GameObject go;
			relicsList.TryGetValue(examinedStack, out go);
			examinedStack = null;
			relicInfo.parent.gameObject.SetActive(false);
			setupUI();
		}
	}
}
