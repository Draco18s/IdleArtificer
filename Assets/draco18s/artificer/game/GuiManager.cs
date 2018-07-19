using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Assets.draco18s.artificer.ui;
using System.Collections.Generic;
using DG.Tweening;

public class GuiManager : MonoBehaviour {
	public static GuiManager instance;
	public GameObject mainCanvas;
	public GameObject gridArea;
	public GameObject buildingList;
	public GameObject infoPanel;
	public GameObject topPanel;
	public GameObject currentMoney;
	public GameObject craftTab;
	public GameObject craftArea;
	public GameObject craftHeader;
	public GameObject enchantTab;
	public GameObject enchantArea;
	public GameObject enchantHeader;
	public GameObject questTab;
	public GameObject questArea;
	public GameObject questHeader;
	public GameObject guildTab;
	public GameObject guildArea;
	public GameObject guildHeader;
	public GameObject researchTab;
	public GameObject researchArea;
	public GameObject researchHeader;
	public GameObject achievementsTab;
	public GameObject achievementsArea;
	public GameObject achievementsHeader;

	public GameObject autoBuildBar;
	public GameObject buyVendorsArea;
	public GameObject buyApprenticesArea;
	public GameObject buyJourneymenArea;
	public GameObject guildmasterArea;
	public GameObject resetGuildWindow;
	public GameObject skillPanel;

	public GameObject tooltip;
	public GameObject notification;

	public GameObject autoBuildTarget;

	public Sprite gray_square;
	public Sprite inner_item_bg;
	public Sprite selTab;
	public Sprite unselTab;
	public Sprite checkOn;
	public Sprite checkOff;
	public Sprite[] req_icons;
	public Sprite[] equip_icons;

	private static List<NotificationItem> notificationQueue = new List<NotificationItem>();

	void Start() {
		instance = this;
		req_icons = Resources.LoadAll<Sprite>("items/req_icons");
		equip_icons = Resources.LoadAll<Sprite>("items/equip_icons");
		notification.transform.position = new Vector3(Screen.width - 5, Screen.height + 2, 0);
	}

	public static void ShowTooltip(Vector3 p, string v) {
		ShowTooltip(p, v, 1);
	}
	public static void ShowTooltip(Vector3 pos, string v, float ratio) {
		ShowTooltip(pos, v, ratio, 1);
	}
	public static void ShowTooltip(Vector3 pos, string v, float ratio, float scale) {
		if(v.Length == 0) return;

		instance.tooltip.SetActive(true);
		((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
		((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 64);
		instance.tooltip.transform.position = pos;
		Text textArea = instance.tooltip.transform.Find("Text").GetComponent<Text>();
		textArea.text = v;
		//width + 7.5
		//height + 6
		bool fits = false;
		if(textArea.preferredWidth < 610) {
			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textArea.preferredWidth);
			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textArea.preferredHeight);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (textArea.preferredWidth / 2) + 8);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (textArea.preferredHeight / 2) + 7.5f);
			fits = true;
		}
		/*if(t.preferredHeight < 232) {
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (t.preferredHeight / 4) + 7.5f);
			fits = true;
		}*/
		float w = 64;// t.preferredWidth;
		if(!fits) {
			float h = 68;
			do {
				w += 64;
				((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
				h = textArea.preferredHeight;
			} while(h * ratio > w);

			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
			h = textArea.preferredHeight;
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (w / 2) + 8);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (h / 2) + 7.5f);
		}
		float wid = ((RectTransform)instance.tooltip.transform).rect.width;
		float hig = ((RectTransform)instance.tooltip.transform).rect.height;
		if(instance.tooltip.transform.position.x + wid > Screen.width) {
			//shift the tooltip down. No check for off-screen
			if(instance.tooltip.transform.position.y - hig*1.5f < 35) {
				instance.tooltip.transform.position = new Vector3(Screen.width - wid / 2 - 5, instance.tooltip.transform.position.y + ((RectTransform)instance.tooltip.transform).rect.height, 0);
			}
			else {
				instance.tooltip.transform.position = new Vector3(Screen.width - wid / 2 - 5, instance.tooltip.transform.position.y - ((RectTransform)instance.tooltip.transform).rect.height, 0);
			}
		}
		else {
			instance.tooltip.transform.position += new Vector3(wid / 2, 0, 0);
		}
		instance.tooltip.transform.localScale = new Vector3(scale, scale, scale);
	}

	public static void ShowNotification(NotificationItem item) {
		notificationQueue.Add(item);
	}

	public void Update() {
		if(!DOTween.IsTweening(notification.transform) && notificationQueue.Count > 0) {
			NotificationItem item = notificationQueue[0];
			notificationQueue.RemoveAt(0);
			notification.transform.Find("Title").GetComponent<Text>().text = item.title;
			notification.transform.Find("Text").GetComponent<Text>().text = item.text;
			notification.transform.Find("Img").GetComponent<Image>().sprite = item.image;

			notification.transform.DOMoveY(Screen.height - 85, 0.5f, false).SetEase(Ease.InOutQuad).OnComplete(PauseCallback);
		}
	}

	public void PauseCallback() {
		notification.transform.DOMoveY(Screen.height - 85, 2.5f, false).SetEase(Ease.InOutQuad).OnComplete(ReturnCallback);
	}

	public void ReturnCallback() {
		notification.transform.DOMoveY(Screen.height + 2, 0.5f, false).SetEase(Ease.InOutQuad);//.OnComplete(ReturnCallback);
	}
}
