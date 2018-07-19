using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.quests.challenge.goals.DeepGoals;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.config;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.game {
	public class QuestManager {
		IPointerDownHandler l;
		public static List<Quest> availableQuests = new List<Quest>();
		public static List<Quest> activeQuests = new List<Quest>();
		//public static List<ItemStack> allRelics = new List<ItemStack>();
		public static List<ItemStack> availableRelics = new List<ItemStack>();
		private static float questEquipTimer = 0;
		private static float questEquipTimerMax = 7200; //120 minutes
		private static float newQuestDelayTimer = 0;
		private static float newQuestMaxTime = 1200; //20 minutes
		private static Transform activeQuestList;
		private static Transform regularQuests;
		private static Transform deepGoal;
		private static Transform questList;
		private static Transform inventoryList;
		private static Transform miscInventoryList;
		private static Transform filters;
		private static Industry selectedIndustry;
		private static ItemStack selectedStack;

		private static Dictionary<Industry, GameObject> questInvenList = new Dictionary<Industry, GameObject>();
		private static Dictionary<ItemStack, GameObject> miscInvenList = new Dictionary<ItemStack, GameObject>();

		public static void setupUI() {
			if(questList == null) {
				regularQuests = GuiManager.instance.questArea.transform.Find("Quests");
				questList = regularQuests.Find("Available").GetChild(0).GetChild(0);
				activeQuestList = regularQuests.Find("Active").GetChild(0).GetChild(0);
				inventoryList = GuiManager.instance.questArea.transform.Find("Inventory1").GetChild(0).GetChild(0);
				miscInventoryList = GuiManager.instance.questArea.transform.Find("Inventory2").GetChild(0).GetChild(0);
				deepGoal = GuiManager.instance.questArea.transform.Find("DeepQuest");
				filters = GuiManager.instance.questArea.transform.Find("Filters");
				GameObject img = GuiManager.instance.questArea.transform.Find("Filters").GetChild(0).gameObject;
				foreach(RequirementType t in Enum.GetValues(typeof(RequirementType))) {
					int v = Main.BitNum((long)t);
					GameObject go = Main.Instantiate(img, img.transform.position + new Vector3(v % 13,v / -13,0)*18, Quaternion.identity, filters) as GameObject;
					go.GetComponent<Image>().sprite = GuiManager.instance.req_icons[v-1];
					go.name = "Req" + v;
					go.GetComponent<Button>().onClick.AddListener(delegate { FilterInventory(v-1); });
				}
				img.GetComponent<Button>().onClick.AddListener(delegate { FilterInventory(-1); });
				GuiManager.instance.questHeader.transform.Find("FilterBtn").GetComponent<Button>().onClick.AddListener(delegate { showHideFilters(); });
			}
			int i = 0;
			validateQuests();
			((RectTransform)questList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 150 + 10);
			questList.transform.localPosition = Vector3.zero;
			inventoryList.hierarchyCapacity = 100 * 30;
			miscInventoryList.hierarchyCapacity = 100 * 30;
			questList.hierarchyCapacity = 50 * 30 + 1500;
			activeQuestList.hierarchyCapacity = 50 * 30 + 1500;
			activeQuestList.transform.localPosition = new Vector3(activeQuestList.transform.localPosition.x, 0, activeQuestList.transform.localPosition.z);
			i = 0;
			if(Main.instance.player.builtItems.Count < questInvenList.Count) {
				foreach(GameObject go in questInvenList.Values) {
					Main.Destroy(go);
				}
				questInvenList.Clear();
			}
			foreach(Industry ind in Main.instance.player.builtItems) {
				if(ind.industryItem.canBeGivenToQuests) {
					GameObject go;
					questInvenList.TryGetValue(ind, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM, inventoryList) as GameObject;
						//go.transform.SetParent(inventoryList);
						Industry newInd = ind;
						questInvenList.Add(newInd, go);
						//ind.questInvenListObj = go;
						go.name = ind.saveName;
						Text tx = go.transform.Find("Title").GetComponent<Text>();
						tx.text = Main.ToTitleCase(Localization.translateToLocal(ind.unlocalizedName));
						tx.fontSize = 28;
						go.transform.Find("Quantity").GetComponent<Text>().text = "0 / " + Main.AsCurrency(ind.quantityStored);
						go.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + ind.saveName);
						int req_num = 1;
						long ty = (long)ind.industryItem.getAllReqs();
						bool abort = false;
						for(int r = 1; r <= 5; r++) {
							if(ty == 0) abort = true;
							while((ty & 1) == 0 && ty > 0) {
								req_num++;
								ty = ty >> 1;
								if(ty == 0) abort = true;
							}
							if(abort) {
								go.transform.Find("Req" + r).gameObject.SetActive(false);
							}
							else {
								go.transform.Find("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
								ty = ty >> 1;
								ty = ty << 1;
							}
						}
						go.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(newInd); });
					}
					go.transform.localPosition = new Vector3(7, (i * -125) - 5, 0);
					i++;
				}
			}
			((RectTransform)inventoryList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);
			inventoryList.transform.localPosition = Vector3.zero;
			i = 0;
			//Debug.Log("Misc Inventory size: " + Main.instance.player.miscInventory.Count);
			refreshMiscInventory();
			selectedIndustry = null;
			selectedStack = null;
			IDeepGoal goal = Main.instance.player.getActiveDeepGoal();
			//Transform filters = GuiManager.instance.questArea.transform.FindChild("Filters");
			if(goal == DeepGoalsTypes.NONE) {
				deepGoal.gameObject.SetActive(false);
				((RectTransform)regularQuests).offsetMax = new Vector2(((RectTransform)regularQuests).offsetMax.x, 0);
				//filters.position = new Vector3(0,0,0);
			}
			else {
				if(goal.isActive()) {
					deepGoal.gameObject.SetActive(true);
					deepGoal.GetChild(0).GetComponent<Text>().text = goal.name;
					deepGoal.GetChild(1).GetComponent<Text>().text = goal.description;
					((RectTransform)regularQuests).offsetMax = new Vector2(((RectTransform)regularQuests).offsetMax.x, -100);
				}
				else {
					deepGoal.gameObject.SetActive(true);
					deepGoal.GetChild(0).GetComponent<Text>().text = goal.name;
					deepGoal.GetChild(1).GetComponent<Text>().text = goal.completeDescription;
					((RectTransform)regularQuests).offsetMax = new Vector2(((RectTransform)regularQuests).offsetMax.x, -100);
				}
			}
			FilterInventory(-1);
			filters.gameObject.SetActive(false);
		}

		private static void showHideFilters() {
			filters.gameObject.SetActive(!filters.gameObject.activeSelf);
		}

		private static int lastFilter = -1;

		private static void FilterInventory(int v) {
			if(lastFilter == v) v = -1;
			lastFilter = v;
			int i = 0;
			foreach(Industry ind in Main.instance.player.builtItems) {
				GameObject go;
				questInvenList.TryGetValue(ind, out go);
				if(go == null) continue;
				if(v < 0 || ind.hasReqType((RequirementType)(1L<<v))) {
					go.SetActive(true);
					go.transform.localPosition = new Vector3(7, (i * -125) - 5, 0);
					i++;
				}
				else {
					go.SetActive(false);
				}
			}
			i = 0;
			foreach(ItemStack ind in Main.instance.player.miscInventory) {
				GameObject go;
				miscInvenList.TryGetValue(ind, out go);
				if(go == null) continue;
				if(v < 0 || ind.doesStackHave((RequirementType)(1L << v))) {
					go.SetActive(true);
					go.transform.localPosition = new Vector3(7, (i * -125) - 5, 0);
					i++;
				}
				else {
					go.SetActive(false);
				}
			}
		}

		private static void CreateNewQuestGuiItem(Quest q, int i) {
			GameObject go = Main.Instantiate(PrefabManager.instance.QUEST_GUI_LISTITEM, questList) as GameObject;
			Quest theQuest = q;
			q.guiItem = go;
			//go.transform.SetParent(questList);
			go.transform.localPosition = new Vector3(7, (i * -150) - 7, 0);
			if(q.obstacles[q.obstacles.Length - 1].type == null) {
				throw new Exception("!!!");
				/*Debug.Log(q.getOriginalGoal());
				Main.Destroy(go);
				q.questComplete = true;
				q.timeUntilQuestExpires = -100;*/
				return;
			}
			go.transform.Find("Name").GetComponent<Text>().text = ToTitleCase(q.obstacles[q.obstacles.Length - 1].type.name);
			go.transform.Find("Hero").GetComponent<Text>().text = "Hero: " + q.heroName;
			for(int r = 1; r <= 6; r++) {
				int req_num = Main.BitNum(q.getReq(r - 1)) - 1;
				if(req_num < 0) {
					go.transform.Find("Req" + r).gameObject.SetActive(false);
				}
				else {
					Transform rq = go.transform.Find("Req" + r);
					rq.GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num];
					int v = req_num;
					Button bb = rq.GetComponent<Button>();
					bb.onClick.AddListener(delegate { FilterInventory(v); });
					bb.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(bb.transform.parent.Find("ReqHover").position + Vector3.right * 60, "Items that supply these traits will surely aid the hero.\nClick to filter.", 5); }, false);
				}
			}
			Button b1 = go.transform.Find("Start").GetComponent<Button>();
			b1.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b1.transform.position + Vector3.right*45, "Start the quest now with the the inventory shown above.\nYou do not have to supply any items, but the quest will likely fail.\nSuccessful quests will generate additional rewards.", 6.5F); }, false);
			b1.onClick.AddListener(delegate { startQuest(theQuest, go); });
			Button b2 = go.transform.Find("Cancel").GetComponent<Button>();
			b2.onClick.AddListener(delegate {
				Main.instance.player.getActiveDeepGoal().onFailedQuest(q);
				removeQuest(theQuest, go);
				newQuestDelayTimer -= 300;
			});
			b2.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b2.transform.position + Vector3.right * 45, "Ignore the quest. Reduces the time until the next quest by " + Main.SecondsToTime(300) + ".", 3.5f); }, false);
			for(int r = 1; r <= 6; r++) {
				Transform btn = go.transform.Find("Inven" + r);
				int slotnum = r - 1;
				btn.GetComponent<Button>().onClick.AddListener(delegate { AddRemoveItemFromQuest(theQuest, slotnum); });
				if(q.inventory.Count >= r) {
					if(q.inventory[r - 1].item.industry != null) {
						if(theQuest.inventory[r - 1].item.industry.craftingGridGO == null) {
							foreach(GameObject cgo in CraftingManager.buildButtons) {
								if(cgo.GetComponent<ItemButtonData>().connectedItem == theQuest.inventory[r - 1].item.industry) {
									q.guiItem.transform.Find("Inven" + r).GetComponent<Image>().sprite = cgo.transform.Find("Img").GetComponent<Image>().sprite;
									break;
								}
							}
						}
						else {
							q.guiItem.transform.Find("Inven" + r).GetComponent<Image>().sprite = theQuest.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).Find("Img").GetComponent<Image>().sprite;
						}
					}
					else {
						q.guiItem.transform.Find("Inven" + r).GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + theQuest.inventory[r - 1].item.name);
					}
				}
			}
			go.transform.Find("Reward1").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.rewards[0].item.name);
			go.transform.Find("Reward2").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.rewards[1].item.name);
			go.transform.Find("Expire").GetComponent<Text>().text = "Expires in " + Main.SecondsToTime((int)q.timeUntilQuestExpires);
			Button b3 = go.transform.Find("RewardLabel").GetChild(0).GetComponent<Button>();
			b3.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b3.transform.position + Vector3.right * 92, "You will get these items when the quest is started.", 3); }, false);
			Button b4 = go.transform.Find("ReqHover").GetComponent<Button>();
			b4.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b4.transform.position + Vector3.right * 60, "Items that supply these traits will surely aid the hero.\nClick to filter.", 5); }, false);
		}

		private static void CreateNewActiveQuestGuiItem(Quest q) {
			GameObject go = Main.Instantiate(PrefabManager.instance.ACTIVE_QUEST_GUI_LISTITEM, activeQuestList) as GameObject;
			//Quest theQuest = q;
			q.guiItem = go;
			//go.transform.SetParent(activeQuestList);

			//go.transform.FindChild("Name").GetComponent<Text>().text = ToTitleCase(q.obstacles[q.obstacles.Length - 1].type.name);
			go.transform.Find("Hero").GetComponent<Text>().text = q.heroName;
			QuestInfo info = go.GetComponent<QuestInfo>();
			Image img = info.HPBar.GetComponent<Image>();
			img.material = Main.Instantiate(img.material);
			img.material.SetColor("_Color", Color.red);
			img.material.SetFloat("_Cutoff", 0);
			img = info.MPBar.GetComponent<Image>();
			img.material = Main.Instantiate(img.material);
			img.material.SetColor("_Color", Color.blue);
			img.material.SetFloat("_Cutoff", 0);
			img = info.ProgBar.GetComponent<Image>();
			img.material = Main.Instantiate(img.material);
			img.material.SetColor("_Color", Color.green);
			img.material.SetFloat("_Cutoff", 1);
		}

		private static string ToTitleCase(string stringToConvert) {
			string output = "";
			foreach(char c in stringToConvert) {
				if(char.IsUpper(c)) {
					output += " ";
				}
				output += c;
			}
			return output.Substring(6);
		}

		private static void SelectItem(ItemStack stack) {
			selectedStack = stack;
			selectedIndustry = null;
		}

		private static void SelectItem(Industry ind) {
			selectedIndustry = ind;
			selectedStack = null;
		}

		private static void AddRemoveItemFromQuest(Quest theQuest, int slot) {
			if(selectedIndustry == null && selectedStack == null) {
				if(slot < theQuest.inventory.Count) {
					if(theQuest.inventory[slot].wasAddedByJourneyman && (theQuest.inventory[slot].relicData != null || theQuest.inventory[slot].enchants.Count > 0)) {
						Main.instance.player.addItemToInventory(theQuest.inventory[slot]);
					}
					theQuest.inventory.Remove(theQuest.inventory[slot]);
				}
			}
			else if(slot < theQuest.inventory.Count && ((selectedIndustry != null && theQuest.inventory[slot].item == selectedIndustry.industryItem) || (selectedStack != null && theQuest.inventory[slot].item == selectedStack.item))) {
				theQuest.inventory.Remove(theQuest.inventory[slot]);
			}
			else {
				if(selectedIndustry != null) {
					ItemStack stack = new ItemStack(selectedIndustry, Mathf.RoundToInt(selectedIndustry.getStackSizeForQuest() * Main.instance.GetQuestStackMultiplier(selectedIndustry, theQuest.numQuestsBefore) * theQuest.getGoal().getReqScalar() / (selectedIndustry.industryItem.getEffectiveness()>1? selectedIndustry.industryItem.getEffectiveness():1)));
					if(slot < theQuest.inventory.Count) {
						theQuest.inventory[slot] = stack;
					}
					else {
						theQuest.inventory.Add(stack);
					}
				}
				else if(selectedStack != null) {
					if(slot < theQuest.inventory.Count) {
						theQuest.inventory[slot] = selectedStack;
					}
					else if(!theQuest.inventory.Contains(selectedStack)) {
						theQuest.inventory.Add(selectedStack);
					}
					foreach(Quest q in availableQuests) {
						if(q != theQuest) {
							if(q.inventory.Contains(selectedStack)) {
								q.inventory.Remove(selectedStack);
								for(int r = 1; r <= 6; r++) {
									Image im = q.guiItem.transform.Find("Inven" + r).GetComponent<Image>();
									im.sprite = GuiManager.instance.gray_square;
									if(q.inventory.Count >= r) {
										if(q.inventory[r - 1].item.industry != null) {
											im.GetComponent<Image>().sprite = q.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).Find("Img").GetComponent<Image>().sprite;
										}
										else {
											im.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.inventory[r - 1].item.name);
										}
									}
								}
							}
						}
					}
				}
			}
			for(int r = 1; r <= 6; r++) {
				Image im = theQuest.guiItem.transform.Find("Inven" + r).GetComponent<Image>();
				im.sprite = GuiManager.instance.gray_square;
				if(theQuest.inventory.Count >= r) {
					if(theQuest.inventory[r - 1].item.industry != null) {
						if(theQuest.inventory[r - 1].item.industry.craftingGridGO == null) {
							foreach(GameObject go in CraftingManager.buildButtons) {
								if(go.GetComponent<ItemButtonData>().connectedItem == theQuest.inventory[r - 1].item.industry) {
									im.GetComponent<Image>().sprite = go.transform.Find("Img").GetComponent<Image>().sprite;
									break;
								}
							}
						}
						else {
							im.GetComponent<Image>().sprite = theQuest.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).Find("Img").GetComponent<Image>().sprite;
						}
					}
					else {
						im.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + theQuest.inventory[r - 1].item.name);
					}
				}
			}
			updateLists();
		}

		private static void refreshMiscInventory() {
			int i;
			for(i = 0; i < miscInventoryList.childCount; i++) {
				Main.Destroy(miscInventoryList.GetChild(i).gameObject);
			}
			miscInvenList.Clear();
			i = 0;
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				if(stack.item.canBeGivenToQuests && (stack.relicData == null || stack.isIDedByPlayer)) {
					ItemStack s = stack;
					GameObject go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM, miscInventoryList) as GameObject;
					miscInvenList.Add(stack, go);
					//go.transform.SetParent(miscInventoryList);
					go.transform.localPosition = new Vector3(7, (i * -125) - 5, 0);
					//ind.invenListObj = go;
					go.name = stack.item.name;
					Text tx = go.transform.Find("Title").GetComponent<Text>();
					tx.text = Main.ToTitleCase(stack.getDisplayName());
					if(stack.relicData == null)
						tx.fontSize = 28;
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize);
					go.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
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
							go.transform.Find("Req" + r).gameObject.SetActive(false);
						}
						else {
							go.transform.Find("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
							ty = ty >> 1;
							ty = ty << 1;
						}
					}
					go.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(s); });
					i++;
				}
			}
			((RectTransform)miscInventoryList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);
			miscInventoryList.transform.localPosition = Vector3.zero;
		}

		public static float getEquipTimer() {
			return questEquipTimer;
		}

		public static void setEquipTimer(float v) {
			questEquipTimer = v;
		}

		private static void validateQuests() {
			int i = 0;
			foreach(Industry ind in Main.instance.player.builtItems) {
				GameObject go;// = ind.questInvenListObj;
				questInvenList.TryGetValue(ind, out go);
				if(go != null && go.activeSelf) {
					//go.transform.localPosition = new Vector3(7, (i * -125) - 7, 0);
					//go.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(Localization.translateToLocal(ind.unlocalizedName));
					//go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + ind.saveName);
					int total = 0;
					foreach(Quest q in availableQuests) {
						foreach(ItemStack stack in q.inventory) {
							if(stack.item == ind.industryItem && stack.enchants.Count == 0 && stack.relicData == null) {
								total += stack.stackSize;
							}
						}
					}
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored, 4, true) + " / " + Main.AsCurrency(total, 4, true);
					//bool haveEnough = ind.quantityStored >= total;
					
					i++;
				}
			}
			((RectTransform)inventoryList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);


			foreach(Quest q in availableQuests) {
				
				q.guiItem.transform.Find("Start").GetComponent<Button>().interactable = isQuestReady(q);
			}
		}

		private static bool isQuestReady(Quest q) {
			bool questReady = true;
			Hashtable data = new Hashtable();
			foreach(ItemStack stack in q.inventory) {
				if(stack.enchants.Count == 0 && stack.relicData == null && stack.item.industry != null) {
					if(data.Contains(stack.item.industry)) {
						data[stack.item.industry] = (int)data[stack.item.industry] + stack.stackSize;
					}
					else {
						data[stack.item.industry] = stack.stackSize;
					}
				}
			}
			foreach(Industry ind in data.Keys) {
				questReady &= (ind.quantityStored >= (int)data[ind]);
			}
			return questReady;
		}

		public static bool IsIndustryOnQuest(Industry item) {
			foreach(Quest q in availableQuests) {
				foreach(ItemStack stack in q.inventory) {
					if(stack.item == item.industryItem) {
						return true;
					}
				}
			}
			return false;
		}

		private static void removeQuest(Quest theQuest, GameObject go) {
			Button b = theQuest.guiItem.transform.Find("Start").GetComponent<Button>();
			b.RemoveAllEvents();
			b = theQuest.guiItem.transform.Find("Cancel").GetComponent<Button>();
			b.RemoveAllEvents();
			b = go.transform.Find("RewardLabel").GetChild(0).GetComponent<Button>();
			b.RemoveAllEvents();
			b = go.transform.Find("ReqHover").GetComponent<Button>();
			b.RemoveAllEvents();

			availableQuests.Remove(theQuest);
			Main.Destroy(go);
			int i = 0;
			foreach(Quest q in availableQuests) {
				q.guiItem.transform.localPosition = new Vector3(7, (i * -150) - 7, 0);
				i++;
			}
			((RectTransform)questList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 150 + 10);
			if((i * 150 + 10 - ((RectTransform)questList.parent).rect.height) < questList.transform.localPosition.y) {
				questList.transform.localPosition = new Vector3(0, Math.Max((i * 150 + 10) - ((RectTransform)questList.parent).rect.height,0), 0);
			}
			//questList.transform.localPosition = Vector3.zero;
			selectedIndustry = null;
		}

		private static void startQuest(Quest theQuest, GameObject go) {
			removeQuest(theQuest, go);
			activeQuests.Add(theQuest);

			foreach(ItemStack stack in theQuest.inventory) {
				UpgradeValueWrapper wrap;
				Main.instance.player.upgrades.TryGetValue(UpgradeType.QUEST_GOODS_VALUE, out wrap);
				if(stack.item.industry != null && stack.relicData == null && stack.enchants.Count == 0) {
					stack.item.industry.quantityStored -= stack.stackSize;

					Main.instance.player.money += BigRational.ToBigInt(new BigRational(stack.stackSize) * ((UpgradeFloatValue)wrap).value * Main.instance.player.currentGuildmaster.questValueMultiplier(stack.item.industry.industryType) * stack.item.getBaseValue());

					//Debug.Log(stack.GetHashCode());
					stack.setToMaxSize();
				}
				else {
					Main.instance.player.miscInventory.Remove(stack);
					if(stack.enchants.Count > 0 || stack.relicData != null) {
						ItemStack toPlayer = stack.split(stack.stackSize - 1);
						Main.instance.player.addItemToInventory(toPlayer);
					}
					else {
						ItemStack toPlayer = stack.split(stack.stackSize - 1);
						Main.instance.player.addItemToInventory(toPlayer);
					}
					//BigRational val = stack.item.getBaseValue() * BigRational.Pow(1.1f, stack.enchants.Count + stack.antiquity);
					//Main.instance.player.money += BigRational.ToBigInt(val * 0.001f * ((UpgradeFloatValue)wrap).value);
				}
			}
			theQuest.questStarted();
			Main.instance.player.addItemToInventory(theQuest.rewards[0]);
			Main.instance.player.addItemToInventory(theQuest.rewards[1]);
			selectedIndustry = null;
			selectedStack = null;
			//setupUI();
			refreshMiscInventory();
			FilterInventory(lastFilter);
			validateQuests();
			CreateNewActiveQuestGuiItem(theQuest);
			updateActiveQuestList();
			Main.instance.player.getActiveDeepGoal().finalizeQuest(ref theQuest);
		}

		public static void updateLists() {
			int i = 0;
			foreach(Quest q in availableQuests) {
				if(q.guiItem == null) {
					CreateNewQuestGuiItem(q, i);
				}
				q.guiItem.transform.localPosition = new Vector3(7, (i * -150) - 7, 0);
				i++;
			}
			((RectTransform)questList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 150 + 14);
			//questList.transform.localPosition = Vector3.zero;
			//i = 0;
			validateQuests();
			//inventoryList.transform.localPosition = Vector3.zero;

			GuiManager.instance.questHeader.transform.Find("MoneyArea").GetChild(0).GetComponent<Text>().text = "$" + Main.AsCurrency(Main.instance.player.money, 12);
			GuiManager.instance.questHeader.transform.Find("QuestArea").GetChild(0).GetComponent<Text>().text = Main.AsCurrency(StatisticsTracker.questsCompleted.serializedValue, 12);
		}

		protected static void updateActiveQuestList() {
			int i = 0;
			foreach(Quest q in activeQuests) {
				if(q.guiItem == null) {
					CreateNewActiveQuestGuiItem(q);
				}
				GameObject go = q.guiItem;
				if(q.isActive()) {
					go.transform.localPosition = new Vector3(7, (i * -68) - 7, 0);
					QuestInfo info = go.GetComponent<QuestInfo>();
					//Debug.Log(info);
					info.status.text = "is " + q.getStatus();
					for(int r = 1; r <= 4; r++) {
						int req_num = Main.BitNum(q.getCurrentReq(r - 1)) - 1;
						if(req_num < 0) {
							info.getReq(r).SetActive(false);
							//go.transform.FindChild("Req" + r).gameObject.SetActive(false);
						}
						else {
							GameObject o = info.getReq(r);
							o.SetActive(true);
							o.GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num];
							//go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num];
						}
					}
					info.gameObject.SetActive(false);
					float per = ((float)q.heroCurHealth) / q.heroMaxHealth;
					float ext = 0;
					Image img = info.HPBar.GetComponent<Image>();
					ext = img.material.GetFloat("_Cutoff");
					ext = MathHelper.EaseOutQuadratic(0.1f, ext, 1 - per, 1);
					img.material.SetFloat("_Cutoff", ext);

					per = q.QuestTimeLeft() / Quest.GetQuestMaxTime();
					img = info.MPBar.GetComponent<Image>();
					ext = img.material.GetFloat("_Cutoff");
					ext = MathHelper.EaseOutQuadratic(0.1f, ext, 1 - per, 1);
					img.material.SetFloat("_Cutoff", ext);

					per = q.GetCompletion();
					//Debug.Log((per*100) + "%");
					img = info.ProgBar.GetComponent<Image>();
					ext = img.material.GetFloat("_Cutoff");
					ext = MathHelper.EaseOutQuadratic(0.1f, ext, 1 - per, 1);
					img.material.SetFloat("_Cutoff", ext);
					info.gameObject.SetActive(true);

					i++;
				}
				else {
					Main.Destroy(go);
				}
			}
			((RectTransform)activeQuestList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 68 + 10);
		}

		public static void tickAllQuests(float time) {
			newQuestDelayTimer -= time;
			foreach(Quest q in activeQuests) {
				EnumResult res = q.doQuestStep(time);
				if(res != EnumResult.CONTINUE) {
					q.finalResult = res;
					q.questComplete = true;
					if(res == EnumResult.SUCCESS) {
						ItemStack newRelic = q.determineRelic();
						if(newRelic != null) {
							newRelic = makeRelic(newRelic, q);
							//allRelics.Add(newRelic);
							availableRelics.Add(newRelic);
							StatisticsTracker.relicsMade.addValue(1);
							if(StatisticsTracker.relicsMade.value == 1) {
								StatisticsTracker.maxQuestDifficulty.addValue(1);
							}
						}
						foreach(ItemStack st in q.inventory) {
							if(st.relicData != null || st.enchants.Count > 0 && st != newRelic) {
								//if the player gave it to the hero, then it's ID'd and can go back to the player's general inventory
								//if it isn't, then it goes to the Unidentified Relics list
								//Debug.Log("ID? " + st.isIDedByPlayer);
								if(st.isIDedByPlayer) {
									Main.instance.player.addItemToInventory(st);
								}
								else {
									if(!Main.instance.player.unidentifiedRelics.Contains(st)) {
										if(!StatisticsTracker.unlockedResearch.isAchieved()) {
											StatisticsTracker.unlockedResearch.setAchieved();
											GuiManager.instance.researchTab.GetComponent<Button>().interactable = true;
										}
										Main.instance.player.unidentifiedRelics.Add(st);
									}
								}
							}
						}
						Main.instance.CompleteQuest(q.getGoal());
					}
					if(res == EnumResult.FAIL) {
						foreach(ItemStack st in q.inventory) {
							if(st.relicData != null) {
								st.isIDedByPlayer = false;
								availableRelics.Add(st);
							}
						}
					}
				}
			}
			activeQuests.Where(x => !x.isActive()).ToList().ForEach(x => des(x));
			activeQuests.RemoveAll(x => !x.isActive());
			while(newQuestDelayTimer <= 0 && availableQuests.Count < 10) {
				newQuestDelayTimer += getNewQuestMaxTime();
				Quest q = Quest.GenerateNewQuest();
				//q.timeUntilQuestExpires = 18000; //5 hours
				availableQuests.Add(q);
			}
			if(newQuestDelayTimer <= 0) {
				newQuestDelayTimer = -1;
				GuiManager.instance.questHeader.transform.Find("NewQuestTime").GetComponent<Text>().text = "---";
			}
			else {
				GuiManager.instance.questHeader.transform.Find("NewQuestTime").GetComponent<Text>().text = "New Quest in " + Main.SecondsToTime((int)newQuestDelayTimer);
			}
			foreach(Quest q in availableQuests) {
				q.timeUntilQuestExpires -= time;
				if(q.guiItem != null)
					q.guiItem.transform.Find("Expire").GetComponent<Text>().text = "Expires in " + Main.SecondsToTime((int)q.timeUntilQuestExpires);
				if(q.timeUntilQuestExpires <= 0) {
					Main.instance.player.getActiveDeepGoal().onFailedQuest(q);
					Main.Destroy(q.guiItem);
				}
			}

			List<Quest> questsToStart = new List<Quest>();
			for(int j = 0; j < Main.instance.player.journeymen; j++) {
				if(j < availableQuests.Count) {
					Quest jq = availableQuests[j];
					if(jq.timeUntilQuestExpires < 30 && jq.inventory.Count > 0) {
						if(isQuestReady(jq)) {
							questsToStart.Add(jq);
						}
					}
				}
			}
			foreach(Quest q in questsToStart) {
				startQuest(q, q.guiItem);
			}

			availableQuests.RemoveAll(x => x.timeUntilQuestExpires <= 0);
			
			updateActiveQuestList();
			questEquipTimer += (time * Main.instance.player.currentGuildmaster.journeymenRateMultiplier() * (1 + 0.05f*((AchievementMulti)StatisticsTracker.clicksAch).getNumAchieved()));
			//journeyman equip loop
			float tim = (questEquipTimerMax - questEquipTimer) / Main.instance.player.currentGuildmaster.journeymenRateMultiplier();
			Transform timer = GuiManager.instance.questHeader.transform.Find("JourneymanTime");
			timer.GetComponent<Text>().text = "Auto-Equip in " + Main.SecondsToTime((int)tim);
			timer.gameObject.SetActive(Main.instance.player.journeymen > 0);
			int adjustQuestTime = 0;
			if(questEquipTimer >= questEquipTimerMax) {
				questEquipTimer -= questEquipTimerMax;
				int questIndex = 0;
				for(int j = 0; j < Main.instance.player.journeymen*2; j++) {
					bool didAnything = false;
					if(questIndex < availableQuests.Count) {
						Quest jq = availableQuests[questIndex];
						for(int r = 1; r <= 6; r++) {
							RequirementType req = (RequirementType)jq.getReq(r - 1);
							if(req != 0 && !jq.doesHeroHave(req, false)) {
								foreach(ItemStack stack in Main.instance.player.miscInventory) {
									if((stack.relicData != null || stack.enchants.Count > 0) && stack.doesStackHave(req)) {
										Main.instance.player.miscInventory.Remove(stack);
										ItemStack toPlayer = stack.split(stack.stackSize - 1);
										Main.instance.player.addItemToInventory(toPlayer);
										stack.wasAddedByJourneyman = true;
										jq.inventory.Add(stack);
										didAnything = true;
										goto foundItem;
									}
								}
								for(int b = Main.instance.player.builtItems.Count-1; b>=0; b--) {
									Industry ind = Main.instance.player.builtItems[b];
									if(ind.hasReqType(req)) {
										int v = Mathf.RoundToInt(ind.getStackSizeForQuest() * Main.instance.GetQuestStackMultiplier(ind, jq.numQuestsBefore) * jq.getGoal().getReqScalar());
										if(ind.quantityStored >= v) {
											ItemStack stack = new ItemStack(ind, v);
											jq.inventory.Add(stack);
											didAnything = true;
											goto foundItem;
										}
									}
								}
							}
						}
						if(jq.inventory.Count < 4 && jq.inventory.Count(x => x.doesStackHave(RequirementType.MANA)) < 2) {
							for(int b = Main.instance.player.builtItems.Count - 1; b >= 0; b--) {
								Industry ind = Main.instance.player.builtItems[b];
								if(ind.hasReqType(RequirementType.MANA)) {
									int v = Mathf.RoundToInt(ind.getStackSizeForQuest() * Main.instance.GetQuestStackMultiplier(ind, jq.numQuestsBefore) * jq.getGoal().getReqScalar());
									if(ind.quantityStored >= v) {
										ItemStack stack = new ItemStack(ind, v);
										jq.inventory.Add(stack);
										didAnything = true;
										goto foundItem;
									}
								}
							}
						}
						if(jq.inventory.Count < 6 && jq.inventory.Count(x => x.doesStackHave(RequirementType.HEALING)) < 2) {
							for(int b = Main.instance.player.builtItems.Count - 1; b >= 0; b--) {
								Industry ind = Main.instance.player.builtItems[b];
								if(ind.hasReqType(RequirementType.HEALING)) {
									int v = Mathf.RoundToInt(ind.getStackSizeForQuest() * Main.instance.GetQuestStackMultiplier(ind, jq.numQuestsBefore) * jq.getGoal().getReqScalar());
									if(ind.quantityStored >= v) {
										ItemStack stack = new ItemStack(ind, v);
										jq.inventory.Add(stack);
										didAnything = true;
										goto foundItem;
									}
								}
							}
						}
						j--;
						questIndex++;
						foundItem:
						;
					}
					if(!didAnything) {
						adjustQuestTime += 300;
					}
				}
				newQuestDelayTimer -= adjustQuestTime;
				for(int j = 0; j <= questIndex && j < availableQuests.Count; j++) {
					Quest q = availableQuests[j];
					if(q.guiItem != null) {
						for(int r = 1; r <= 6; r++) {
							Image im = q.guiItem.transform.Find("Inven" + r).GetComponent<Image>();
							im.sprite = GuiManager.instance.gray_square;
							if(q.inventory.Count >= r) {
								if(q.inventory[r - 1].item.industry != null) {
									//Debug.Log(r + ": " + q.inventory[r - 1].item.name);
									im.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.inventory[r - 1].item.name);
									//im.GetComponent<Image>().sprite = q.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Img").GetComponent<Image>().sprite;
								}
								else {
									im.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.inventory[r - 1].item.name);
								}
							}
						}
					}
				}
			}
		}

		private static void des(Quest x) {
			Main.Destroy(x.guiItem);
		}
		public static ItemStack makeRelic(ItemStack stack, ObstacleType ob, string otherData, string hero) {
			if(ob is IRelicMaker) {
				IRelicMaker maker = (IRelicMaker)ob;
				if(stack.relicData == null) {
					stack.relicData = new List<RelicInfo>();
				}
				RelicInfo info = new RelicInfo(hero, maker.relicNames(stack), string.Format(maker.relicDescription(stack),otherData), ob.getRewardScalar());
				Debug.Log(info);
				stack.relicData.Add(info);
				stack.isIDedByPlayer = false;
			}
			return stack;
		}

		public static ItemStack makeRelic(ItemStack stack, ObstacleType ob, string hero) {
			if(ob is IRelicMaker) {
				IRelicMaker maker = (IRelicMaker)ob;
				if(stack.relicData == null) {
					stack.relicData = new List<RelicInfo>();
				}
				RelicInfo info = new RelicInfo(hero, maker.relicNames(stack), maker.relicDescription(stack), ob.getRewardScalar());
				Debug.Log(info);
				stack.relicData.Add(info);
				stack.isIDedByPlayer = false;
			}
			return stack;
		}

		public static ItemStack makeRelic(ItemStack stack, IRelicMaker ob, int scalar, string hero) {
			IRelicMaker maker = ob;
			if(stack.relicData == null) {
				stack.relicData = new List<RelicInfo>();
			}
			RelicInfo info = new RelicInfo(hero, maker.relicNames(stack), maker.relicDescription(stack), scalar);
			Debug.Log(info);
			stack.relicData.Add(info);
			stack.isIDedByPlayer = false;
			return stack;
		}

		public static QuestChallenge getGoal(Quest q) {
			int j = q.obstacles.Length;
			do {
				j--;
			} while(j >= 0 && !(q.obstacles[j].type is IRelicMaker));
			if(j < 0) {
				j = q.obstacles.Length;
				do {
					j--;
					//Debug.Log(q.obstacles[j].type + ":" + ((q.obstacles[j].type is IRelicMaker) ? " is a RelicMaker" : " is not"));
				} while(j >= 1 && !(q.obstacles[j].type is IRelicMaker));
				//Debug.Log("    " + q.obstacles[j].type.name);
				if(q.obstacles[j].type is IDescriptorData) {
					return q.obstacles[j];
				}
				else {
					return q.obstacles[j];
				}
			}
			else {
				return q.obstacles[j];
			}
		}

		private static ItemStack makeRelic(ItemStack stack, Quest q) {
			Debug.Log("Making a relic! " + stack.item.name);
			Debug.Log("Already have? " + Main.instance.player.miscInventory.Contains(stack));
			if(Main.instance.player.miscInventory.Contains(stack)) {
				Debug.Log(q.heroName + ":" + q.getGoal().name);
				Debug.Log("UnIDed? " + Main.instance.player.unidentifiedRelics.Contains(stack));
				Debug.Log("Available? " + QuestManager.availableRelics.Contains(stack));
			}
			QuestChallenge goal = getGoal(q);
			if(goal.type is IDescriptorData && q.miscData != null) {
				object namewrap;
				q.miscData.TryGetValue(((IDescriptorData)goal.type).getDescValue(), out namewrap);
				return makeRelic(stack, goal.type, (string)namewrap, q.heroName);
			}
			else {
				return makeRelic(stack, goal.type, q.heroName);
			}
		}

		public static ItemStack getRandomTreasure(Quest theQuest) {
			Debug.Log("Available relics: " + availableRelics.Count);
			if(availableRelics.Count == 0) return null;
			ItemStack stack = availableRelics[theQuest.questRand.Next(availableRelics.Count)];
			availableRelics.Remove(stack);
			Debug.Log("Available relics: " + availableRelics.Count);
			return stack;
		}

		public static void activateQuest(Quest q) {
			availableQuests.Remove(q);
			activeQuests.Add(q);
		}

		public static float getNewQuestTimer() {
			return newQuestDelayTimer;
		}

		public static void LoadTimerFromSave(float time) {
			newQuestDelayTimer = time;
		}

		public static float getNewQuestMaxTime() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.QUEST_SPEED, out wrap);
			return (newQuestMaxTime - (((UpgradeFloatValue)wrap).value) - (10 * ((AchievementMulti)StatisticsTracker.vendorsPurchasedAch).getNumAchieved())) * (1 / Main.instance.player.currentGuildmaster.newQuestRateMultiplier());
		}

		public static float getEquipRate() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(UpgradeType.JOURNEYMAN_RATE, out wrap);
			return questEquipTimerMax - ((UpgradeFloatValue)wrap).value;
		}
	}
}
