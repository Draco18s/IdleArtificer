using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.tutorial {
	public delegate bool Check();

	public class TutorialStep {

		public Vector3 arrowTarget;
		public Vector3 displayAt;
		public string displayText;
		public Check triggerStep;
		public Check hideTrigger;
		public Check isOnCorrectTab;

		public TutorialStep(Vector3 target, Vector3 textLoc, string text, Check trigger, Check hide, Check correctTab) {
			arrowTarget = target;
			displayAt = textLoc;
			displayText = text;
			triggerStep = trigger;
			hideTrigger = hide;
			isOnCorrectTab = correctTab;
		}
	}
}