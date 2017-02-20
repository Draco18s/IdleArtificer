using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
namespace Assets.draco18s.artificer.ui {
	public class SelectObject : MonoBehaviour, ISelectHandler {
		Action<GameObject> selectCallback;
		Action<GameObject> deSelCallback;

		public void OnSelect(BaseEventData eventData) {
			if(selectCallback == null) {
				throw new Exception("OnSelect for " + gameObject.name + " must have an event handler delegate!");
			}
			selectCallback(this.gameObject);
		}

		public void OnDeselect(BaseEventData eventData) {
			if(selectCallback == null) {
				throw new Exception("OnDeselect for " + gameObject.name + " must have an event handler delegate!");
			}
			deSelCallback(this.gameObject);
		}

		public void selectListener(Action<GameObject> p) {
			selectCallback = p;
		}

		public void deselectListener(Action<GameObject> p) {
			deSelCallback = p;
		}
	}
}