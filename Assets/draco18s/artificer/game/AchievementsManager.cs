using UnityEngine;
using System.Collections;
using Assets.draco18s.artificer.statistics;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.draco18s.util;
using Assets.draco18s.config;

namespace Assets.draco18s.artificer.game {
	public class AchievementsManager {
		private static Transform achievList;
		private static Transform statsList;
		private static Dictionary<StatAchievement, GameObject> achLookup = new Dictionary<StatAchievement, GameObject>();
		private static Dictionary<StatBase, GameObject> statLookup = new Dictionary<StatBase, GameObject>();

		public static void OneTimeSetup() {
			achievList = GuiManager.instance.achievementsArea.transform.FindChild("Achievements").GetChild(1).GetChild(0);
			statsList = GuiManager.instance.achievementsArea.transform.FindChild("Statistics").GetChild(1).GetChild(0);
			IEnumerator<StatAchievement> list = StatisticsTracker.getAchievementsList();
			int h = 0;
			while(list.MoveNext()) {
				StatAchievement item = list.Current;
				GameObject obj = GameObject.Instantiate(PrefabManager.instance.ACHIEVEMENT_LISTITEM, achievList) as GameObject;
				obj.transform.localPosition = new Vector3(11, -50 * item.displayOrder - 11, 0);
				//TODO: translate
				obj.transform.FindChild("Name").GetComponent<Text>().text = Localization.translateToLocal(item.achieveName);
				obj.transform.FindChild("Description").GetComponent<Text>().text = Localization.translateToLocal(item.description);
				obj.transform.FindChild("Progress").GetComponent<Text>().text = item.isAchieved()?"Completed":"In progress";
				obj.transform.FindChild("Image").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource(item.achieveImage + (item.isAchieved() ? "":"_off"));
				h++;
				achLookup.Add(item, obj);
			}
			((RectTransform)achievList.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50 * h + 18);

			IEnumerator<StatBase> list2 = StatisticsTracker.getStatsList();
			h = 0;
			while(list2.MoveNext()) {
				StatBase item = list2.Current;
				GameObject obj = GameObject.Instantiate(PrefabManager.instance.ACHIEVEMENT_LISTITEM, statsList) as GameObject;
				obj.transform.localPosition = new Vector3(11, -50 * item.displayOrder - 11, 0);
				Transform trans;
				trans = obj.transform.FindChild("Name");
				trans.GetComponent<Text>().text = Localization.translateToLocal(item.statName);
				trans.localPosition -= new Vector3(35,0,0);
				trans = obj.transform.FindChild("Description");
				trans.GetComponent<Text>().text = Localization.translateToLocal(item.description);
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
				item.Value.transform.FindChild("Progress").GetComponent<Text>().text = item.Key.isAchieved() ? "Completed" : "In progress";
				item.Value.transform.FindChild("Image").GetComponent<Image>().sprite = SpriteLoader.getSpriteForResource(item.Key.achieveImage + (item.Key.isAchieved() ? "" : "_off"));
			}
			Dictionary<StatBase, GameObject>.Enumerator statlist = statLookup.GetEnumerator();
			while(statlist.MoveNext()) {
				KeyValuePair<StatBase, GameObject> item = statlist.Current;
				item.Value.transform.FindChild("Progress").GetComponent<Text>().text = item.Key.getDisplay();
			}
		}

		public static void update() {

		}
	}
}