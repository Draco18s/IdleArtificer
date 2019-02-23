using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Koopakiller.Numerics;
using Assets.draco18s.config;

namespace Assets.draco18s.artificer.game {
	public class EnchantingManager {
		private static Transform inventoryList1;
		private static Transform inventoryList2;
		private static Transform inventoryList3;
		//private static Industry selectedIndustry;
		//private static ItemStack selectedStack;
		private static ItemStack inputStack1;
		private static ItemStack inputStack2;
		private static ItemStack outputStack;
		private static Image inputImg1;
		private static Image inputImg2;
		private static Image outputImg;
		private static Text outputName;

		private static Industry preSelectedIndustry;
		private static ItemStack preSelectedStack;
		private static Dictionary<Industry, GameObject> enchantInvenList = new Dictionary<Industry, GameObject>();
		private static Dictionary<ItemStack, GameObject> enchantMiscInvenList = new Dictionary<ItemStack, GameObject>();
		private static Dictionary<Industry, GameObject> enchantIngredList = new Dictionary<Industry, GameObject>();

		public static void OneTimeSetup() {
			Transform enchantWindow = GuiManager.instance.enchantArea.transform.Find("Enchant");
			Transform i1 = enchantWindow.Find("Input1");
			Transform i2 = enchantWindow.Find("Input2");
			Transform o1 = enchantWindow.Find("Output");
			//i1.GetComponent<Button>().onClick.AddListener(delegate { addInput1(); });
			//i2.GetComponent<Button>().onClick.AddListener(delegate { addInput2(); });
			o1.GetComponent<Button>().onClick.AddListener(delegate { getOutput(); });
			inputImg1 = i1.GetComponent<Image>();
			inputImg2 = i2.GetComponent<Image>();
			outputImg = o1.GetComponent<Image>();
			inventoryList1 = GuiManager.instance.enchantArea.transform.Find("Inventory1").GetChild(0).GetChild(0);
			inventoryList2 = GuiManager.instance.enchantArea.transform.Find("Inventory2").GetChild(0).GetChild(0);
			inventoryList3 = GuiManager.instance.enchantArea.transform.Find("Inventory3").GetChild(0).GetChild(0);
			inventoryList1.transform.hierarchyCapacity = 100 * 10;
			inventoryList2.transform.hierarchyCapacity = 100 * 10;
			inventoryList3.transform.hierarchyCapacity = 100 * 10;
#pragma warning disable 0219
			Enchantment e = Enchantments.ALERTNESS;
#pragma warning restore 0219
			outputName = enchantWindow.Find("OutputName").GetComponent<Text>();
			outputName.text = "";
		}

