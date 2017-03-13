using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
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
	public class QuestManager {
		public static List<Quest> availableQuests = new List<Quest>();
		public static List<Quest> activeQuests = new List<Quest>();
		//public static List<ItemStack> allRelics = new List<ItemStack>();
		public static List<ItemStack> availableRelics = new List<ItemStack>();
		private static float newQuestDelayTimer = 0;
		private static float newQuestMaxTime = 1200; //20 minutes
		private static Transform activeQuestList;
		private static Transform questList;
		private static Transform inventoryList;
		private static Transform miscInventoryList;
		private static Industry selectedIndustry;
		private static ItemStack selectedStack;

		private static Dictionary<Industry, GameObject> questInvenList = new Dictionary<Industry, GameObject>();

		public static void setupUI() {
			if(questList == null) {
				questList = GuiManager.instance.questArea.transform.FindChild("Available").GetChild(0).GetChild(0);
				activeQuestList = GuiManager.instance.questArea.transform.FindChild("Active").GetChild(0).GetChild(0);
				inventoryList = GuiManager.instance.questArea.transform.FindChild("Inventory1").GetChild(0).GetChild(0);
				miscInventoryList = GuiManager.instance.questArea.transform.FindChild("Inventory2").GetChild(0).GetChild(0);
			}
			int i = 0;
			validateQuests();
			((RectTransform)questList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 150 + 10);
			questList.transform.localPosition = Vector3.zero;
			inventoryList.hierarchyCapacity = 100 * 30;
			miscInventoryList.hierarchyCapacity = 100 * 30;
			questList.hierarchyCapacity = 50 * 30 + 1500;
			activeQuestList.hierarchyCapacity = 50 * 30 + 1500;
			i = 0;
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
						go.name = ind.name;
						Text tx = go.transform.FindChild("Title").GetComponent<Text>();
						tx.text = Main.ToTitleCase(ind.name);
						tx.fontSize = 28;
						go.transform.FindChild("Quantity").GetComponent<Text>().text = "0 / " + Main.AsCurrency(ind.quantityStored);
						go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + ind.name);
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
								go.transform.FindChild("Req" + r).gameObject.SetActive(false);
							}
							else {
								go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
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
		}

		private static void CreateNewQuestGuiItem(Quest q, int i) {
			GameObject go = Main.Instantiate(PrefabManager.instance.QUEST_GUI_LISTITEM, questList) as GameObject;
			Quest theQuest = q;
			q.guiItem = go;
			//go.transform.SetParent(questList);
			go.transform.localPosition = new Vector3(7, (i * -150) - 7, 0);
			go.transform.FindChild("Name").GetComponent<Text>().text = ToTitleCase(q.obstacles[q.obstacles.Length - 1].type.name);
			go.transform.FindChild("Hero").GetComponent<Text>().text = "Hero: " + q.heroName;
			for(int r = 1; r <= 6; r++) {
				int req_num = Main.BitNum(q.getReq(r - 1)) - 1;
				if(req_num < 0) {
					go.transform.FindChild("Req" + r).gameObject.SetActive(false);
				}
				else {
					go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num];
				}
			}
			Button b1 = go.transform.FindChild("Start").GetComponent<Button>();
			b1.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b1.transform.position + Vector3.right*45, "Start the quest now with the the inventory shown above.\nYou do not have to supply any items, but the quest will likely fail.\nSuccessful quests will generate additional rewards.", 6.5F); }, false);
			b1.onClick.AddListener(delegate { startQuest(theQuest, go); });
			Button b2 = go.transform.FindChild("Cancel").GetComponent<Button>();
			b2.onClick.AddListener(delegate { removeQuest(theQuest, go); newQuestDelayTimer -= 300; });
			b2.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b2.transform.position + Vector3.right * 45, "Ignore the quest. Reduces the time until the next quest by " + Main.SecondsToTime(300) + ".", 3.5f); }, false);
			for(int r = 1; r <= 6; r++) {
				Transform btn = go.transform.FindChild("Inven" + r);
				int slotnum = r - 1;
				btn.GetComponent<Button>().onClick.AddListener(delegate { AddRemoveItemFromQuest(theQuest, slotnum); });
				if(q.inventory.Count >= r) {
					if(q.inventory[r - 1].item.industry != null) {
						q.guiItem.transform.FindChild("Inven" + r).GetComponent<Image>().sprite = q.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Img").GetComponent<Image>().sprite;
					}
					else {
						q.guiItem.transform.FindChild("Inven" + r).GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + theQuest.inventory[r - 1].item.name);
					}
				}
			}
			go.transform.FindChild("Reward1").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.rewards[0].item.name);
			go.transform.FindChild("Reward2").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + q.rewards[1].item.name);
			go.transform.FindChild("Expire").GetComponent<Text>().text = "Expires in " + Main.SecondsToTime((int)q.timeUntilQuestExpires);
			Button b3 = go.transform.FindChild("RewardLabel").GetChild(0).GetComponent<Button>();
			b3.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b3.transform.position + Vector3.right * 92, "You will get these items when the quest is started.", 3); }, false);
			Button b4 = go.transform.FindChild("ReqHover").GetComponent<Button>();
			b4.AddHover(delegate (Vector3 p) { if(go.transform.localPosition.y == -7) GuiManager.ShowTooltip(b4.transform.position + Vector3.right * 60, "Items that supply these traits will surely aid the hero.", 5); }, false);
		}

		private static void CreateNewActiveQuestGuiItem(Quest q) {
			GameObject go = Main.Instantiate(PrefabManager.instance.ACTIVE_QUEST_GUI_LISTITEM, activeQuestList) as GameObject;
			//Quest theQuest = q;
			q.guiItem = go;
			//go.transform.SetParent(activeQuestList);

			//go.transform.FindChild("Name").GetComponent<Text>().text = ToTitleCase(q.obstacles[q.obstacles.Length - 1].type.name);
			go.transform.FindChild("Hero").GetComponent<Text>().text = q.heroName;
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
		}

		private static void SelectItem(Industry ind) {
			selectedIndustry = ind;
		}

		private static void AddRemoveItemFromQuest(Quest theQuest, int slot) {
			if(selectedIndustry == null && selectedStack == null) {
				if(slot < theQuest.inventory.Count) {
					theQuest.inventory.Remove(theQuest.inventory[slot]);
				}
			}
			else if(slot < theQuest.inventory.Count && ((selectedIndustry != null && theQuest.inventory[slot].item == selectedIndustry.industryItem) || (selectedStack != null && theQuest.inventory[slot].item == selectedStack.item))) {
				theQuest.inventory.Remove(theQuest.inventory[slot]);
			}
			else {
				if(selectedIndustry != null) {
					ItemStack stack = new ItemStack(selectedIndustry, Mathf.RoundToInt(selectedIndustry.getStackSizeForQuest() * Main.instance.GetQuestStackMultiplier(selectedIndustry, theQuest.numQuestsBefore) * theQuest.getGoal().getReqScalar()));
					if(slot < theQuest.inventory.Count) {
						theQuest.inventory[slot] = stack;
					}
					else {
						theQuest.inventory.Add(stack);
					}
				}
				else if(selectedStack != null) {
					//TODO: update UI
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
									Image im = q.guiItem.transform.FindChild("Inven" + r).GetComponent<Image>();
									im.sprite = GuiManager.instance.gray_square;
									if(q.inventory.Count >= r) {
										if(q.inventory[r - 1].item.industry != null) {
											im.GetComponent<Image>().sprite = q.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Img").GetComponent<Image>().sprite;
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
				Image im = theQuest.guiItem.transform.FindChild("Inven" + r).GetComponent<Image>();
				im.sprite = GuiManager.instance.gray_square;
				if(theQuest.inventory.Count >= r) {
					if(theQuest.inventory[r - 1].item.industry != null) {
						im.GetComponent<Image>().sprite = theQuest.inventory[r - 1].item.industry.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Img").GetComponent<Image>().sprite;
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
			i = 0;
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				if(stack.item.canBeGivenToQuests) {
					ItemStack s = stack;
					GameObject go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM, miscInventoryList) as GameObject;
					//go.transform.SetParent(miscInventoryList);
					go.transform.localPosition = new Vector3(7, (i * -125) - 5, 0);
					//ind.invenListObj = go;
					go.name = stack.item.name;
					Text tx = go.transform.FindChild("Title").GetComponent<Text>();
					tx.text = Main.ToTitleCase(stack.getDisplayName());
					if(stack.relicData == null)
						tx.fontSize = 28;
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize);
					go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
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
					go.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(s); });
					i++;
				}
			}
			((RectTransform)miscInventoryList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);
			miscInventoryList.transform.localPosition = Vector3.zero;
		}

		private static void validateQuests() {
			int i = 0;
			foreach(Industry ind in Main.instance.player.builtItems) {
				GameObject go;// = ind.questInvenListObj;
				questInvenList.TryGetValue(ind, out go);
				if(go != null) {
					go.transform.localPosition = new Vector3(7, (i * -125) - 7, 0);
					go.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(ind.name);
					go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + ind.name);
					int total = 0;
					foreach(Quest q in availableQuests) {
						foreach(ItemStack stack in q.inventory) {
							if(stack.item == ind.industryItem) {
								total += stack.stackSize;
							}
						}
					}
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + total;
					//bool haveEnough = ind.quantityStored >= total;
					
					i++;
				}
			}
			((RectTransform)inventoryList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);


			foreach(Quest q in availableQuests) {
				bool questReady = true;
				long totalForQuest = 0;
				foreach(ItemStack stack in q.inventory) {
					totalForQuest = 0;
					if(stack.item.industry != null) {
						totalForQuest += stack.stackSize;
						questReady &= (stack.item.industry.quantityStored >= totalForQuest);
					}
				}
				q.guiItem.transform.FindChild("Start").GetComponent<Button>().interactable = questReady;
			}
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
			Button b = theQuest.guiItem.transform.FindChild("Start").GetComponent<Button>();
			b.RemoveAllEvents();
			b = theQuest.guiItem.transform.FindChild("Cancel").GetComponent<Button>();
			b.RemoveAllEvents();
			b = go.transform.FindChild("RewardLabel").GetChild(0).GetComponent<Button>();
			b.RemoveAllEvents();
			b = go.transform.FindChild("ReqHover").GetComponent<Button>();
			b.RemoveAllEvents();

			availableQuests.Remove(theQuest);
			Main.Destroy(go);
			int i = 0;
			foreach(Quest q in availableQuests) {
				q.guiItem.transform.localPosition = new Vector3(7, (i * -150) - 7, 0);
				i++;
			}
			((RectTransform)questList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 150 + 10);
			questList.transform.localPosition = Vector3.zero;
			selectedIndustry = null;
		}

		private static void startQuest(Quest theQuest, GameObject go) {
			removeQuest(theQuest, go);
			activeQuests.Add(theQuest);

			foreach(ItemStack stack in theQuest.inventory) {
				if(stack.item.industry != null && stack.relicData == null && stack.enchants.Count == 0) {
					stack.item.industry.quantityStored -= stack.stackSize;
				}
				else {
					Main.instance.player.miscInventory.Remove(stack);
					if(stack.enchants.Count > 0 || stack.relicData != null) {
						ItemStack toPlayer = stack.split(stack.stackSize - 1);
						Main.instance.player.addItemToInventory(toPlayer);
					}
					else {
						ItemStack toPlayer = stack.split(stack.stackSize - stack.item.getStackSizeForQuest());
						Main.instance.player.addItemToInventory(toPlayer);
					}
				}
				stack.setToMaxSize();
			}
			theQuest.questStarted();
			Main.instance.player.addItemToInventory(theQuest.rewards[0]);
			Main.instance.player.addItemToInventory(theQuest.rewards[1]);
			selectedIndustry = null;
			selectedStack = null;
			//setupUI();
			refreshMiscInventory();
			validateQuests();
			CreateNewActiveQuestGuiItem(theQuest);
			updateActiveQuestList();
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
			((RectTransform)questList).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 150 + 10);
			//questList.transform.localPosition = Vector3.zero;
			//i = 0;
			validateQuests();
			//inventoryList.transform.localPosition = Vector3.zero;

			GuiManager.instance.questHeader.transform.FindChild("MoneyArea").GetChild(0).GetComponent<Text>().text = "$" + Main.AsCurrency(Main.instance.player.money, 12);
			GuiManager.instance.questHeader.transform.FindChild("QuestArea").GetChild(0).GetComponent<Text>().text = Main.AsCurrency(Main.instance.player.totalQuestsCompleted, 12);
		}

		protected static void updateActiveQuestList() {
			int i = 0;
			foreach(Quest q in activeQuests) {
				GameObject go = q.guiItem;
				if(q.isActive()) {
					go.transform.localPosition = new Vector3(7, (i * -68) - 7, 0);
					QuestInfo info = go.GetComponent<QuestInfo>();
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
					float per = (float)q.heroCurHealth / q.heroMaxHealth;
					float ext = 0;
					Image img = info.HPBar.GetComponent<Image>();
					ext = img.material.GetFloat("_Cutoff");
					ext = MathHelper.EaseOutQuadratic(0.1f, ext, 1 - per, 1);
					img.material.SetFloat("_Cutoff", ext);

					per = q.QuestTimeLeft() / 2000;
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

					i++;
				}
				else {
					Main.Destroy(go);
				}
			}
		}

		public static void tickAllQuests(float time) {
			newQuestDelayTimer -= time;
			foreach(Quest q in activeQuests) {
				EnumResult res = q.doQuestStep(time);
				if(res != EnumResult.CONTINUE) {
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
							if(st.relicData != null || st.enchants.Count > 0 /*&& st != newRelic*/) {
								//if the player gave it to the hero, then it's ID'd and can go back to the player's general inventory
								//if it isn't, then it goes to the Unidentified Relics list
								Debug.Log(st.isIDedByPlayer);
								if(st.isIDedByPlayer)
									Main.instance.player.addItemToInventory(st);
								else
									Main.instance.player.unidentifiedRelics.Add(st);
								
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
			activeQuests.Where(x => !x.isActive()).ToList().ForEach(x => Main.Destroy(x.guiItem));
			activeQuests.RemoveAll(x => !x.isActive());
			while(newQuestDelayTimer <= 0) {
				newQuestDelayTimer += getNewQuestMaxTime();
				Quest q = Quest.GenerateNewQuest();
				q.timeUntilQuestExpires = 18000; //5 hours
				availableQuests.Add(q);
			}
			foreach(Quest q in availableQuests) {
				q.timeUntilQuestExpires -= time;
				if(q.guiItem != null)
					q.guiItem.transform.FindChild("Expire").GetComponent<Text>().text = "Expires in " + Main.SecondsToTime((int)q.timeUntilQuestExpires);
				if(q.timeUntilQuestExpires <= 0) {
					Main.Destroy(q.guiItem);
				}
			}
			availableQuests.RemoveAll(x => x.timeUntilQuestExpires <= 0);
			GuiManager.instance.questHeader.transform.FindChild("NewQuestTime").GetComponent<Text>().text = "New Quest in " + Main.SecondsToTime((int)newQuestDelayTimer);
			updateActiveQuestList();
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

		private static ItemStack makeRelic(ItemStack stack, Quest q) {
			Debug.Log("Making a relic!");
			int j = q.obstacles.Length;
			do {
				j--;
			} while(j >= 0 && !(q.obstacles[j].type is IRelicMaker));
			if(j < 0) {
				j = q.obstacles.Length;
				do {
					j--;
					Debug.Log(q.obstacles[j].type.name + ":"  + ((q.obstacles[j].type is IRelicMaker)?" is a RelicMaker":" is not"));
				} while(j >= 0 && !(q.obstacles[j].type is IRelicMaker));
				Debug.Log("    " + q.obstacles[j].type.name);
				return makeRelic(stack, q.obstacles[j].type, q.heroName);
			}
			else {
				return makeRelic(stack, q.obstacles[j].type, q.heroName);
			}
			/*QuestChallenge ob = q.obstacles.Last();
			if(ob.type is IQuestGoal) {
				IQuestGoal goal = (IQuestGoal)ob.type;
				if(stack.relicData == null) {
					stack.relicData = new List<RelicInfo>();
				}
				RelicInfo info = new RelicInfo(goal.relicNames(stack), goal.relicDescription(stack), ob.type.getRewardScalar());
				Debug.Log(info);
				stack.relicData.Add(info);
				stack.isIDedByPlayer = true;// false;
			}
			return stack;*/
		}

		public static ItemStack getRandomTreasure(Quest theQuest) {
			if(availableRelics.Count == 0) return null;
			ItemStack stack = availableRelics[theQuest.questRand.Next(availableRelics.Count)];
			availableRelics.Remove(stack);
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
			return newQuestMaxTime - ((UpgradeFloatValue)wrap).value;
		}
	}
}
