using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.util;
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
		public static GameObject selectedIcon;
		protected static Text numVendors;

		private static Vector3 lastPos;
		public static void setupUI() {
			numVendors = GuiManager.instance.craftHeader.transform.FindChild("AvailableVendors").GetComponent<Text>();
			numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
			buildButtons = new List<GameObject>();
			int i = 0;
			FieldInfo[] fields = typeof(Industries).GetFields();
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

				item.listObj = it;
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

		public static void Update() {
			isShiftDown = Input.GetButton("Shift");
			isCntrlDown = Input.GetButton("Control");
			if(GuiManager.instance.craftArea.activeSelf) {
				foreach(GameObject bt in buildButtons) {
					ItemButtonData dat = bt.GetComponent<ItemButtonData>();
					BigInteger c = dat.connectedItem.GetScaledCost();
					bt.transform.FindChild("Cost").GetComponent<Text>().text = "$" + Main.AsCurrency(c);
					if(c > Main.instance.player.money) {
						dat.GetComponent<Button>().interactable = false;
					}
					else {
						dat.GetComponent<Button>().interactable = true;
					}
				}
				if(Input.GetButtonDown("Cancel")) {
					FacilityUnselected(selectedIcon);
				}
				if(selectedIcon != null) {
					UpdateIcon();
				}
				foreach(Industry item in Main.instance.player.builtItems) {
					foreach(IndustryInput input in item.inputs) {
						GameObject obj = input.arrow; //Instantiate(PrefabManager.instance.GRID_GUI_ARROW);

						obj.transform.SetParent(item.guiObj.transform.parent.GetChild(0));

						obj.transform.localPosition = item.guiObj.transform.GetChild(0).localPosition;

						if(input.item.guiObj != null) {
							obj.SetActive(true);
							float dx = input.item.guiObj.transform.GetChild(0).localPosition.x - item.guiObj.transform.GetChild(0).localPosition.x;
							float dy = input.item.guiObj.transform.GetChild(0).localPosition.y - item.guiObj.transform.GetChild(0).localPosition.y;

							float dist = Mathf.Sqrt(dx * dx + dy * dy);
							((RectTransform)obj.transform.GetChild(0)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dist);
							//input.arrow = obj;

							obj.transform.GetChild(0).localPosition = new Vector3((dist / -2) - 32, 0);

							obj.transform.localEulerAngles = new Vector3(0, 0, 180 + MathHelper.RadiansToDegrees(Mathf.Atan2(dy, dx)));

							obj.transform.GetChild(0).GetComponent<Image>().color = GetColorForProductivity(item, input);
						}
						else {
							obj.SetActive(false);
						}
					}

					Transform t;
					t = item.guiObj.transform.GetChild(0).GetChild(0).FindChild("Ico1");
					t.gameObject.SetActive(QuestManager.IsIndustryOnQuest(item));
					t = item.guiObj.transform.GetChild(0).GetChild(0).FindChild("Ico2");
					t.gameObject.SetActive(item.getRawVendors() > 0);
					t = item.guiObj.transform.GetChild(0).GetChild(0).FindChild("Ico3");
					t.gameObject.SetActive(item.apprentices > 0);

				}
				GuiManager.instance.currentMoney.GetComponent<Text>().text = "$" + Main.AsCurrency(Main.instance.player.money, 12);
			}
		}
		public static void BuildIndustry(Industry item) {
			BuildIndustry(item, false);
		}

		public static void BuildIndustry(Industry item, bool fromSave) {
			//FacilityUnselected(selectedIcon);
			if(fromSave || Main.instance.player.money >= item.GetScaledCost()) {
				if(!fromSave) {
					Main.instance.player.money -= item.GetScaledCost();
					item.level++;
				}
				if(!Main.instance.player.builtItems.Contains(item)) {
					Main.instance.player.builtItems.Add(item);

					Main.instance.player.builtItems.Sort((a, b) => a.cost.CompareTo(b.cost));
					if(!fromSave)
						item.setTimeRemaining(float.MinValue);
					GameObject it = Main.Instantiate(PrefabManager.instance.BUILDING_GUI_GRIDITEM);
					it.name = "Grid_Icon_" + item.name;
					it.transform.SetParent(GuiManager.instance.gridArea.transform);
					it.transform.localPosition = new Vector3(0, 0, 0);
					int y = (Screen.height - 128) / 2;
					y = MathHelper.snap(y, 24);
					it.transform.GetChild(0).localPosition = new Vector3(0, y, 0);

					if(fromSave) {
						it.transform.GetChild(0).localPosition = item.getGridPos();
					}

					it.transform.GetChild(0).GetChild(0).FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.name);
					item.guiObj = it;
					Image img = it.transform.GetChild(0).GetChild(0).FindChild("Progress").GetComponent<Image>();
					img.material = Main.Instantiate(img.material);
					SelectObject selObj = it.GetComponentInChildren<SelectObject>();
					selObj.selectListener(delegate { FacilitySelected(item.guiObj); });
					selObj.deselectListener(delegate { FacilityUnselected(item.guiObj); });
					Main.instance.player.itemData.Add(it, item);
					/*Industry item2;
					Main.instance.player.itemData.TryGetValue(it, out item2);
					Debug.Log(item.name + " -> " + item2);*/
					foreach(IndustryInput input in item.inputs) {
						GameObject obj = Main.Instantiate(PrefabManager.instance.GRID_GUI_ARROW);
						obj.transform.SetParent(it.transform.parent.GetChild(0));
						obj.transform.localPosition = it.transform.GetChild(0).localPosition;
						input.arrow = obj;
						if(input.item.guiObj != null) {
							obj.SetActive(true);
							float dx = input.item.guiObj.transform.GetChild(0).localPosition.x - it.transform.GetChild(0).localPosition.x;
							float dy = input.item.guiObj.transform.GetChild(0).localPosition.y - it.transform.GetChild(0).localPosition.y;

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


		public static void FacilitySelected(GameObject go) {
			GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
			selectedIcon = go;
			lastPos = selectedIcon.transform.GetChild(0).localPosition;
			GuiManager.instance.infoPanel.transform.localPosition = new Vector3(-1465, 55, 0);
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			GuiManager.instance.infoPanel.transform.FindChild("Output").gameObject.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + item.name);
		}

		public static void IncreaseVendors() {
			if(selectedIcon != null) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				IncreaseVendors(item);
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().VendNum.text = "" + item.getRawVendors();
			}
			numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
		}
		public static void IncreaseVendors(Industry item) {
			if(Main.instance.player.currentVendors < Main.instance.player.maxVendors) {
				int num = (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
				int maxAdd = Main.instance.player.maxVendors - Main.instance.player.currentVendors;
				num = Math.Min(num, maxAdd);
				item.AdjustVendors(item.getRawVendors() + num);
				Main.instance.player.currentVendors = Math.Min(Main.instance.player.currentVendors + num, Main.instance.player.maxVendors);
			}
		}

		public static void DecreaseVendors() {
			if(selectedIcon != null) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				DecreaseVendors(item);
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().VendNum.text = "" + item.getRawVendors();
			}
			numVendors.text = Main.instance.player.currentVendors + " of " + Main.instance.player.maxVendors + " vendors in use";
		}
		public static void DecreaseVendors(Industry item) {
			if(item.getRawVendors() > 0) {
				int num = (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
				num = Math.Min(num, item.getRawVendors());
				item.AdjustVendors(item.getRawVendors() - num);
				Main.instance.player.currentVendors -= num;
			}
		}

		public static void IncreaseApprentices() {
			if(selectedIcon != null) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				IncreaseApprentices(item);
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
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				DecreaseApprentices(item);
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
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				item.autoBuildLevel += (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
			}
		}

		public static void DecreaseAutoBuild() {
			if(selectedIcon != null) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				if(item.autoBuildLevel > 0) {
					item.autoBuildLevel = Math.Max(item.autoBuildLevel - (isCntrlDown ? 50 : (isShiftDown ? 10 : 1)), 0);
				}
			}
		}

		public static void IncreaseBuildMagnitude() {
			if(selectedIcon != null) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				item.autoBuildMagnitude += (isCntrlDown ? 50 : (isShiftDown ? 10 : 1));
			}
		}

		public static void DecreaseBuildMagnitude() {
			if(selectedIcon != null) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				if(item.autoBuildMagnitude > 0) {
					item.autoBuildMagnitude = Math.Max(item.autoBuildMagnitude - (isCntrlDown ? 50 : (isShiftDown ? 10 : 1)), 0);
				}
			}
		}

		public static void AdvanceTimer() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			item.addTimeRaw(Main.instance.GetClickRate());
			//item.timeRemaining -= Main.instance.GetClickRate();
		}

		public static void SellAll() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			Main.instance.player.AddMoney(item.GetSellValue() * item.quantityStored);
			//Main.instance.player.money += item.GetSellValue() * item.quantityStored;
			item.quantityStored = 0;
		}

		public static BigInteger SellAllValue() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return (item.GetSellValue() * item.quantityStored);
		}

		public static BigInteger GetQuantity() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return item.quantityStored;
		}

		public static string GetName() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return item.name;
		}
		public static BigInteger ValueSoldByVendors() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return ValueSoldByVendors(item);
		}
		public static BigInteger NumberSoldByVendors() {
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			return NumberSoldByVendors(item);
		}

		public static BigInteger ValueSoldByVendors(Industry item) {
			BigInteger num = NumberSoldByVendors(item);
			num *= item.GetSellValue();
			num = Main.instance.GetVendorValue() * num;
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
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			BigInteger spendCost = item.GetScaledCost();
			int buyNum = 1;
			if(isShiftDown) {
				buyNum = 10;
				BigInteger c = item.GetScaledCost(buyNum);
				spendCost = c;
			}
			else if(isCntrlDown) {
				buyNum = 50;
				BigInteger c = item.GetScaledCost(buyNum);
				spendCost = c;
			}
			if(Main.instance.player.money >= spendCost) {
				Main.instance.player.money -= spendCost;
				item.level+=buyNum;
			}
			GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
		}

		public static void DowngradeCurrent() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			info.DowngradeBtn.gameObject.SetActive(false);
		}

		public static void Do_DowngradeCurrent() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			//BigInteger spendCost = item.GetScaledCost();
			int buyNum = 1;
			if(isShiftDown) {
				buyNum = 10;
				//BigInteger c = item.GetScaledCost(buyNum);
				//spendCost = c;
			}
			else if(isCntrlDown) {
				buyNum = 50;
				//BigInteger c = item.GetScaledCost(buyNum);
				//spendCost = c;
			}
			//if(Main.instance.player.money >= spendCost) {
				//Main.instance.player.money -= spendCost;
				item.level -= buyNum;
				info.DowngradeBtn.gameObject.SetActive(true);
			//}
		}

		public static void ToggleAllowConsume() {
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			if(info.ConsumeToggle.IsInteractable()) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				item.isConsumersHalted = info.ConsumeToggle.isOn;
			}
		}

		public static void ToggleAllowProduction() {
			Toggle t = GuiManager.instance.infoPanel.GetComponent<InfoPanel>().ProduceToggle;
			if(t.IsInteractable()) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				item.isProductionHalted = t.isOn;
			}
		}

		public static void ToggleSellStores() {
			Toggle t = GuiManager.instance.infoPanel.GetComponent<InfoPanel>().SellToggle;
			if(t.IsInteractable()) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				item.isSellingStores = t.isOn;
			}
		}

		public static void ToggleAutoBuild() {
			Toggle t = GuiManager.instance.infoPanel.GetComponent<InfoPanel>().BuildToggle;
			if(t.IsInteractable()) {
				Industry item;
				Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
				item.doAutobuild = t.isOn;
			}
		}

		internal static void FacilityUnselected() {
			FacilityUnselected(null);
		}

		public static void FacilityUnselected(GameObject go) {
			GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
			selectedIcon = null;
			GuiManager.instance.infoPanel.transform.localPosition = new Vector3(-1465, 55, 0);
		}

		public static void UpdateIcon() {
			//Vector3 offset;
			Vector3 newpos;
			if((lastPos - selectedIcon.transform.GetChild(0).localPosition).sqrMagnitude > 0) {
				newpos = selectedIcon.transform.GetChild(0).localPosition;
				//int maxy = Mathf.RoundToInt((Screen.height-50) * ((RectTransform)selectedIcon.transform.parent).anchorMax.y);
				//int minx = Mathf.RoundToInt(((RectTransform)selectedIcon.transform.parent).offsetMin.x);
				int maxy = Screen.height - 100;
				int minx = Screen.width / 2 - 32;
				newpos.y = Mathf.Clamp(newpos.y, 32, maxy-32);
				newpos.x = Mathf.Clamp(newpos.x, -1*(minx-128), minx);
				newpos = MathHelper.snap(newpos, 24);
				selectedIcon.transform.GetChild(0).localPosition = newpos;
				Main.instance.mouseDownTime += 1;
				lastPos = newpos;
			}
			Industry item;
			Main.instance.player.itemData.TryGetValue(selectedIcon, out item);
			Material mat = GuiManager.instance.infoPanel.transform.FindChild("Progress").GetComponent<Image>().material;
			mat.SetFloat("_Cutoff", ((item.getTimeRemaining() >= 0 ? item.getTimeRemaining() : 10) / 10f));
			mat.SetColor("_Color", item.productType.color);
			int ch = ((item.output * item.level) - item.consumeAmount);
			if(item.isProductionHalted) ch = -item.consumeAmount;
			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			if(item.isSellingStores && item.getRawVendors() > 0) {
				int q = item.getVendors() * Main.instance.GetVendorSize();
				if(item.isProductionHalted) q = 0;
				//q = Math.Min(q, item.quantityStored);
				if(item.quantityStored+ch < q) {
					q = BigInteger.ToInt32(item.quantityStored)+ch;
				}
				ch = ch - q;
				if(ch == 0) {
					info.StorageTxt.text = "Change:\n0 / cycle\n" + Main.AsCurrency(item.quantityStored);
				}
				else {
					if(ch >= 0) {
						info.StorageTxt.text = "Change:\n" + ch + " / cycle\n" + Main.AsCurrency(item.quantityStored);
					}
					else {
						info.StorageTxt.text = "Change:\n" + ch + "(0)" + " / cycle\n" + Main.AsCurrency(item.quantityStored);
					}
				}
			}
			else {
				int q = item.getVendors() * Main.instance.GetVendorSize();
				if(ch >= 0 && q >= 0) {
					ch = Mathf.Max(ch - q, 0);
				}
				else {
					//ch = ch - q; //Mathf.Max(ch - q, 0);
				}
				info.StorageTxt.text = "Change:\n" + ch + " / cycle\n" + Main.AsCurrency(item.quantityStored);
			}

			if(!Input.GetMouseButton(0) && Main.instance.mouseDownTime > 0 && Main.instance.mouseDownTime < 0.3f) {
				Vector3 pos = selectedIcon.transform.GetChild(0).localPosition;//new Vector3(selectedIcon.transform.localPosition.x, selectedIcon.transform.localPosition.y + 50, 0);

				List<RaycastResult> results = RaycastMouse();
				bool insideSelected = false;
				foreach(RaycastResult res in results) {
					GameObject go = res.gameObject;
					while(go.transform.parent != null) {
						if(go == selectedIcon || go == GuiManager.instance.infoPanel) {
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
				pos.y -= v;
				int h = (Screen.width / 2) - 170;
				pos.x = Mathf.Clamp(pos.x, 115 - h, h);
				pos.y = Mathf.Clamp(pos.y, -v+170, v-270) + 0.5f;
				
				GuiManager.instance.infoPanel.transform.localPosition = pos;
				info.Title.text = Main.ToTitleCase(item.name);
				info.PricePer.text = "$" + Main.AsCurrency(item.GetSellValue());
				info.SellToggle.interactable = false;
				info.ConsumeToggle.interactable = false;
				info.SellToggle.isOn = item.isSellingStores;
				info.ConsumeToggle.isOn = item.isConsumersHalted;
				info.ProduceToggle.isOn = item.isProductionHalted;
				info.SellToggle.interactable = true;
				info.ConsumeToggle.interactable = true;
				info.VendNum.text = "" + item.getRawVendors();
				info.BuildToggle.isOn = item.doAutobuild;
				info.BuildNum.text = "" + item.autoBuildLevel;
				info.MagnitudeNum.text = "10 E" + item.autoBuildMagnitude;
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
				info.VendText.text = "Sold by Vendors: " + num;
				info.ApprText.text = item.apprentices + " Apprentices";
			}
			BigInteger spendCost = item.GetScaledCost();
			bool revertBtn = false;
			if(isShiftDown) {
				BigInteger c = item.GetScaledCost(10);
				spendCost = c;
				info.Upgrade.text = "+10 ($" + Main.AsCurrency(c) + ")";
				if(info.Downgrade.text != "-10") {
					info.Downgrade.text = "-10";
					revertBtn = true;
				}
			}
			else if(isCntrlDown) {
				BigInteger c = item.GetScaledCost(50);
				spendCost = c;
				info.Upgrade.text = "+50 ($" + Main.AsCurrency(c) + ")";
				if(info.Downgrade.text != "-50") {
					info.Downgrade.text = "-50";
					revertBtn = true;
				}
			}
			else {
				info.Upgrade.text = "+1 ($" + Main.AsCurrency(item.GetScaledCost()) + ")";
				if(info.Downgrade.text != "-1") {
					info.Downgrade.text = "-1";
					revertBtn = true;
				}
			}
			if(revertBtn) {
				GuiManager.instance.infoPanel.GetComponent<InfoPanel>().DowngradeBtn.gameObject.SetActive(true);
				info.ConfDowngradeBtn.GetComponentInChildren<Text>().text = "Confirm " + info.Downgrade.text;
			}
			info.SetOutputNum("x" + Main.AsCurrency(item.output * item.level));
			info.Level.text = "Level " + item.level;
			if(spendCost > Main.instance.player.money) {
				info.UpgradeBtn.interactable = false;
			}
			else {
				info.UpgradeBtn.interactable = true;
			}
			for(int j = 1; j <= 3; j++) {
				Transform go = GuiManager.instance.infoPanel.transform.FindChild("Input" + j);
				if(item.inputs.Count >= j) {
					go.gameObject.SetActive(true);
					IndustryInput input = item.inputs[j - 1];
					go.GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + input.item.name);

					info.SetInputINum(j, true, "x" + Main.AsCurrency(input.quantity * item.level));
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
