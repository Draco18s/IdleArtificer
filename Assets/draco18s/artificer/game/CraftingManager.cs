﻿using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.game {
	public class CraftingManager {
		public static List<GameObject> buildButtons;
		public static Industry selectedIcon;
		public static bool doSynchronize = false;
		protected static Text numVendors;

		private static Vector3 lastPos;
		public static void setupUI() {
			numVendors = GuiManager.instance.craftHeader.transform.FindChild("AvailableVendors").GetComponent<Text>();
			numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
			buildButtons = new List<GameObject>();
			int i = 0;
			FieldInfo[] fields = typeof(Industries).GetFields();
			GuiManager.instance.buildingList.transform.transform.hierarchyCapacity = (fields.Length + 1) * 17;
			foreach(FieldInfo field in fields) {
				GameObject it = Main.Instantiate(PrefabManager.instance.BUILDING_GUI_LISTITEM);
				buildButtons.Add(it);
				Industry item = (Industry)field.GetValue(null);
				it.transform.SetParent(GuiManager.instance.buildingList.transform);
				it.transform.localPosition = new Vector3(6, i * -141 - 5, 0);
				it.name = field.Name;
				it.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(item.name);
				it.transform.FindChild("Cost").GetComponent<Text>().text = "$" + Main.AsCurrency(item.cost);
				it.transform.FindChild("Value").GetComponent<Text>().text = "$" + Main.AsCurrency(item.GetSellValue());
				it.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.name);
				Transform outnum = it.transform.FindChild("OutNum");
				outnum.GetComponent<Text>().text = "x" + Main.AsCurrency(item.output);
				outnum.GetChild(0).GetComponent<Text>().text = "x" + Main.AsCurrency(item.output);

				for(int j = 1; j <= 3; j++) {
					Transform go = it.transform.FindChild("Input" + j);
					if(item.inputs.Count >= j) {
						IndustryInput input = item.inputs[j - 1];
						go.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + input.item.name);
						go.GetChild(0).GetComponent<Text>().text = "x" + item.inputs[j-1].quantity;
						go.GetChild(0).GetChild(0).GetComponent<Text>().text = "x" + item.inputs[j-1].quantity;
					}
					else {
						go.gameObject.SetActive(false);
					}
				}

				//item.listObj = it;
				Button b = it.GetComponent<Button>();
				b.onClick.AddListener(delegate { BuildIndustry(item); });
				ItemButtonData dat = it.GetComponent<ItemButtonData>();
				dat.connectedItem = item;
				i++;
			}
			((RectTransform)GuiManager.instance.buildingList.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (i * 141 + 10));
			GuiManager.instance.buildingList.transform.localPosition = Vector3.zero;
		}

		private static bool isShiftDown;
		private static bool isCntrlDown;
		private static BigInteger cachedMoney = 0;

		public static void Update() {
			isShiftDown = Input.GetButton("Shift");
			isCntrlDown = Input.GetButton("Control");
			if(GuiManager.instance.craftArea.activeSelf) {
				Profiler.BeginSample("Check buildable");
				if(BigInteger.Abs(cachedMoney - Main.instance.player.money) > (BigRational)Main.instance.player.money * 0.05) {
					cachedMoney = Main.instance.player.money;
					BigRational c;
					foreach(GameObject bt in buildButtons) {
						ItemButtonData dat = bt.GetComponent<ItemButtonData>();
						c = dat.connectedItem.GetScaledCost();
						bt.transform.FindChild("Cost").GetComponent<Text>().text = "$" + Main.AsCurrency((BigInteger)c);
						if(c > Main.instance.player.money) {
							dat.GetComponent<Button>().interactable = false;
						}
						else {
							dat.GetComponent<Button>().interactable = true;
						}
					}
				}
				Profiler.EndSample();
				if(Input.GetButtonDown("Cancel")) {
					FacilityUnselected(selectedIcon);
				}
				Profiler.BeginSample("Update selectedIcon");
				if(selectedIcon != null) {
					UpdateIcon();
				}
				Profiler.EndSample();
				Profiler.BeginSample("Check built items");
				foreach(Industry item in Main.instance.player.builtItems) {
					foreach(IndustryInput input in item.inputs) {
						GameObject obj = input.arrow; //Instantiate(PrefabManager.instance.GRID_GUI_ARROW);
						//obj.transform.SetParent(item.craftingGridGO.transform.parent.GetChild(0));
						obj.transform.localPosition = item.craftingGridGO.transform.GetChild(0).localPosition;

						if(input.item.craftingGridGO != null) {
							obj.SetActive(true);
							float dx = input.item.craftingGridGO.transform.GetChild(0).localPosition.x - item.craftingGridGO.transform.GetChild(0).localPosition.x;
							float dy = input.item.craftingGridGO.transform.GetChild(0).localPosition.y - item.craftingGridGO.transform.GetChild(0).localPosition.y;

							float dist = Mathf.Sqrt(dx * dx + dy * dy);
							((RectTransform)obj.transform.GetChild(0)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dist);
							//input.arrow = obj;

							obj.transform.GetChild(0).localPosition = new Vector3((dist / -2) - 32, 0);

							obj.transform.localEulerAngles = new Vector3(0, 0, 180 + MathHelper.RadiansToDegrees(Mathf.Atan2(dy, dx)));

							obj.transform.GetChild(0).GetComponent<Image>().color = GetColorForProductivity(item, input);
						}
						else if(input.item.craftingGridGO == null && obj.activeSelf) {
							obj.SetActive(false);
						}
					}

					if(Mathf.FloorToInt(Time.time * 10) % 20 == 0) {
						Transform t;
						t = item.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Ico1");
						if(QuestManager.IsIndustryOnQuest(item)) {
							int totalForQuest = 0;
							int totalReady = 0;
							int totalNeed = 0;
							foreach(Quest q in QuestManager.availableQuests) {
								bool questReady = true;
								bool needed = false;
								totalForQuest = 0;
								foreach(ItemStack stack in q.inventory) {
									if(stack.item == item.industryItem) {
										totalForQuest += stack.stackSize;
										questReady &= (item.quantityStored >= totalForQuest);
										needed = true;
									}
								}
								if(needed) {
									totalNeed++;
									if(questReady) {
										totalReady++;
									}
								}
							}
							if(totalReady >= totalNeed) {
								t.GetComponent<Image>().color = Color.green;
							}
							else if(totalReady > 0) {
								t.GetComponent<Image>().color = Color.blue;
							}
							else {
								t.GetComponent<Image>().color = Color.red;
							}
						}
						t.gameObject.SetActive(QuestManager.IsIndustryOnQuest(item));
						t = item.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Ico2");
						if(item.getRawVendors() > 0) {
							BigInteger avaialbleToSell = item.output * item.level + (item.isSellingStores?item.quantityStored:0) - item.consumeAmount;
							BigInteger sellCapacity = item.getVendors() * Main.instance.GetVendorSize();
							if(avaialbleToSell < sellCapacity && avaialbleToSell <= sellCapacity-(item.getOneVendor()*Main.instance.GetVendorSize())) {
								t.GetComponent<Image>().color = Color.red;
							}
							else {
								t.GetComponent<Image>().color = Color.black;
							}
							t.gameObject.SetActive(true);
						}
						else {
							t.gameObject.SetActive(false);
						}
						t = item.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Ico3");
						t.gameObject.SetActive(item.apprentices > 0);
					}
				}
				Profiler.EndSample();
				Profiler.BeginSample("Update money ui");
				GuiManager.instance.currentMoney.GetComponent<Text>().text = "$" + Main.AsCurrency(Main.instance.player.money, 12);
				Profiler.EndSample();

			}
		}
		public static void BuildIndustry(Industry item) {
			BuildIndustry(item, false, false);
		}

		public static void BuildIndustry(Industry item, bool fromSave, bool skip) {
			//FacilityUnselected(selectedIcon);
			if(skip || fromSave || Main.instance.player.money >= item.GetScaledCost()) {
				if(!fromSave && !skip) {
					Main.instance.player.money -= (BigInteger)item.GetScaledCost();
					item.level++;
				}
				if(!Main.instance.player.builtItems.Contains(item)) {
					Main.instance.player.builtItems.Add(item);

					Main.instance.player.builtItems.Sort((a, b) => a.cost.CompareTo(b.cost));
					if(!fromSave)
						item.setTimeRemaining(float.MinValue);
					GameObject it = Main.Instantiate(PrefabManager.instance.BUILDING_GUI_GRIDITEM, GuiManager.instance.gridArea.transform.GetChild(1)) as GameObject;
					it.name = "Grid_Icon_" + item.name;
					//it.transform.SetParent(GuiManager.instance.gridArea.transform);
					it.transform.localPosition = new Vector3(0, 0, 0);
					int y = (Screen.height - 128) / 2;
					y = MathHelper.snap(y, 24);
					it.transform.GetChild(0).localPosition = new Vector3(0, y, 0);

					//if(fromSave || skip) {
						it.transform.GetChild(0).localPosition = item.getGridPos();
					//}

					it.transform.GetChild(0).GetChild(0).FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.name);
					item.craftingGridGO = it;
					Image img = it.transform.GetChild(0).GetChild(0).FindChild("Progress").GetComponent<Image>();
					img.material = Main.Instantiate(img.material);
					SelectObject selObj = it.GetComponentInChildren<SelectObject>();
					Industry ind = item;
					selObj.selectListener(delegate { FacilitySelected(ind); });
					selObj.deselectListener(delegate { FacilityUnselected(ind); });
					Main.instance.player.itemData.Add(it, item);
					/*Industry item2;
					Main.instance.player.itemData.TryGetValue(it, out item2);
					Debug.Log(item.name + " -> " + item2);*/
					foreach(IndustryInput input in item.inputs) {
						GameObject obj = Main.Instantiate(PrefabManager.instance.GRID_GUI_ARROW, GuiManager.instance.gridArea.transform.GetChild(0)) as GameObject;
						//obj.transform.SetParent(it.transform.parent.GetChild(0));
						obj.transform.localPosition = it.transform.GetChild(0).localPosition;
						input.arrow = obj;
						if(input.item.craftingGridGO != null) {
							obj.SetActive(true);
							float dx = input.item.craftingGridGO.transform.GetChild(0).localPosition.x - it.transform.GetChild(0).localPosition.x;
							float dy = input.item.craftingGridGO.transform.GetChild(0).localPosition.y - it.transform.GetChild(0).localPosition.y;

							float dist = Mathf.Sqrt(dx * dx + dy * dy);
							((RectTransform)obj.transform.GetChild(0)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dist);


							obj.transform.GetChild(0).localPosition = new Vector3((dist / -2) - 32, 0);

							obj.transform.localEulerAngles = new Vector3(0, 0, 180 + MathHelper.RadiansToDegrees(Mathf.Atan2(dy, dx)));

							obj.transform.GetChild(0).GetComponent<Image>().color = GetColorForProductivity(item, input);
						}
						else {
							obj.SetActive(false);
						}
						//selectedIcon = item.guiObj;
					}
					if(!fromSave) {
						#region stats and achievements
						if(item == Industries.LEATHER) {
							StatisticsTracker.minQuestDifficulty.addValue(1);
							StatisticsTracker.maxQuestDifficulty.addValue(1);
						}
						if(item == Industries.NIGHTSHADE) {
							StatisticsTracker.maxQuestDifficulty.addValue(1);
						}
						if(item == Industries.POT_POISON) {
							StatisticsTracker.maxQuestDifficulty.addValue(1);
						}
						if(item == Industries.IRON_ORE) {
							StatisticsTracker.maxQuestDifficulty.addValue(2);
							StatisticsTracker.minQuestDifficulty.addValue(1);
						}
						if(item == Industries.GOLD_ORE) {
							StatisticsTracker.maxQuestDifficulty.addValue(2);
							StatisticsTracker.minQuestDifficulty.addValue(1);
						}
						/*if(item == Industries.IRON_ORE) {
							StatisticsTracker.maxQuestDifficulty.addValue(1);
							StatisticsTracker.minQuestDifficulty.addValue(1);
						}
						if(item == Industries.IRON_SWORD) {
							StatisticsTracker.minQuestDifficulty.addValue(1);
						}
						if(item == Industries.IRON_RING) {
							StatisticsTracker.maxQuestDifficulty.addValue(1);
							StatisticsTracker.minQuestDifficulty.addValue(1);
						}*/
						#endregion
					}
					Transform t;
					t = it.transform.GetChild(0).GetChild(0).FindChild("Ico1");
					t.gameObject.SetActive(QuestManager.IsIndustryOnQuest(item));
					t.GetComponent<Button>().AddHover(delegate (Vector3 p) {
						GuiManager.ShowTooltip(t.position + Vector3.right * 10, "Needed for quests", 1);
					});
					t = it.transform.GetChild(0).GetChild(0).FindChild("Ico2");
					t.gameObject.SetActive(item.getRawVendors() > 0);
					t.GetComponent<Button>().AddHover(delegate(Vector3 p) {
						GuiManager.ShowTooltip(t.position + Vector3.right*10, item.getRawVendors() + " vendor(s), selling " + Main.AsCurrency(NumberSoldByVendors(item)) + "/cy", 1);
					});
					t.GetComponent<Button>().onClick.AddListener(delegate { IncreaseVendors(item); });
					t.GetComponent<Button>().OnRightClick(delegate { DecreaseVendors(item); });

					t = it.transform.GetChild(0).GetChild(0).FindChild("Ico3");
					t.gameObject.SetActive(item.apprentices > 0);
					t.GetComponent<Button>().AddHover(delegate (Vector3 p) {
						GuiManager.ShowTooltip(t.position + Vector3.right * 10, item.apprentices + " apprentice(s) assigned", 1);
					});
					t.GetComponent<Button>().onClick.AddListener(delegate { IncreaseApprentices(item); });
					t.GetComponent<Button>().OnRightClick(delegate { DecreaseApprentices(item); });
				}
				Main.instance.mouseDownTime += 1;
			}
		}
		public static void SelectInput(int inputNum) {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				if(inputNum-1 < selectedIcon.inputs.Count) {
					//FacilityUnselected(selectedIcon);
					FacilitySelected(selectedIcon.inputs[inputNum-1].item);
					//Main.instance.mouseDownTime += 1;
				}
			}
		}
		public static void SelectOutput(int inputNum) {
			if(selectedIcon != null) {
				//Industry selIndust;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out selIndust);
				List<IndustryOutput> consumers = new List<IndustryOutput>();
				foreach(GameObject go in buildButtons) {
					ItemButtonData dat = go.GetComponent<ItemButtonData>();
					if(dat.connectedItem.inputs.Count > 0) {
						foreach(IndustryInput inp in dat.connectedItem.inputs) {
							if(inp.item == selectedIcon) {
								consumers.Add(new IndustryOutput(dat.connectedItem, inp.quantity));
							}
						}
					}
				}
				Debug.Log(inputNum + " > " + consumers.Count);
				Debug.Log(consumers[inputNum].item.name);
				FacilitySelected(consumers[inputNum].item);
			}
		}

		public static void FacilitySelected(Industry go) {
			if(go.craftingGridGO == null) {
				lastLevel = -1;
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
				selectedIcon = go;
				//lastPos = selectedIcon.craftingGridGO.transform.GetChild(0).localPosition;
				GuiManager.instance.infoPanel.transform.FindChild("Output").gameObject.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + selectedIcon.name);
			}
			else {
				lastLevel = -1;
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
				selectedIcon = go;
				lastPos = selectedIcon.craftingGridGO.transform.GetChild(0).localPosition;
				//GuiManager.instance.infoPanel.transform.localPosition = new Vector3(-1465, 55, 0);
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				GuiManager.instance.infoPanel.transform.FindChild("Output").gameObject.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + selectedIcon.name);
			}
		}

		public static void ShowInfo() {
			if(selectedIcon == null) return;

			Vector3 pos = selectedIcon.craftingGridGO.transform.GetChild(0).localPosition;//new Vector3(selectedIcon.transform.localPosition.x, selectedIcon.transform.localPosition.y + 50, 0);
			int v = (Screen.height) / 2;
			pos.y -= v;
			int h = (Screen.width / 2) - 170;
			pos.x = Mathf.Clamp(pos.x, 115 - h, h);
			pos.y = Mathf.Clamp(pos.y, -v + 170, v - 270) + 0.5f;

			GuiManager.instance.infoPanel.transform.localPosition = pos;
			ShowConsumers();
		}

		protected static void ShowConsumers() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			//Industry selIndust;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out selIndust);
			List<IndustryOutput> consumers = new List<IndustryOutput>();
			foreach(GameObject go in buildButtons) {
				ItemButtonData dat = go.GetComponent<ItemButtonData>();
				if(dat.connectedItem.inputs.Count > 0) {
					foreach(IndustryInput inp in dat.connectedItem.inputs) {
						if(inp.item == selectedIcon) {
							consumers.Add(new IndustryOutput(dat.connectedItem,inp.quantity));
						}
					}
				}
			}
			int i = 0;
			int needs = 0;
			GameObject img;
			int vMod = (consumers.Count<=6?32:20);
			if(info.ConsumersDock.childCount >= consumers.Count) {
				foreach(Transform child in info.ConsumersDock) {
					if(i + 1 > consumers.Count) {
						Main.Destroy(child.gameObject);
					}
					else {
						needs += consumers[i].quantity;
						img = child.GetChild(0).gameObject;
						img.transform.parent.localPosition = new Vector3((i % 3 * 32), -(i / 3 * vMod), 0);
						img.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + consumers[i].item.name);
					}
					i++;
				}
			}
			else {
				foreach(IndustryOutput inp in consumers) {
					if(i + 1 > info.ConsumersDock.childCount) {
						int j = i;
						GameObject img2 = new GameObject("Consume" + (i + 1) + "_bg");
						Image img_ = img2.AddComponent<Image>();
						img2.transform.SetParent(info.ConsumersDock);
						((RectTransform)img2.transform).sizeDelta = new Vector2(32, 32);
						((RectTransform)img2.transform).pivot = new Vector2(0, 1);
						//Image img_ = img2.GetComponent<Image>();
						img_.sprite = GuiManager.instance.inner_item_bg;
						img_.type = Image.Type.Sliced;
						img_.raycastTarget = false;

						img = new GameObject("Consume"+(i+1));
						img.AddComponent<Image>();
						img.transform.SetParent(img2.transform);
						((RectTransform)img.transform).sizeDelta = new Vector2(30, 30);
						((RectTransform)img.transform).pivot = new Vector2(0, 1);
						img.transform.localPosition = new Vector3(1, -0.8f, 0);
						Button btn = img.AddComponent<Button>();
						btn.onClick.AddListener(delegate {
							CraftingManager.SelectOutput(j);
						});
					}
					else {
						img = info.ConsumersDock.GetChild(i).GetChild(0).gameObject;
					}
					needs += inp.quantity;
					img.transform.parent.localPosition = new Vector3((i % 3 * 32), -(i / 3 * vMod), 0);
					img.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + inp.item.name);
					i++;
				}
			}
		}

		public static void IncreaseVendors() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				IncreaseVendors(selectedIcon);
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().VendNum.text = "" + selectedIcon.getRawVendors();
			}
		}
		public static void IncreaseVendors(Industry item) {
			if(Main.instance.player.currentVendors < Main.instance.player.maxVendors) {
				int num = (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
				int maxAdd = Main.instance.player.maxVendors - Main.instance.player.currentVendors;
				num = Math.Min(num, maxAdd);
				//Debug.Log(Main.instance.player.currentVendors + "+" + num);
				item.AdjustVendors(item.getRawVendors() + num);
				Main.instance.player.currentVendors = Math.Min(Main.instance.player.currentVendors + num, Main.instance.player.maxVendors);

				Transform t = item.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Ico2");
				if(item.getRawVendors() > 0) {
					BigInteger avaialbleToSell = item.output * item.level + (item.isSellingStores ? item.quantityStored : 0) - item.consumeAmount;
					BigInteger sellCapacity = item.getVendors() * Main.instance.GetVendorSize();
					if(avaialbleToSell < sellCapacity && avaialbleToSell <= sellCapacity - (item.getOneVendor() * Main.instance.GetVendorSize())) {
						t.GetComponent<Image>().color = Color.red;
					}
					else {
						t.GetComponent<Image>().color = Color.black;
					}
					t.gameObject.SetActive(true);
				}
				else {
					t.gameObject.SetActive(false);
				}
			}
			numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
		}

		public static void DecreaseVendors() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				DecreaseVendors(selectedIcon);
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().VendNum.text = "" + selectedIcon.getRawVendors();
			}
		}
		public static void DecreaseVendors(Industry item) {
			if(item.getRawVendors() > 0) {
				int num = (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
				num = Math.Min(num, item.getRawVendors());
				item.AdjustVendors(item.getRawVendors() - num);
				Main.instance.player.currentVendors -= num;

				Transform t = item.craftingGridGO.transform.GetChild(0).GetChild(0).FindChild("Ico2");
				if(item.getRawVendors() > 0) {
					BigInteger avaialbleToSell = item.output * item.level + (item.isSellingStores ? item.quantityStored : 0) - item.consumeAmount;
					BigInteger sellCapacity = item.getVendors() * Main.instance.GetVendorSize();
					if(avaialbleToSell < sellCapacity && avaialbleToSell <= sellCapacity - (item.getOneVendor() * Main.instance.GetVendorSize())) {
						t.GetComponent<Image>().color = Color.red;
					}
					else {
						t.GetComponent<Image>().color = Color.black;
					}
					t.gameObject.SetActive(true);
				}
				else {
					t.gameObject.SetActive(false);
				}
			}
			numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
		}

		public static void IncreaseApprentices() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				IncreaseApprentices(selectedIcon);
			}
			//numVendors.text = Main.instance.player.currentApprentices + " of " + Main.instance.player.maxApprentices + " vendors in use";
		}
		public static void IncreaseApprentices(Industry item) {
			if(Main.instance.player.currentApprentices < Main.instance.player.maxApprentices) {
				//item.AdjustVendors(item.getRawVendors() + 1);
				item.apprentices += 1;
				Main.instance.player.currentApprentices = Math.Min(Main.instance.player.currentApprentices + 1, Main.instance.player.maxApprentices);
			}
		}

		public static void DecreaseApprentices() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				DecreaseApprentices(selectedIcon);
			}
			//numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
		}
		public static void DecreaseApprentices(Industry item) {
			if(item.apprentices > 0) {
				item.apprentices -= 1;
				Main.instance.player.currentApprentices -= 1;
			}
		}

		public static void IncreaseAutoBuild() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				selectedIcon.autoBuildLevel += (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
			}
		}

		public static void DecreaseAutoBuild() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				if(selectedIcon.autoBuildLevel > 0) {
					selectedIcon.autoBuildLevel = Math.Max(selectedIcon.autoBuildLevel - (isCntrlDown ? 50 : (isShiftDown ? 10 : 1)), 0);
				}
			}
		}

		public static void IncreaseBuildMagnitude() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				selectedIcon.autoBuildMagnitude += (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
			}
		}

		public static void DecreaseBuildMagnitude() {
			if(selectedIcon != null) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				if(selectedIcon.autoBuildMagnitude > 0) {
					selectedIcon.autoBuildMagnitude = Math.Max(selectedIcon.autoBuildMagnitude - (isCntrlDown ? 50 : (isShiftDown ? 10 : 1)), 0);
				}
			}
		}

		public static void AdvanceTimer() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			selectedIcon.addTimeRaw(-Main.instance.GetClickRate());
			//item.timeRemaining -= Main.instance.GetClickRate();
		}

		public static void SellAll() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			Main.instance.player.AddMoney(selectedIcon.GetSellValue() * selectedIcon.quantityStored);
			selectedIcon.quantityStored = 0;
		}

		public static BigInteger SellAllValue() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return (selectedIcon.GetSellValue() * selectedIcon.quantityStored);
		}

		public static BigInteger GetQuantity() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return selectedIcon.quantityStored;
		}

		public static string GetName() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return selectedIcon.name;
		}
		public static BigInteger ValueSoldByVendors() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return ValueSoldByVendors(selectedIcon);
		}
		public static BigInteger NumberSoldByVendors() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return NumberSoldByVendors(selectedIcon);
		}

		public static BigInteger ValueSoldByVendors(Industry item) {
			BigInteger num = NumberSoldByVendors(item);
			num *= item.GetSellValue();
			num = (BigInteger)((BigRational)num * Main.instance.GetVendorValue());
			return num;
		}

		public static BigInteger NumberSoldByVendors(Industry item) {
			BigInteger num = 0;
			if(item.isSellingStores) {
				BigInteger m = item.output * item.level + item.quantityStored - item.consumeAmount;
				if(m > item.getVendors() * Main.instance.GetVendorSize()) {
					num = item.getVendors() * Main.instance.GetVendorSize();
				}
				else {
					num = m;
				}
			}
			else {
				if(item.output * item.level < item.getVendors() * Main.instance.GetVendorSize())
					num = item.output * item.level;
				else
					num = item.getVendors() * Main.instance.GetVendorSize();
			}

			return num;
		}

		public static void UpgradeCurrent() {
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			BigInteger spendCost = 0;
			int buyNum = 1;
			if(isShiftDown) {
				buyNum = 10;
				BigInteger c = (BigInteger)selectedIcon.GetScaledCost(buyNum);
				spendCost = c;
			}
			else if(isCntrlDown) {
				buyNum = 50;
				BigInteger c = (BigInteger)selectedIcon.GetScaledCost(buyNum);
				spendCost = c;
			}
			else {
				spendCost = (BigInteger)selectedIcon.GetScaledCost();
			}
			if(Main.instance.player.money >= spendCost) {
				Main.instance.player.money -= spendCost;
				if(selectedIcon.level == 0) {
					BuildIndustry(selectedIcon, false, true);
				}
				selectedIcon.level+=buyNum;
			}
			GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
		}

		public static void DowngradeCurrent() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			info.DowngradeBtn.gameObject.SetActive(false);
		}

		public static void Do_DowngradeCurrent() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			//BigInteger spendCost = item.GetScaledCost();
			int buyNum = 1;
			if(isShiftDown) {
				buyNum = 10;
			}
			else if(isCntrlDown) {
				buyNum = 50;
			}
			selectedIcon.level -= buyNum;
			info.DowngradeBtn.gameObject.SetActive(true);
			if(selectedIcon.level <= 0) {
				Main.instance.player.builtItems.Remove(selectedIcon);
				Main.Destroy(selectedIcon.craftingGridGO);
				selectedIcon.craftingGridGO = null;
				if(Main.instance.player.builtItems.Count == 0 && Main.instance.player.money < 10) {
					Main.instance.player.money = 20;// Main.instance.player.GetStartingCash();
				}
			}
		}

		public static void ToggleAllowConsume() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			if(info.ConsumeToggle.IsInteractable()) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				selectedIcon.isConsumersHalted = info.ConsumeToggle.isOn;
			}
		}

		public static void ToggleAllowProduction() {
			Toggle t = GuiManager.instance.infoPanel.GetComponent<InfoPanel>().ProduceToggle;
			if(t.IsInteractable()) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				selectedIcon.isProductionHalted = t.isOn;
			}
		}

		public static void ToggleSellStores() {
			Toggle t = GuiManager.instance.infoPanel.GetComponent<InfoPanel>().SellToggle;
			if(t.IsInteractable()) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				selectedIcon.isSellingStores = t.isOn;
			}
		}

		public static void ToggleAutoBuild() {
			Toggle t = GuiManager.instance.infoPanel.GetComponent<InfoPanel>().BuildToggle;
			if(t.IsInteractable()) {
				//Industry item;
				//Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				selectedIcon.doAutobuild = t.isOn;
			}
		}

		internal static void FacilityUnselected() {
			FacilityUnselected(null);
		}

		public static void FacilityUnselected(Industry go) {
			GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
			selectedIcon = null;
			GuiManager.instance.infoPanel.transform.localPosition = new Vector3(-1465, 55, 0);
			lastLevel = -1;
		}

		public static void SynchronizeInustries() {
			doSynchronize = true;
			GuiManager.instance.craftHeader.transform.FindChild("SyncBtn").GetComponent<Button>().interactable = false;
		}

		private static int lastLevel = 0;
		private static int lastNum = 0;

		public static void UpdateIcon() {
			Vector3 newpos;
			if(selectedIcon.craftingGridGO != null && (lastPos - selectedIcon.craftingGridGO.transform.GetChild(0).localPosition).sqrMagnitude > 0) {
				newpos = selectedIcon.craftingGridGO.transform.GetChild(0).localPosition;
				//int maxy = Mathf.RoundToInt((Screen.height-50) * ((RectTransform)selectedIcon.transform.parent).anchorMax.y);
				//int minx = Mathf.RoundToInt(((RectTransform)selectedIcon.transform.parent).offsetMin.x);
				int maxy = Screen.height - 100;
				int minx = Screen.width / 2 - 32;
				newpos.y = Mathf.Clamp(newpos.y, 48, maxy-32);
				newpos.x = Mathf.Clamp(newpos.x, -1*(minx - 128), minx);
				newpos = MathHelper.snap(newpos, 24);
				selectedIcon.craftingGridGO.transform.GetChild(0).localPosition = newpos;
				Main.instance.mouseDownTime += 1;
				lastPos = newpos;
			}
			//Industry item;
			//Main.instance.player.itemData.TryGetValue(item, out selectedIcon);
			Material mat = GuiManager.instance.infoPanel.transform.FindChild("Progress").GetComponent<Image>().material;
			mat.SetFloat("_Cutoff", ((selectedIcon.getTimeRemaining() >= 0 ? selectedIcon.getTimeRemaining() : 10) / 10f));
			mat.SetColor("_Color", selectedIcon.productType.color);
			int ch = ((selectedIcon.output * selectedIcon.level) - selectedIcon.consumeAmount);
			if(selectedIcon.isProductionHalted) ch = -selectedIcon.consumeAmount;
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			if(selectedIcon.isSellingStores && selectedIcon.getRawVendors() > 0) {
				int q = selectedIcon.getVendors() * Main.instance.GetVendorSize();
				if(selectedIcon.isProductionHalted) q = 0;
				//q = Math.Min(q, item.quantityStored);
				if(selectedIcon.quantityStored+ch < q) {
					q = BigInteger.ToInt32(selectedIcon.quantityStored)+ch;
				}
				ch = ch - q;
				if(ch == 0) {
					info.StorageTxt.text = "Change:\n0 / cycle\n" + Main.AsCurrency(selectedIcon.quantityStored);
				}
				else {
					if(ch >= 0) {
						info.StorageTxt.text = "Change:\n" + Main.AsCurrency(ch,6) + " / cycle\n" + Main.AsCurrency(selectedIcon.quantityStored);
					}
					else {
						info.StorageTxt.text = "Change:\n" + Main.AsCurrency(ch, 6) + "(0)" + " / cycle\n" + Main.AsCurrency(selectedIcon.quantityStored);
					}
				}
			}
			else {
				int q = selectedIcon.getVendors() * Main.instance.GetVendorSize();
				if(ch >= 0 && q >= 0) {
					ch = Mathf.Max(ch - q, 0);
				}
				else {
					//ch = ch - q; //Mathf.Max(ch - q, 0);
				}
				info.StorageTxt.text = "Change:\n" + Main.AsCurrency(ch, 6) + " / cycle\n" + Main.AsCurrency(selectedIcon.quantityStored);
			}
			if(!Input.GetMouseButton(0) && Main.instance.mouseDownTime > 0 && Main.instance.mouseDownTime < 0.3f) {
				if(selectedIcon.craftingGridGO != null) {
					Vector3 pos = selectedIcon.craftingGridGO.transform.GetChild(0).localPosition;//new Vector3(selectedIcon.transform.localPosition.x, selectedIcon.transform.localPosition.y + 50, 0);

					List<RaycastResult> results = RaycastMouse();
					bool insideSelected = false;
					foreach(RaycastResult res in results) {
						GameObject go = res.gameObject;
						while(go.transform.parent != null) {
							if(go == selectedIcon.craftingGridGO || go == GuiManager.instance.infoPanel) {
								insideSelected = true;
							}
							go = go.transform.parent.gameObject;
						}
					}
					if(!insideSelected) {
						FacilityUnselected(selectedIcon);
						return;
					}

					//pos.y -= 120;
					int v = (Screen.height) / 2;
					//pos.y += v;
					int h = (Screen.width / 2) - 170;
					pos.x = Mathf.Clamp(pos.x, 115 - h, h);
					pos.y = Mathf.Clamp(pos.y, v-214, v+110) + 0.5f;

					GuiManager.instance.infoPanel.transform.localPosition = pos;
				}
				info.Title.text = Main.ToTitleCase(selectedIcon.name);
				info.PricePer.text = "$" + Main.AsCurrency(selectedIcon.GetSellValue());
				info.SellToggle.interactable = false;
				info.ConsumeToggle.interactable = false;
				info.SellToggle.isOn = selectedIcon.isSellingStores;
				info.ConsumeToggle.isOn = selectedIcon.isConsumersHalted;
				info.ProduceToggle.isOn = selectedIcon.isProductionHalted;
				info.SellToggle.interactable = true;
				info.ConsumeToggle.interactable = true;
				info.VendNum.text = "" + selectedIcon.getRawVendors();
				info.BuildToggle.isOn = selectedIcon.doAutobuild;
				info.BuildNum.text = "" + selectedIcon.autoBuildLevel;
				info.MagnitudeNum.text = "10 E" + selectedIcon.autoBuildMagnitude;
				BigInteger num = 0;
				if(selectedIcon.isSellingStores) {
					BigInteger m = selectedIcon.output * selectedIcon.level + selectedIcon.quantityStored - selectedIcon.consumeAmount;
					if(m > selectedIcon.getVendors() * Main.instance.GetVendorSize()) {
						num = selectedIcon.getVendors() * Main.instance.GetVendorSize();
					}
					else {
						num = m;
					}
				}
				else {
					if(selectedIcon.output * selectedIcon.level < selectedIcon.getVendors() * Main.instance.GetVendorSize())
						num = selectedIcon.output * selectedIcon.level;
					else
						num = selectedIcon.getVendors() * Main.instance.GetVendorSize();
				}
				info.VendText.text = "Sold by Vendors: " + num;
				info.ApprText.text = selectedIcon.apprentices + " Apprentices";
				ShowConsumers();
			}
			BigInteger spendCost = 0;// item.GetScaledCost();
			bool revertBtn = false;
			Profiler.BeginSample("Calc costs");
			if(lastLevel != selectedIcon.level) {
				/*lastLevel = item.level;
				lastCostTen = (BigInteger)item.GetScaledCost(10);
				lastCostFifty = (BigInteger)item.GetScaledCost(50);
				lastCostOne = (BigInteger)item.GetScaledCost();*/
			}
			int n = 0;
			if(isShiftDown) {
				n = 10;
				spendCost = (BigInteger)selectedIcon.GetScaledCost(10);
				/*spendCost = lastCostTen;
				Profiler.BeginSample("Shift");
				BigInteger c = (BigInteger)item.GetScaledCost(10);
				spendCost = c;
				info.Upgrade.text = "+10 ($" + Main.AsCurrency(c,6) + ")";
				if(info.Downgrade.text != "-10") {
					info.Downgrade.text = "-10";
					revertBtn = true;
				}
				Profiler.EndSample();*/
			}
			else if(isCntrlDown) {
				n = 50;
				spendCost = (BigInteger)selectedIcon.GetScaledCost(50);
				/*spendCost = lastCostFifty;
				Profiler.BeginSample("Control");
				BigInteger c = (BigInteger)item.GetScaledCost(50);
				spendCost = c;
				info.Upgrade.text = "+50 ($" + Main.AsCurrency(c, 6) + ")";
				if(info.Downgrade.text != "-50") {
					info.Downgrade.text = "-50";
					revertBtn = true;
				}
				Profiler.EndSample();*/
			}
			else {
				n = 1;
				spendCost = (BigInteger)selectedIcon.GetScaledCost(1);
				/*spendCost = lastCostOne;
				Profiler.BeginSample("One");
				if(info.Downgrade.text != "-1") {
					info.Downgrade.text = "-1";
					revertBtn = true;
				}
				Profiler.EndSample();*/
			}
			Profiler.EndSample();
			if(lastNum != n || lastLevel != selectedIcon.level) {
				//Debug.Log(Main.AsCurrency((BigInteger)item.GetScaledCost(1), 6));
				//Debug.Log(Main.AsCurrency((BigInteger)item.GetScaledCost(), 6));
				//Debug.Log(spendCost);
				info.Upgrade.text = "+" + n + " ($" + Main.AsCurrency(spendCost, 6) + ")";
				info.Downgrade.text = "-" + n;
				lastNum = n;
				lastLevel = selectedIcon.level;
			}
			if(revertBtn) {
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
				info.ConfDowngradeBtn.GetComponentInChildren<Text>().text = "Confirm " + info.Downgrade.text;
			}
			info.SetOutputNum("x" + Main.AsCurrency(selectedIcon.output * selectedIcon.level, 3, true));
			info.Level.text = "Level " + selectedIcon.level;

			foreach(GameObject bt in buildButtons) {
				ItemButtonData dat = bt.GetComponent<ItemButtonData>();
				if(dat.connectedItem == selectedIcon) {
					if(spendCost > Main.instance.player.money) {
						dat.GetComponent<Button>().interactable = false;
					}
					else {
						dat.GetComponent<Button>().interactable = true;
					}
					goto skipRest;
				}
			}
			skipRest:
			if(spendCost > Main.instance.player.money) {
				info.UpgradeBtn.interactable = false;
			}
			else {
				info.UpgradeBtn.interactable = true;
			}
			for(int j = 1; j <= 3; j++) {
				Transform go = GuiManager.instance.infoPanel.transform.FindChild("Input" + j);
				if(selectedIcon.inputs.Count >= j) {
					go.gameObject.SetActive(true);
					IndustryInput input = selectedIcon.inputs[j - 1];
					go.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + input.item.name);

					info.SetInputINum(j, true, "x" + Main.AsCurrency(input.quantity * selectedIcon.level, 3, true));
				}
				else {
					go.gameObject.SetActive(false);
					info.SetInputINum(j, false);
				}
			}
		}

		public static List<RaycastResult> RaycastMouse() {

			PointerEventData pointerData = new PointerEventData(EventSystem.current) {
				pointerId = -1,
			};

			pointerData.position = Input.mousePosition;

			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);

			//Debug.Log(results.Count);

			return results;
		}

		private static Color GetColorForProductivity(Industry item, IndustryInput input) {
			int t = Mathf.RoundToInt(Time.time * 5);
			int quantPerCycle = input.item.output * input.item.level;
			int needPerCycle = input.quantity * item.level;

			quantPerCycle *= input.item.getHalveAndDouble();
			needPerCycle *= item.getHalveAndDouble();

			if((input.item.isConsumersHalted || (input.item.quantityStored < needPerCycle && item.getTimeRemaining()  <= 0)) && t % 4 != 0)
				return ColorHelper.PURPLE;
			//Debug.Log("--> " + needPerCycle + ">" + quantPerCycle);
			if(needPerCycle > quantPerCycle) {
				if(input.item.quantityStored >= needPerCycle) {
					return ColorHelper.ORANGE;
				}
				return Color.red;
			}
			return Color.green;
		}
	}
}
