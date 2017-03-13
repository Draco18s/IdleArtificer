using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.game {
	class ResearchManager {
		private static Transform relicList;
		private static Dictionary<ItemStack, GameObject> relicsList = new Dictionary<ItemStack, GameObject>();
		private static Material progressBarMat;
		private static Text timeLeftTxt;
		private static System.Random rand = new System.Random();

		public static void OneTimeSetup() {
			Transform trans = GuiManager.instance.researchArea.transform;
			relicList = trans.FindChild("RelicsList").GetChild(0).GetChild(0);
			relicList.transform.hierarchyCapacity = 200 * 20;
			progressBarMat = trans.FindChild("Research").FindChild("RelicProgress").GetComponent<Image>().material;
			trans.FindChild("Research").FindChild("Barbg").GetComponent<Button>().onClick.AddListener(delegate { IncrementResearch(); });
			timeLeftTxt = trans.FindChild("Research").FindChild("TimeLeft").GetComponent<Text>();
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
			((RectTransform)relicList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((i / 4) * 100 + 10));
			relicList.localPosition = Vector3.zero;
		}

		public static void update(float dt) {
			if(Main.instance.player.unidentifiedRelics.Count > 0) {
				Debug.Log("Time " + Main.instance.player.researchTime);
				UpgradeValueWrapper wrap;
				Main.instance.player.upgrades.TryGetValue(UpgradeType.RESEARCH_RATE, out wrap);
				Main.instance.player.researchTime += dt * ((UpgradeFloatValue)wrap).value;
				if(Main.instance.player.researchTime >= 3600) {
					Main.instance.player.researchTime -= 3600;
					ItemStack s = Main.instance.player.unidentifiedRelics[rand.Next(Main.instance.player.unidentifiedRelics.Count)];
					s.isIDedByPlayer = true;
					Main.instance.player.unidentifiedRelics.Remove(s);
					Main.instance.player.addItemToInventory(s);
				}
				timeLeftTxt.text = Main.SecondsToTime(3600 - Main.instance.player.researchTime);
			}
			timeLeftTxt.text = "∞";
		}

		public static void IncrementResearch() {
			UpgradeValueWrapper wrap1;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.RESEARCH_RATE, out wrap1);
			UpgradeValueWrapper wrap2;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.CLICK_RATE, out wrap2);
			Main.instance.player.researchTime += 200 * ((UpgradeFloatValue)wrap1).value * ((UpgradeFloatValue)wrap2).value;
		}
	}
}
