using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.ui {
	public class NotificationItem {
		public readonly string title;
		public readonly string text;
		public readonly Sprite image;

		public NotificationItem(string header, string body, Sprite graphic) {
			title = header;
			text = body;
			image = graphic;
		}
	}
}
