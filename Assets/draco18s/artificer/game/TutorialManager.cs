using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.tutorial;
using Assets.draco18s.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.draco18s.artificer.game {
	public class TutorialManager {
		protected static List<TutorialList> allTutorials = new List<TutorialList>();
		protected static GameObject arrow;
		protected static Vector3 offset = GuiManager.instance.mainCanvas.transform.position;

		public static void init() {
			arrow = Main.Instantiate(PrefabManager.instance.GRID_GUI_ARROW, GuiManager.instance.mainCanvas.transform) as GameObject;
			arrow.SetActive(false);
			arrow.transform.SetSiblingIndex(GuiManager.instance.tooltip.transform.GetSiblingIndex());

			createCraftingTutorial();
			createGuildTutorial();
			createQuestTutorial();
			createEnchantingTutorial();
			createResetTutorial();
			allTutorials.Reverse(); //foreach runs the array backwards
		}

		private static void createResetTutorial() {
			Check tabCheck = delegate {
				return GuiManager.instance.craftArea.GetComponent<Canvas>().enabled;
			};
			TutorialList tutorialList = new TutorialList("reset");
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-420, 225, 0), new Vector3(-260, 180, 0), "Your first shop reset! Well done, all that renown should come in handy.\nYou'll need to repurchase all of your industries, but their products will sell for more now.", delegate { return Main.instance.player.renown > 0; }, delegate { return Main.instance.player.builtItems.Count > 0 && Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-485, 290, 0), new Vector3(-260, 180, 0), "This is where the autobuild feature will come in handy. You can turn it on and off here. Remember that each industry will also have to be individually enabled.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(195, 345, 0), new Vector3(0, 200, 0), "You'll also need to repurchase all of your shop upgrades! And now that you have some renown you can spend it on renown upgrades too, just remember that shop resets will clear those too.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-95, 345, 0), new Vector3(0, 200, 0), "All of your enchanting ingredients are safe, though, as are all of the relics you've found and identified.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-420, 225, 0), new Vector3(-260, 180, 0), "This is the end of this part of the tutorial.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			allTutorials.Add(tutorialList);
		}

		private static void createCraftingTutorial() {
			Check tabCheck = delegate {
				return GuiManager.instance.craftArea.GetComponent<Canvas>().enabled;
			};
			TutorialList tutorialList = new TutorialList("crafting");
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-420, 225, 0), new Vector3(-260, 180, 0), "These are Industries. They are your primary way of making money. They produce goods of different kinds every 10 seconds.\nBuild the Wood industry to get started.", delegate { return true; }, delegate { return Main.instance.player.builtItems.Count > 0 && Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(0, -70, 0), new Vector3(175, 170, 0), "You can click and drag to move industries around the crafting area and right-click it to advance its progress.\nYou can also left-click it to get more details, such as its inputs, consumers, current stock and more. Do that now.", delegate { return true; }, delegate { return GuiManager.instance.infoPanel.transform.localPosition.x > -400; }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-100, 45, 0), new Vector3(0, 170, 0), "This shows the industry's level, allows you to upgrade (or downgrade) it. In general each level allows the industry to produce 1 good.\nYou can also assign Apprentices here (which you get access to much later).\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(0, 45, 0), new Vector3(0, 170, 0), "This shows the industry's inputs (if it has any) and how many this industry consumes. Clicking on the input's icon will show the info for that industry.\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(0, 45, 0), new Vector3(0, 150, 0), "You can also left or right-click the icon for the selected industry here to advance it.\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(110, 45, 0), new Vector3(0, 170, 0), "This area shows how much of this resource is currently being stored, as well as how quickly you're gaining (or losing) that stock\nClicking 'sell all' will sell all stored resources for their value (the price in the top-right).\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-100, -220, 0), new Vector3(-200, -310, 0), "This area shows the industries that consume this resource. They can be clicked on like inputs.\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(0, -220, 0), new Vector3(-200, -310, 0), "This area deals with the Smart Autobuild feature, which is mainly used later in the game to more easily build up production after a reset.\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-40, -150, 0), new Vector3(-200, -310, 0), "You need to enable it for each industry first as well as set the desired upgrade level before any effect will take place.\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(0, -220, 0), new Vector3(-200, -310, 0), "The 'Min Magnitude' setting is a threshhold. While no industry can be built if you don't have the purchase cost, this setting can make the autobuild ignore this industry until you have even more money (measured in powers of 10).\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(160, -185, 0), new Vector3(230, -310, 0), "This section deals with Vendors, which will automatically sell some resources after they are produced. Vendors have some intelligence and will not sell any stock that is needed by any industries that consume this resource.\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(160, -185, 0), new Vector3(230, -310, 0), "Some resources are very plentiful so vendors will sell more of them, but in general 1 vendor 1 good sold.\nYou start with 3 vendors and will be able to purcahse more later.\nGo ahead and assign one now.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return CraftingManager.selectedIcon.getRawVendors() > 0;
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(110, -220, 0), new Vector3(-200, -310, 0), "Starting Vendors means that when the industry is first built (whether by you or the Autobuild) that many vendors will automatically be assigned (if available).\nClick to continue.", delegate { return true; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-420, 225, 0), new Vector3(-260, 180, 0), "Good job! Your wood industry is producing wood and your vendors are selling it. Keep upgrading and collecting income until you have $50 and buy Charcoal", delegate { return Main.instance.player.money > 10; },
				delegate {
					CraftingManager.FacilitySelected(Main.instance.player.builtItems[0]);
					CraftingManager.ShowInfo();
					GuiManager.instance.infoPanel.transform.localPosition = new Vector3(0, 312, 0);
					return Input.GetMouseButtonUp(0);
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-420, 225, 0), new Vector3(-260, 180, 0), "Great! You're on your way to success. This is the end of this part of the tutorial.", delegate { return Main.instance.player.builtItems.Count > 1; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			allTutorials.Add(tutorialList);
		}

		private static void createGuildTutorial() {
			TutorialList tutorialList = new TutorialList("guild");
			Check tabCheck = delegate {
				return GuiManager.instance.guildArea.GetComponent<Canvas>().enabled;
			};
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(195, 345, 0), new Vector3(0, 200, 0), "Guild Hall unlocked!\nClick the Guild tab to learn more.", delegate { return StatisticsTracker.unlockedGuild.isAchieved(); }, delegate { return GuiManager.instance.guildArea.GetComponent<Canvas>().enabled; }, delegate {
				return !GuiManager.instance.guildArea.GetComponent<Canvas>().enabled;
			}));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(200, 60, 0), new Vector3(200, 50, 0), "You can't actually afford anything yet, but you're close! You'll want to know what's available so you can plan ahead.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-395, 265, 0), new Vector3(200, 50, 0), "These are upgrades to your shop. There are two categories:\n - Upgrades you can buy with cash\n - And upgrades your can buy with Renown\nBoth types are reset whenever you reset your shop.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(0, 290, 0), new Vector3(200, 50, 0), "You don't have any yet, as shown here, but when you do have some, each point increases your cash income by 2% (additive).\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(410, 290, 0), new Vector3(200, 50, 0), "Over here is how much renown you'll get when you reset the shop. The 'reset shop' button is in this location on the Crafting tab.\nDon't worry if this value isn't very high yet, you'll earn more soon.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(60, 125, 0), new Vector3(200, 50, 0), "The middle area here is for Vendors, Apprentices (remember those?), Journeymen, and Guildmasters.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(60, 125, 0), new Vector3(200, 50, 0), "Vendors automatically sell resources.\nApprentices are auto-clickers.\nJourneymen will equip adventurers when you start doing quests.\nGuildmasters offer flat bonuses to 4 different aspects of the game. You'll need 100,000 renown to elect a new one!\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(310, 130, 0), new Vector3(200, 60, 0), "Skills unlock when you elect your first guildmaster.\nElecting a new guildmaster is like a super-reset:\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(310, 130, 0), new Vector3(200, 60, 0), "All your earned renown converts into skill points, all but the ten most interesting relics are destroyed (with the rest scattered into the world to be found again, even more powerful than before), and all of your enchanting ingredients are lost.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(310, 130, 0), new Vector3(200, 60, 0), "And maybe some other things too. You'll have to discover them when the time comes.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(200, 60, 0), new Vector3(200, 50, 0), "This is the end of this part of the tutorial.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			allTutorials.Add(tutorialList);
		}

		public static void Reset() {
			foreach(TutorialList list in allTutorials) {
				list.lastStep = 0;
			}
		}

		private static void createQuestTutorial() {
			TutorialList tutorialList = new TutorialList("quests");
			Check tabCheck = delegate {
				return GuiManager.instance.questArea.GetComponent<Canvas>().enabled;
			};
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-95, 345, 0), new Vector3(0, 200, 0), "Quests unlocked!\nClick the Quest tab to learn more.", delegate { return StatisticsTracker.unlockedQuesting.isAchieved(); }, delegate { return GuiManager.instance.questArea.GetComponent<Canvas>().enabled; }, delegate {
				return !GuiManager.instance.questArea.GetComponent<Canvas>().enabled;
			}));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-400, 230, 0), new Vector3(5, 100, 0), "This shows the items you have available to give to heroes.\nThe second collumn is for non-industry resources, such as enchanted items, which is why it is currently empty.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-140, 230, 0), new Vector3(5, 100, 0), "These are the current heroes and their quest. The first one in the list has tooltips enabled; you will be able to examine them after the tutorial.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(360, 230, 0), new Vector3(5, 100, 0), "This area will show the active quests: ones that have been started and the hero is currently adventuring\nOnly some information about the quest will be shown (e.g. the hero's inventory will not be visible) and you will not be able to interact with the quest.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(360, 230, 0), new Vector3(5, 100, 0), "Aside from their inventory, heroes also have Health and Mana.\nHealth is lost whenever a hero takes damage. Monsters, traps, and the environment can all inflict damage.\nMana is an abstract representation of the hero's ability to continue: stamina, willpower, deadlines, etc.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-83, 263, 0), new Vector3(5, 100, 0), "These icons indicate what kinds of items will likely help the hero.\nNote that this is never a complete list and the exact meaning of each icon isn't important.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-473, 184, 0), new Vector3(5, 100, 0), "Your inventory will display the same icons for the types of requirements they fulfil.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-450, 295, 0), new Vector3(5, 100, 0), "You can use the filter menu to hide items that don't have a given icon. You can also click the requirement icon in an available quest.\nAssign a matching item to any quest now by clicking the item, then the quest inventory.", delegate { return true; },
				delegate {
					foreach(Quest q in QuestManager.availableQuests) {
						if(q.inventory.Count > 0) {
							for(int i = 0; i < 6; i++) {
								if(q.doesHeroHave((RequirementType)q.getReq(i))) {
									return true;
								}
							}
						}
					}
					return false;
				}, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-35, 170, 0), new Vector3(5, 100, 0), "Great! If the 'start quest' button has turned red, that means you don't have enough of that resource for the hero to leave yet.\nThis is fine, quests remain available for 4 hours giving you time to collect what you need.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-410, 163, 0), new Vector3(5, 100, 0), "This will show you how much you have and how much is needed by all quests.\nAdditionally, in the Crafting tab industries will recieve an icon if they're supplying a quest that shows the status ('not enough','enough for some but not all quests', and 'enough for all quests'). The industry info window will also have detailed information.\nYou can view that later if you leave an industry assigned to a quest.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-17, 144, 0), new Vector3(5, 100, 0), "This shows what items you'll get when you start the quest. These are items you can't craft! They're used for enchanting and you'll need about 200 of any one kind to unlock the Enchanting tab.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(5, 100, 0), new Vector3(5, 100, 0), "That's the end of this part of the tutorial. Keep it up!", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			allTutorials.Add(tutorialList);
		}

		private static void createEnchantingTutorial() {
			TutorialList tutorialList = new TutorialList("enchanting");
			Check tabCheck = delegate {
				return GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled;
			};
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-95, 345, 0), new Vector3(0, 200, 0), "Enchanting unlocked!\nClick the Quest tab to learn more.", delegate { return StatisticsTracker.unlockedEnchanting.isAchieved(); }, delegate { return GuiManager.instance.questArea.GetComponent<Canvas>().enabled; }, delegate {
				return !GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled;
			}));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-415, 220, 0), new Vector3(0, 200, 0), "Items can be enchanted by clicking an item to enchant—either an industry or an existing enchanted item—to add it to the enchanting UI.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-415, 220, 0), new Vector3(0, 200, 0), "Relics and enchanted items are sorted such that ones that can accept more enchantments appear towards the top.\nAdd an item to the enchanting UI by clicking on one now.", delegate { return GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled; }, delegate { return EnchantingManager.hasItem1(); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(415, 220, 0), new Vector3(0, 200, 0), "Then clicking an enchanting ingredient on the right, which are sorted alphabetically and by quantity.\nAdd one now\nIf none are visible, try a different base item.", delegate { return true; }, delegate { return EnchantingManager.hasItem2(); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-415, 220, 0), new Vector3(0, 200, 0), "Not all enchantments can be applied to all items.\nEach ingredient shows the quest requirements that the resulting item will fulfil.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(415, 0, 0), new Vector3(0, 200, 0), "Some enchantments may not add requirement effects, but have other effects, such as regenerating heros' health, mana, progressing from event to event more quickly, etc.\nClick to continue.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(415, 0, 0), new Vector3(0, 200, 0), "That's the end of this part of the tutorial. You should now have an understanding of every part of the game and its UI.\nThanks for playing", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));

			allTutorials.Add(tutorialList);
		}

		private static void createResearchTutorial() {
			TutorialList tutorialList = new TutorialList("research");
			Check tabCheck = delegate {
				return GuiManager.instance.researchArea.GetComponent<Canvas>().enabled;
			};
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-95, 345, 0), new Vector3(0, 200, 0), "Research unlocked!\nClick the Quest tab to learn more.", delegate { return StatisticsTracker.unlockedResearch.isAchieved(); }, delegate { return GuiManager.instance.questArea.GetComponent<Canvas>().enabled; }, delegate {
				return !GuiManager.instance.researchArea.GetComponent<Canvas>().enabled;
			}));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(140, 220, 0), new Vector3(240, 130, 0), "This bar shows the progress towards the identification of a new relic, the passive time remaining, and the number of unidentified relics.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(140, 220, 0), new Vector3(240, 130, 0), "Clicking the bar will advance the progress based on a multiple of your crafting click time.\nGo ahead and try it now and identify an artifact.", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));

			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(140, 220, 0), new Vector3(240, 130, 0), "Research complete! Return to the Research tab to continue the tutorial.", delegate {
				foreach(ItemStack stack in Main.instance.player.miscInventory) {
					if(stack.relicData != null && stack.isIDedByPlayer) {
						return true;
					}
				}
				return false;
			}, delegate { return GuiManager.instance.questArea.GetComponent<Canvas>().enabled; }, tabCheck));

			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-400, 120, 0), new Vector3(240, 130, 0), "Congrats! You've identified a relic. Some information about it is displayed here, with some more details in a tooltip, and a full examination in the info window.\nClick the relic to view its information.", delegate { return true; }, delegate { return GuiManager.instance.researchArea.transform.Find("RelicInfoOpen").gameObject.activeSelf; }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(145, 95, 0), new Vector3(240, 130, 0), "This window shows all the information about the relic: what heroes have completed quests with it, that's quest's notoriety, what it's enchanted with, and so on.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-60, -110, 0), new Vector3(240, 130, 0), "This shows the entire list of the relic's quest requirement icons, as only 5 can be displayed normally.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-85, -165, 0), new Vector3(240, 130, 0), "You can sell a relic for cash, though it is often worthwhile to hold onto them until they have a large amount of notoriety.\nA relic's notoriety is equal to the maximum of the individually noted quest notoriety values.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(-85, -165, 0), new Vector3(240, 130, 0), "You can also forget about this relic if you so desire. All this does is remove it from your inventory and toss it back into the pile of unidentified relics.\nClick to continue", delegate { return true; }, delegate { return Input.GetMouseButtonUp(0); }, tabCheck));
			tutorialList.tutorialList.Add(new TutorialStep(new Vector3(190, 85, 0), new Vector3(240, 130, 0), "That's everything!\nClose the relic info window to complete this part of the tutorial", delegate { return true; }, delegate { return !GuiManager.instance.researchArea.transform.Find("RelicInfoOpen").gameObject.activeSelf; }, tabCheck));

			allTutorials.Add(tutorialList);
		}

		public static void update() {
			bool exitEarly = false;
			foreach(TutorialList list in allTutorials) {
				if(list.lastStep < list.tutorialList.Count && list.tutorialList[list.lastStep].triggerStep()) {
					showTutorialItem(list.tutorialList[list.lastStep]);
					if(list.lastStep < list.tutorialList.Count && !list.tutorialList[list.lastStep].isOnCorrectTab()) {
						hideTutorialItem();
					}
				}
				if(list.lastStep < list.tutorialList.Count && list.tutorialList[list.lastStep].hideTrigger()) {
					hideTutorialItem();
					list.lastStep++;
				}
				if(exitEarly) break;
			}
		}

		private static void showTutorialItem(TutorialStep tutorialStep) {
			GuiManager.ShowTooltip(tutorialStep.displayAt + offset, tutorialStep.displayText, 2.25f, 1.3f, false);

			arrow.SetActive(true);
			float dx = (tutorialStep.displayAt).x - (tutorialStep.arrowTarget).x + ((RectTransform)GuiManager.instance.tooltip.transform).rect.width / 2;
			float dy = (tutorialStep.displayAt).y - (tutorialStep.arrowTarget).y;

			float dist = Mathf.Sqrt(dx * dx + dy * dy);
			((RectTransform)arrow.transform.GetChild(0)).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dist);

			arrow.transform.GetChild(0).localPosition = new Vector3((dist / -2), 0);
			arrow.transform.position = tutorialStep.arrowTarget + offset;//new Vector3((dist / -2) - 32, 0);
			arrow.transform.localEulerAngles = new Vector3(0, 0, 180 + MathHelper.RadiansToDegrees(Mathf.Atan2(dy, dx)));
		}

		private static void hideTutorialItem() {
			arrow.SetActive(false);
			GuiManager.HideTooltip();
		}

		protected class TutorialList {
			public readonly string saveID;
			public List<TutorialStep> tutorialList = new List<TutorialStep>();
			public int lastStep = 0;

			public TutorialList(string saveName) {
				saveID = saveName;
			}
		}

		public static void serialize(ref SerializationInfo info, ref StreamingContext context) {
			foreach(TutorialList list in allTutorials) {
				info.AddValue(list.saveID,list.lastStep);
			}
		}

		public static void deserialize(ref SerializationInfo info, ref StreamingContext context) {
			if(Main.saveVersionFromDisk >= 23) {
				SerializationInfoEnumerator infoEnum = info.GetEnumerator();
				Hashtable values = new Hashtable();
				while(infoEnum.MoveNext()) {
					SerializationEntry val = infoEnum.Current;
					values.Add(val.Name, val.Value);
				}
				foreach(TutorialList list in allTutorials) {
					if(values.Contains(list.saveID)) {
						list.lastStep = (int)values[list.saveID];
					}
				}
			}
		}
	}
}
