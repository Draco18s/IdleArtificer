using UnityEngine;
using System.Collections;
using Assets.draco18s.artificer.statistics;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.draco18s.util;
using Assets.draco18s.config;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.game {
	public class AchievementsManager {
		private static Transform achievList;
		private static Transform statsList;
		private static Dictionary<StatAchievement, GameObject> achLookup = new Dictionary<StatAchievement, GameObject>();
		private static Dictionary<IStat, GameObject> statLookup = new Dictionary<IStat, GameObject>();

		public static void OneTimeSetup() {
			achievList = GuiManager.instance.achievementsArea.transform.FindChild("Achievements").GetChild(1).GetChild(0);
			statsList = GuiManager.instance.achievementsArea.transform.FindChild("Statistics").GetChild(1).GetChild(0);
			IEnumerator<StatAchievement> list = StatisticsTracker.getAchievementsList();
			int h = 0;
			while(list.MoveNext()) {
				GameObject obj;
				StatAchievement item = list.Current;
				if(item.isHidden) continue;
				if(item is AchievementMulti) {
					obj = GameObject.Instantiate(PrefabManager.instance.ACHIEVEMENT_MULTI_LISTITEM, achievList) as GameObject;
				}
				else {
					obj = GameObject.Instantiate(PrefabManager.instance.ACHIEVEMENT_LISTITEM, achievList) as GameObject;
					obj.transform.FindChild("Progress").GetComponent<Text>().text = item.isAchieved() ? "Completed" : "In progress";
				}
				obj.transform.localPosition = new Vector3(11, -50 * h - 11, 0);
				obj.transform.FindChild("Name").GetComponent<Text>().text = Localization.translateToLocal(item.achieveName);
				obj.transform.FindChild("Description").GetComponent<Text>().text = Localization.translateToLocal(item.description);
				obj.transform.FindChild("Image").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource(item.achieveImage + (item.isAchieved() ? "":"_off"));
				h++;
				achLookup.Add(item, obj);
			}
			((RectTransform)achievList.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50 * h + 18);

			IEnumerator<IStat> list2 = StatisticsTracker.getStatsList();
			h = 0;
			while(list2.MoveNext()) {
				IStat item = list2.Current;
				if(item.isHidden) continue;
				GameObject obj = GameObject.Instantiate(PrefabManager.instance.ACHIEVEMENT_LISTITEM, statsList) as GameObject;
				obj.transform.localPosition = new Vector3(11, -50 * h - 11, 0);
				Transform trans;
				trans = obj.transform.FindChild("Name");
				trans.GetComponent<Text>().text = Localization.translateToLocal(item.statName);
				trans.localPosition -= new Vector3(35,0,0);
				trans = obj.transform.FindChild("Description");
				trans.GetComponent<Text>().text = item.description;
				trans.localPosition -= new Vector3(35, 0, 0);
				obj.transform.FindChild("Progress").GetComponent<Text>().text = item.getDisplay();
				obj.transform.FindChild("Image").gameObject.SetActive(false);
				obj.transform.FindChild("BG").gameObject.SetActive(false);
				h++;
				statLookup.Add(item, obj);
			}
			((RectTransform)statsList.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50 * h + 18);
		}

		public static void setupUI() {
			Dictionary<StatAchievement, GameObject>.Enumerator achlist = achLookup.GetEnumerator();
			while(achlist.MoveNext()) {
				KeyValuePair<StatAchievement, GameObject> item = achlist.Current;
				item.Value.transform.FindChild("Image").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource(item.Key.achieveImage + (item.Key.isAchieved() ? "" : "_off"));
				if(item.Key is AchievementMulti) {
					AchievementMulti mult = (AchievementMulti)item.Key;
					int numChecked = mult.getNumAchieved();
					for(int j = 1; j <= mult.getNumValues(); j++) {
						Image img = item.Value.transform.FindChild("Check(" + j + ")").GetComponent<Image>();
						if(j <= numChecked) {
							img.sprite = GuiManager.instance.checkOn;
							img.color = ColorHelper.DGREEN;
						}
						else {
							img.sprite = GuiManager.instance.checkOff;
							img.color = ColorHelper.DRED;
						}
					}
					item.Value.transform.FindChild("Description").GetComponent<Text>().text = string.Format(Localization.translateToLocal(item.Key.description), Main.AsCurrency(mult.getNextValue(), 6));
				}
				else {
					item.Value.transform.FindChild("Progress").GetComponent<Text>().text = item.Key.isAchieved() ? "Completed" : "In progress";
				}
			}
			Dictionary<IStat, GameObject>.Enumerator statlist = statLookup.GetEnumerator();
			while(statlist.MoveNext()) {
				KeyValuePair<IStat, GameObject> item = statlist.Current;
				item.Value.transform.FindChild("Progress").GetComponent<Text>().text = item.Key.getDisplay();
				item.Value.transform.FindChild("Description").GetComponent<Text>().text = item.Key.description;
			}
		}

		public static void update() {

		}
	}
}