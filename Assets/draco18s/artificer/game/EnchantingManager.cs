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
			Transform enchantWindow = GuiManager.instance.enchantArea.transform.FindChild("Enchant");
			Transform i1 = enchantWindow.FindChild("Input1");
			Transform i2 = enchantWindow.FindChild("Input2");
			Transform o1 = enchantWindow.FindChild("Output");
			//i1.GetComponent<Button>().onClick.AddListener(delegate { addInput1(); });
			//i2.GetComponent<Button>().onClick.AddListener(delegate { addInput2(); });
			o1.GetComponent<Button>().onClick.AddListener(delegate { getOutput(); });
			inputImg1 = i1.GetComponent<Image>();
			inputImg2 = i2.GetComponent<Image>();
			outputImg = o1.GetComponent<Image>();
			inventoryList1 = GuiManager.instance.enchantArea.transform.FindChild("Inventory1").GetChild(0).GetChild(0);
			inventoryList2 = GuiManager.instance.enchantArea.transform.FindChild("Inventory2").GetChild(0).GetChild(0);
			inventoryList3 = GuiManager.instance.enchantArea.transform.FindChild("Inventory3").GetChild(0).GetChild(0);
#pragma warning disable 0219
			Enchantment e = Enchantments.ALERTNESS;
#pragma warning restore 0219
			outputName = enchantWindow.FindChild("OutputName").GetComponent<Text>();
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

			foreach(Industry ind in Main.instance.player.builtItems) {
				Industry newInd = ind;
				if(!ind.industryItem.isConsumable && ind.industryItem.canBeGivenToQuests) {
					GameObject go;
					enchantInvenList.TryGetValue(ind, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM);
						go.transform.SetParent(inventoryList1);
						enchantInvenList.Add(newInd, go);
					}
					go.transform.localPosition = new Vector3(6, (i * -125) - 5, 0);
					//ind.enchantInvenListObj = go;
					go.name = ind.name;
					go.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(ind.name);
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + Main.AsCurrency(10000);
					go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + ind.name);
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
							go.transform.FindChild("Req" + r).gameObject.SetActive(false);
						}
						else {
							//if(ind == Industries.LEATHER) {
							//	Debug.Log("   :" + req_num);
							//	Debug.Log("   :" + ((RequirementType)(1L << req_num)));
							//}
							go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
							ty = ty >> 1;
							ty = ty << 1;
						}
					}
					go.GetComponent<Button>().onClick.AddListener(delegate { /*SelectItem(newInd);*/addInput1(newInd); });
					i++;
				}
			}
			i = 0;
			int j = 0;

			//TODO: if item is an enchanted thingy, needs to be in the other list, or something.
			//It definitely needs to be placable in the *other* input slot
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				ItemStack s = stack;
				GameObject go;
				enchantMiscInvenList.TryGetValue(stack, out go);
				if(go == null) {
					go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM);
					go.transform.SetParent(inventoryList2);
					enchantMiscInvenList.Add(s, go);
				}

				go.transform.localPosition = new Vector3(6, (j * -125) - 5, 0);
				//ind.invenListObj = go;
				go.name = stack.item.name;
				go.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(stack.getDisplayName());
				go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
				Enchantment ench = GameRegistry.GetEnchantmentByItem(stack.item);
				int req_num = 1;
				long ty = (long)stack.getAllReqs();
				if(ench != null) {
					ty |= (long)ench.reqTypes;
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize) + " / " + ench.ingredientQuantity;

					go.GetComponent<Button>().onClick.AddListener(delegate { /*SelectItem(s);*/addInput2(s); });
				}
				else {
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize);
					if(stack.enchants.Count > 0) {
						go.transform.SetParent(inventoryList3);
						go.transform.localPosition = new Vector3(6, (i * -125) - 5, 0);
						go.GetComponent<Button>().onClick.AddListener(delegate { /*SelectItem(s);*/addInput1(s); });
						i++;
						j--;
					}
				}
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
						go.transform.FindChild("Req" + r).gameObject.SetActive(false);
					}
					else {
						go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
						ty = ty >> 1;
						ty = ty << 1;
					}
				}

				j++;
				
			}
			FieldInfo[] allEnchants = typeof(Enchantments).GetFields();
			foreach(FieldInfo enchField in allEnchants) {
				Enchantment enchant = (Enchantment)enchField.GetValue(null);
				if(enchant.ingredient.industry != null) {
					int v = (enchant.ingredient.industry.quantityStored > int.MaxValue ? 999999999 : BigInteger.ToInt32(enchant.ingredient.industry.quantityStored));
					ItemStack stack = new ItemStack(enchant.ingredient.industry, v);

					GameObject go;
					enchantIngredList.TryGetValue(enchant.ingredient.industry, out go);
					if(go == null) {
						go = Main.Instantiate(PrefabManager.instance.INVEN_GUI_LISTITEM);
						go.transform.SetParent(inventoryList2);
						enchantIngredList.Add(enchant.ingredient.industry, go);
					}
					go.transform.localPosition = new Vector3(6, (j * -125) - 5, 0);
					//ind.invenListObj = go;
					go.name = stack.item.name;
					go.transform.FindChild("Title").GetComponent<Text>().text = Main.ToTitleCase(stack.getDisplayName());
					go.transform.FindChild("Img").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource("items/" + stack.item.name);
					Enchantment ench = GameRegistry.GetEnchantmentByItem(stack.item);
					int req_num = 1;
					long ty = (long)stack.getAllReqs();
					
					ty |= (long)ench.reqTypes;
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize) + " / " + ench.ingredientQuantity;

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
							go.transform.FindChild("Req" + r).gameObject.SetActive(false);
						}
						else {
							go.transform.FindChild("Req" + r).GetComponent<Image>().sprite = GuiManager.instance.req_icons[req_num - 1];
							ty = ty >> 1;
							ty = ty << 1;
						}
					}
				}
			}

			((RectTransform)inventoryList2).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, j * 126 + 5);
			inventoryList2.transform.localPosition = Vector3.zero;
			((RectTransform)inventoryList1).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i * 126 + 5);
			inventoryList1.transform.localPosition = Vector3.zero;
		}

		public static void update() {
			foreach(Industry ind in Main.instance.player.builtItems) {
				GameObject go;
				enchantInvenList.TryGetValue(ind, out go);
				if(go != null) {
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + Main.AsCurrency(10000);
				}
				else {
					enchantIngredList.TryGetValue(ind, out go);
					if(go != null) {
						//int v = (ind.quantityStored > int.MaxValue ? 999999999 : BigInteger.ToInt32(ind.quantityStored));
						//ItemStack stack = new ItemStack(ind, v);
						go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(ind.quantityStored) + " / " + Main.AsCurrency(25000);
					}
				}
			}
			foreach(ItemStack stack in Main.instance.player.miscInventory) {
				GameObject go;
				enchantMiscInvenList.TryGetValue(stack, out go);
				if(go != null) {
					go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(stack.stackSize) + " / 200";
				}
			}
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
				go.transform.FindChild("Quantity").GetComponent<Text>().text = Main.AsCurrency(selectedIndustry.quantityStored) + " / " + Main.AsCurrency(10000);
				preSelectedIndustry = selectedIndustry;
				selectedIndustry = null;
			}
			else {
				inputImg1.sprite = GuiManager.instance.gray_square;
				inputStack1 = null;
				preSelectedIndustry = null;
			}
			doOutput();
		}
		public static void addInput1(ItemStack selectedStack) {
			if(selectedStack != null) {
				inputImg1.sprite = SpriteLoader.getSpriteForResource("items/" + selectedStack.item.name);
				inputStack1 = selectedStack;// new ItemStack(selectedStack.item, GameRegistry.GetEnchantmentByItem(selectedStack.item).ingredientQuantity);
				//preSelectedStack = selectedStack;
				//selectedStack = null;
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
				Debug.Log(inputStack1.item.name + "[" + inputStack1.enchants.Count + "], " + inputStack2.item.name + " → " + (ench!=null? ench.name:"null"));
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
					if(maxed || inputStack1.relicData == null || inputStack1.relicData.Count < inputStack1.enchants.Count) {
						outputImg.sprite = GuiManager.instance.gray_square;
						outputImg.color = Color.white;
						outputStack = null;
						outputName.text = "";
					}
				}
				else if((inputStack1.item.equipType & ench.enchantSlotRestriction) > 0) {
					outputStack = new ItemStack(inputStack1.item, 1);
					outputStack.applyEnchantment(ench);
					outputName.text = outputStack.getDisplayName();
					outputImg.sprite = inputImg1.sprite;
					if(inputStack1.stackSize <= preSelectedIndustry.quantityStored && inputStack2.stackSize <= preSelectedStack.stackSize) {
						//Debug.Log("   White");
						//Debug.Log(inputStack1.stackSize +" >= "+ preSelectedIndustry.quantityStored + " && " + inputStack2.stackSize + " >= " + preSelectedStack.stackSize);
						outputImg.color = Color.white;
					}
					else {
						//Debug.Log("   Red");
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
		}

		public static void getOutput() {
			if(inputStack1.stackSize <= preSelectedIndustry.quantityStored && inputStack2.stackSize <= preSelectedStack.stackSize) {
				preSelectedIndustry.quantityStored -= inputStack1.stackSize;
				if(preSelectedStack.item.industry != null) {
					preSelectedStack.item.industry.quantityStored -= inputStack2.stackSize;
				}
				else {
					preSelectedStack.stackSize -= inputStack2.stackSize;
				}
				Main.instance.player.addItemToInventory(outputStack);
				setupUI();


				if(!StatisticsTracker.firstEnchantment.isAchieved()) {
					StatisticsTracker.firstEnchantment.setAchieved();
					StatisticsTracker.maxQuestDifficulty.addValue(2);
					StatisticsTracker.minQuestDifficulty.addValue(1);
				}
			}
		}
	}
}