		public static void setupUI() {
			inputImg1.sprite = GuiManager.instance.gray_square;
			inputImg2.sprite = GuiManager.instance.gray_square;
			outputImg.sprite = GuiManager.instance.gray_square;
			//selectedIndustry = null;
			//selectedStack = null;
			inputStack1 = null;
			inputStack2 = null;
			outputStack = null;
			doOutput();
			int i = 0;

			/*for(i = 0; i < inventoryList1.childCount; i++) {
				Main.Destroy(inventoryList1.GetChild(i).gameObject);
			}
			for(i = 0; i < inventoryList3.childCount; i++) {
				Main.Destroy(inventoryList3.GetChild(i).gameObject);
			}
			for(i = 0; i < inventoryList3.childCount; i++) {
				Main.Destroy(inventoryList3.GetChild(i).gameObject);
			}
			i = 0;*/
			List<ItemStack> toRemove = new List<ItemStack>();
			foreach(ItemStack st in enchantMiscInvenList.Keys) {
				GameObject go;
				enchantMiscInvenList.TryGetValue(st, out go);
				if(Main.instance.player.miscInventory.Contains(st)) {
					if(go != null) {
						Main.Destroy(go);
					}
					toRemove.Add(st);
				}
			}
			foreach(ItemStack st in toRemove) {
				enchantMiscInvenList.Remove(st);
			}

			foreach(Industry ind in Main.instance.player.builtItems) {
				Industry newInd = ind;
				if(!ind.industryItem.isConsumable && ind.industryItem.canBeGivenToQuests) {
					GameObject go;
					enchantInvenList.TryGetValue(ind, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM, inventoryList1) as GameObject;
						//go.transform.SetParent(inventoryList1);
						enchantInvenList.Add(newInd, go);
					}
					go.transform.localPosition = new Vector3(6, (i * -125) - 5, 0);
					//ind.enchantInvenListObj = go;
					go.name = ind.saveName;
					go.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(Localization.translateToLocal(ind.unlocalizedName));
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + Main.AsCurrency(10000);
					go.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + ind.saveName);
					int req_num = 1;
					long ty = (long)ind.industryItem.getAllReqs();
					//if(ind == Industries.LEATHER) {
					//Debug.Log("All types:" + ind.industryItem.getAllReqs());
					//}
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
							//if(ind == Industries.LEATHER) {
								//Debug.Log("   :" + req_num);
								//Debug.Log("   :" + ((RequirementType)(1L << req_num)));
							//}
							go.transform.Find("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
							ty = ty >> 1;
							ty = ty << 1;
						}
					}
					go.GetComponent<Button>().onClick.AddListener(delegate { /*SelectItem(newInd);*/addInput1(newInd); });
					i++;
				}
			}
			((RectTransform)inventoryList1).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);
			inventoryList1.transform.localPosition = Vector3.zero;
			i = 0;
			int j = 0;

			//TODO: if item is an enchanted thingy, needs to be in the other list, or something.
			//It definitely needs to be placable in the *other* input slot
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				ItemStack s = stack;
				GameObject go;
				enchantMiscInvenList.TryGetValue(stack, out go);
				Enchantment ench = GameRegistry.GetEnchantmentByItem(stack.item);
				if(go == null) {
					if(ench != null) {
						go = Main.Instantiate(PrefabManager.instance.INGRED_GUI_LISTITEM, inventoryList2) as GameObject;
						//go.transform.SetParent(inventoryList2);
						enchantMiscInvenList.Add(s, go);
					}
					else {
						if(s.relicData != null && !s.isIDedByPlayer) continue;
						//Debug.Log(s.getDisplayName() + ":" + (s.relicData == null ? 0 : s.relicData.Count));
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM, inventoryList3) as GameObject;
						//go.transform.SetParent(inventoryList2);
						enchantMiscInvenList.Add(s, go);
					}
				}
				//UnityEngine.Debug.Log(stack.getDisplayName() + ": " +new Vector3(6, (j * -125) - 5, 0));
				go.transform.localPosition = new Vector3(6, (j * -125) - 5, 0);
				//ind.invenListObj = go;
				go.name = stack.item.name;
				go.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(stack.getDisplayName());
				go.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);

				int req_num = 1;
				long ty = (long)stack.getAllReqs();
				bool abort = false;
				if(ench != null) {
					ty |= (long)ench.reqTypes;
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize) + " / " + ench.ingredientQuantity;
					go.GetComponent<Button>().onClick.AddListener(delegate { addInput2(s); });
					int slot = (int)ench.enchantSlotRestriction;

					for(int r = 1; r <= 5; r++) {
						if(slot == 0) abort = true;
						while((slot & 1) == 0 && slot > 0) {
							req_num++;
							slot = slot >> 1;
							if(slot == 0) abort = true;
						}
						if(abort) {
							go.transform.Find("Equip" + r).gameObject.SetActive(false);
						}
						else {
							go.transform.Find("Equip" + r).GetComponent<Image>().sprite = GuiManager.instance.equip_icons[req_num - 1];
							slot = slot >> 1;
							slot = slot << 1;
						}
					}
				}
				else {
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize);
					if((stack.enchants.Count > 0 || stack.relicData != null) && stack.isIDedByPlayer) {
						//go.transform.SetParent(inventoryList3);
						go.transform.localPosition = new Vector3(6, (i * -125) - 5, 0);
						go.GetComponent<Button>().onClick.AddListener(delegate { /*SelectItem(s);*/addInput1(s); });
						i++;
						j--;
					}
					else {
						j--;
						enchantMiscInvenList.Remove(stack);
						Main.Destroy(go);
					}
				}
				//Debug.Log(stack.getDisplayName() + ":" + (quests.requirement.RequirementType)ty);
				abort = false;
				req_num = 1;
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

				j++;

			}
			FieldInfo[] allEnchants = typeof(Enchantments).GetFields();
			foreach(FieldInfo enchField in allEnchants) {
				Enchantment enchant = (Enchantment)enchField.GetValue(null);
				if(enchant.ingredient.industry != null && enchant.ingredient.industry.level > 0) {
					int v = (enchant.ingredient.industry.quantityStored > int.MaxValue ? 999999999 : BigInteger.ToInt32(enchant.ingredient.industry.quantityStored));
					ItemStack stack = new ItemStack(enchant.ingredient.industry, v);

					GameObject go;
					enchantIngredList.TryGetValue(enchant.ingredient.industry, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INGRED_GUI_LISTITEM, inventoryList2) as GameObject;
						//go.transform.SetParent(inventoryList2);
						enchantIngredList.Add(enchant.ingredient.industry, go);
					}
					go.transform.localPosition = new Vector3(6, (j * -125) - 5, 0);
					//ind.invenListObj = go;
					go.name = stack.item.name;
					go.transform.Find("Title").GetComponent<Text>().text = Main.ToTitleCase(stack.getDisplayName());
					go.transform.Find("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
					Enchantment ench = GameRegistry.GetEnchantmentByItem(stack.item);
					int req_num = 1;
					long ty = (long)ench.reqTypes;
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize) + " / " + ench.ingredientQuantity;

					go.GetComponent<Button>().onClick.AddListener(delegate { /*SelectItem(s);*/addInput2(stack); });

					//Debug.Log(stack.getDisplayName() + ":" + (quests.requirement.RequirementType)ty);
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

					int slot = (int)ench.enchantSlotRestriction;
					abort = false;
					req_num = 1;
					for(int r = 1; r <= 5; r++) {
						if(slot == 0) abort = true;
						while((slot & 1) == 0 && slot > 0) {
							req_num++;
							slot = slot >> 1;
							if(slot == 0) abort = true;
						}
						if(abort) {
							go.transform.Find("Equip" + r).gameObject.SetActive(false);
						}
						else {
							go.transform.Find("Equip" + r).GetComponent<Image>().sprite = GuiManager.instance.equip_icons[req_num - 1];
							slot = slot >> 1;
							slot = slot << 1;
						}
					}

					j++;
				}
			}

			((RectTransform)inventoryList2).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, j * 126 + 5);
			inventoryList2.transform.localPosition = Vector3.zero;
			((RectTransform)inventoryList3).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);
			inventoryList1.transform.localPosition = Vector3.zero;
			
			sortIngredientList();
		}

		public static void update() {
			foreach(Industry ind in Main.instance.player.builtItems) {
				GameObject go;
				enchantInvenList.TryGetValue(ind, out go);
				if(go != null) {
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + Main.AsCurrency(10000);
				}
				else {
					enchantIngredList.TryGetValue(ind, out go);
					if(go != null) {
						//int v = (ind.quantityStored > int.MaxValue ? 999999999 : BigInteger.ToInt32(ind.quantityStored));
						//ItemStack stack = new ItemStack(ind, v);
						Enchantment e = GameRegistry.GetEnchantmentByItem(ind.industryItem);
						go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + Main.AsCurrency(e.ingredientQuantity);
					}
				}
			}
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				GameObject go;
				enchantMiscInvenList.TryGetValue(stack, out go);
				Enchantment e = GameRegistry.GetEnchantmentByItem(stack.item);
				if(go != null && e != null) {
					go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize) + " / " + Main.AsCurrency(e.ingredientQuantity);
				}
			}
		}

		private static void sortRelicList() {
			List<GameObjectPair> relics = new List<GameObjectPair>();
			for(var i = 0; i < inventoryList3.childCount; i++) {
				Transform t = inventoryList3.GetChild(i);
				KeyValuePair<ItemStack, GameObject> stack;
				stack = enchantMiscInvenList.FirstOrDefault(x => x.Value == t.gameObject);
				relics.Add(new GameObjectPair(t, null, stack.Key));
			}
			relics.Sort((x, y) => {
				int valx = x.stack.relicData.Count - x.stack.enchants.Count;
				int valy = y.stack.relicData.Count - y.stack.enchants.Count;

				return valy.CompareTo(valx);
			});
		}

		private static void sortIngredientList() {
			List<GameObjectPair> ingreds = new List<GameObjectPair>();
			for(var i = 0; i < inventoryList2.childCount; i++) {
				Transform t = inventoryList2.GetChild(i);
				KeyValuePair<ItemStack, GameObject> stack;
				stack = enchantMiscInvenList.FirstOrDefault(x => x.Value == t.gameObject);
				KeyValuePair<Industry, GameObject> ind;
				ind = enchantIngredList.FirstOrDefault(x => x.Value == t.gameObject);
				ingreds.Add(new GameObjectPair(t, ind.Key, stack.Key));
			}
			ingreds.Sort((x, y) => {
				var v = y.val.CompareTo(x.val);
				return v != 0 ? v : x.name.CompareTo(y.name);
			});
			for(var i = 0; i < ingreds.Count; i++) {
				ingreds[i].t.localPosition = new Vector3(6, (i * -125) - 5, 0);
			}
			FilterEnchantIngredients(-1);
			FilterEnchantables(-1);
		}

		/*private static void SelectItem(ItemStack stack) {
			selectedStack = stack;
		}

		private static void SelectItem(Industry ind) {
			selectedIndustry = ind;
		}*/
		public static void addInput1(Industry selectedIndustry) {
			if(selectedIndustry != null) {
				inputImg1.sprite = SpriteLoader.getSpriteForResource("items/" + selectedIndustry.industryItem.name);
				inputStack1 = new ItemStack(selectedIndustry, 10000);
				GameObject go;
				enchantInvenList.TryGetValue(selectedIndustry, out go);
				go.transform.Find("Quantity").GetComponent<Text>().text = Main.AsCurrency(selectedIndustry.quantityStored) + " / " + Main.AsCurrency(10000);
				preSelectedIndustry = selectedIndustry;
				//inputStack1 = null;
			}
			else {
				inputImg1.sprite = GuiManager.instance.gray_square;
				inputStack1 = null;
				preSelectedIndustry = null;
			}
			doOutput();
		}

		public static bool hasItem1() {
			return preSelectedIndustry != null || inputStack1 != null;
		}

		public static bool hasItem2() {
			return preSelectedStack != null;
		}

		public static void addInput1(ItemStack selectedStack) {
			if(selectedStack != null) {
				inputImg1.sprite = SpriteLoader.getSpriteForResource("items/" + selectedStack.item.name);
				inputStack1 = selectedStack;// new ItemStack(selectedStack.item, GameRegistry.GetEnchantmentByItem(selectedStack.item).ingredientQuantity);
				//preSelectedStack = selectedStack;
				//selectedStack = null;
				preSelectedIndustry = null;
			}
			else {
				inputImg1.sprite = GuiManager.instance.gray_square;
				inputStack1 = null;
				preSelectedIndustry = null;
			}
			doOutput();
		}

		public static void addInput2(ItemStack selectedStack) {
			if(selectedStack != null) {
				inputImg2.sprite = SpriteLoader.getSpriteForResource("items/" + selectedStack.item.name);
				inputStack2 = new ItemStack(selectedStack.item, GameRegistry.GetEnchantmentByItem(selectedStack.item).ingredientQuantity);
				preSelectedStack = selectedStack;
				selectedStack = null;
			}
			else {
				inputImg2.sprite = GuiManager.instance.gray_square;
				inputStack2 = null;
				preSelectedStack = null;
			}
			doOutput();
		}

		private static void doOutput() {
			if(inputStack1 != null && inputStack2 != null) {
				Enchantment ench = GameRegistry.GetEnchantmentByItem(preSelectedStack.item);
				//Debug.Log(inputStack1.item.name + "[" + inputStack1.enchants.Count + "], " + inputStack2.item.name + " → " + (ench!=null? ench.name:"null"));
				if(inputStack1.enchants.Count > 0) {
					bool maxed = false;
					if(inputStack1.enchants.Contains(ench)) {
						int count = 0;
						foreach(Enchantment ench1 in inputStack1.enchants) {
							if(ench1 == ench) {
								count++;
							}
						}
						maxed = count >= ench.maxConcurrent;
					}
					if((maxed || inputStack1.relicData == null || inputStack1.relicData.Count < inputStack1.enchants.Count) || !((inputStack1.item.equipType & ench.enchantSlotRestriction) > 0)) {
						outputImg.sprite = GuiManager.instance.gray_square;
						outputImg.color = Color.white;
						outputStack = null;
						outputName.text = "";
					}
					else if(inputStack2.stackSize <= preSelectedStack.stackSize) {
						outputStack = inputStack1.clone();
						outputStack.stackSize = 1;
						outputStack.applyEnchantment(ench);
						outputName.text = outputStack.getDisplayName();
						if(outputStack.relicData != null) outputName.text = ench.name + " " + outputName.text;
						outputImg.sprite = inputImg1.sprite;
					}
					else {
						outputStack = inputStack1.clone();
						outputStack.stackSize = 1;
						outputStack.applyEnchantment(ench);
						outputName.text = outputStack.getDisplayName();
						if(outputStack.relicData != null) outputName.text = ench.name + " " + outputName.text;
						outputImg.sprite = inputImg1.sprite;
						outputImg.color = Color.red;
					}
				}
				else if((inputStack1.item.equipType & ench.enchantSlotRestriction) > 0) {
					outputStack = inputStack1.clone();
					outputStack.stackSize = 1;
					outputStack.applyEnchantment(ench);
					outputName.text = outputStack.getDisplayName();
					if(outputStack.relicData != null) outputName.text = ench.name + " " + outputName.text;
					outputImg.sprite = inputImg1.sprite;
					if(preSelectedIndustry != null) {
						//Debug.Log(inputStack1.stackSize + " <= " + preSelectedIndustry.quantityStored);
						//Debug.Log(inputStack2.stackSize + " <= " + preSelectedStack.stackSize);
						if(inputStack1.stackSize <= preSelectedIndustry.quantityStored && inputStack2.stackSize <= preSelectedStack.stackSize) {
							//Debug.Log(inputStack1.stackSize +" >= "+ preSelectedIndustry.quantityStored + " && " + inputStack2.stackSize + " >= " + preSelectedStack.stackSize);
							outputImg.color = Color.white;
						}
						else {
							outputImg.color = Color.red;
						}
					}

					if(inputStack2.stackSize <= preSelectedStack.stackSize) {
						outputImg.color = Color.white;
					}
					else {
						outputImg.color = Color.red;
					}
				}
				else {
					outputImg.sprite = GuiManager.instance.gray_square;
					outputImg.color = Color.white;
					outputStack = null;
					outputName.text = "";
				}
			}
			else {
				
				outputImg.sprite = GuiManager.instance.gray_square;
				outputImg.color = Color.white;
				outputStack = null;
				outputName.text = "";
			}
			if(inputStack1 != null) {
				FilterEnchantIngredients((int)inputStack1.item.equipType);
			}
			else if(inputStack2 != null) {
				Enchantment ench = GameRegistry.GetEnchantmentByItem(preSelectedStack.item);
				FilterEnchantables((int)ench.enchantSlotRestriction);
			}
			else {
				FilterEnchantIngredients(-1);
				FilterEnchantables(-1);
			}
		}

		private static void FilterEnchantables(int filter) {

		}

		private static void FilterEnchantIngredients(int filter) {
			GameObject go;
			List<GameObjectPair> ingreds = new List<GameObjectPair>();
			for(var i = 0; i < inventoryList2.childCount; i++) {
				go = inventoryList2.GetChild(i).gameObject;
				KeyValuePair<ItemStack, GameObject> stack = enchantMiscInvenList.FirstOrDefault(x => x.Value == go);
				KeyValuePair<Industry, GameObject> ind = enchantIngredList.FirstOrDefault(x => x.Value == go);
				Enchantment ench = null;
				if(stack.Key != null) ench = GameRegistry.GetEnchantmentByItem(stack.Key.item);
				if(ind.Key != null) ench = GameRegistry.GetEnchantmentByItem(ind.Key.industryItem);
				if(ench != null && (filter < 0 || ((int)ench.enchantSlotRestriction & filter) > 0)) {
					go.SetActive(true);
					//go.transform.localPosition = new Vector3(6, (i * -125) - 5, 0);
					ingreds.Add(new GameObjectPair(go.transform, ind.Key, stack.Key));
				}
				else {
					if(ench != null)
						go.SetActive(false);
				}
			}

			/*for(var i = 0; i < inventoryList2.childCount; i++) {
				Transform t = inventoryList2.GetChild(i);
				KeyValuePair<ItemStack, GameObject> stack;
				stack = enchantMiscInvenList.FirstOrDefault(x => x.Value == t.gameObject);
				KeyValuePair<Industry, GameObject> ind;
				ind = enchantIngredList.FirstOrDefault(x => x.Value == t.gameObject);
				ingreds.Add(new GameObjectPair(t, ind.Key, stack.Key));
			}*/
			ingreds.Sort((x, y) => {
				var v = y.val.CompareTo(x.val);
				return v != 0 ? v : x.name.CompareTo(y.name);
			});
			for(var i = 0; i < ingreds.Count; i++) {
				ingreds[i].t.localPosition = new Vector3(6, (i * -125) - 5, 0);
			}
			
			((RectTransform)inventoryList2).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ingreds.Count * 126 + 5);
			inventoryList2.transform.localPosition = Vector3.zero;
		}

		public static void getOutput() {
			if(preSelectedIndustry != null) {
				if(inputStack1.stackSize <= preSelectedIndustry.quantityStored && inputStack2.stackSize <= preSelectedStack.stackSize) {
					preSelectedIndustry.quantityStored -= inputStack1.stackSize;
					if(preSelectedStack.item.industry != null) {
						preSelectedStack.item.industry.quantityStored -= inputStack2.stackSize;
					}
					else {
						//Debug.Log(preSelectedStack.stackSize);
						preSelectedStack.stackSize -= inputStack2.stackSize;
						//Debug.Log(preSelectedStack.stackSize);
					}
					Main.instance.player.addItemToInventory(outputStack);
					setupUI();


					if(!StatisticsTracker.firstEnchantment.isAchieved()) {
						StatisticsTracker.firstEnchantment.setAchieved();
						StatisticsTracker.maxQuestDifficulty.addValue(2);
						StatisticsTracker.minQuestDifficulty.addValue(1);
					}
				}
				sortIngredientList();
			}
			else if(inputStack2.stackSize <= preSelectedStack.stackSize) {
				//inputStack1 gets the new enchantment
				if(preSelectedStack.item.industry != null) {
					//Debug.Log(preSelectedStack.item.industry.quantityStored);
					preSelectedStack.item.industry.quantityStored -= inputStack2.stackSize;
					//Debug.Log(preSelectedStack.item.industry.quantityStored);
				}
				else {
					//Debug.Log(preSelectedStack.stackSize);
					preSelectedStack.stackSize -= inputStack2.stackSize;
					//Debug.Log(preSelectedStack.stackSize);
				}
				inputStack1.stackSize -= 1;
				if(inputStack1.stackSize == 0)
					Main.instance.player.miscInventory.Remove(inputStack1);
				outputStack.relicData = inputStack1.relicData;
				inputStack1.relicData = null;
				Main.instance.player.addItemToInventory(outputStack);
				setupUI();
				if(!StatisticsTracker.firstEnchantment.isAchieved()) {
					StatisticsTracker.firstEnchantment.setAchieved();
					StatisticsTracker.maxQuestDifficulty.addValue(2);
					StatisticsTracker.minQuestDifficulty.addValue(1);
				}
				sortIngredientList();
			}
		}

		private class GameObjectPair {
			public readonly Transform t;
			public readonly Industry ind;
			public readonly ItemStack stack;
			public readonly int val = -1;
			public readonly string name = "";

			public GameObjectPair(Transform t, Industry ind, ItemStack stack) {
				this.t = t;
				this.ind = ind;
				this.stack = stack;
				if(ind != null) {
					Enchantment ench = GameRegistry.GetEnchantmentByItem(ind.industryItem);
					val = (ind.quantityStored >= ench.ingredientQuantity ? 1 : 0);
					name = ind.unlocalizedName;
				}
				else if(stack != null) {
					Enchantment ench = GameRegistry.GetEnchantmentByItem(stack.item);
					val = (stack.stackSize >= ench.ingredientQuantity ? 1 : 0);
					name = stack.item.name;
				}
			}
		}
	}
}
