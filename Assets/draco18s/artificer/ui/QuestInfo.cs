using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.ui {
	public class QuestInfo : MonoBehaviour {
		public GameObject Req1;
		public GameObject Req2;
		public GameObject Req3;
		public GameObject Req4;
		public GameObject HPBar;
		public GameObject MPBar;
		public GameObject ProgBar;
		public Text status;

		public GameObject getReq(int v) {
			switch(v) {
				case 1:
					return Req1;
				case 2:
					return Req2;
				case 3:
					return Req3;
				case 4:
					return Req4;
			}
			return null;
		}
	}
}
