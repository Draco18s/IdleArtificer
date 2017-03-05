using Assets.draco18s.artificer.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.draco18s.util;
using Assets.draco18s.artificer.game;
using UnityEngine;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.upgrades {
	public class Upgrade {
		public readonly string displayName;
		public readonly string saveName;
		public readonly BigInteger cost;

		public GameObject upgradListGui;

		private bool isPurchased = false;

		public Upgrade(BigInteger upgradeCost, string dispName, string saveName) {
			displayName = dispName;
			this.saveName = saveName;
			cost = upgradeCost;
		}
		public virtual void applyUpgrade() {
			isPurchased = true;
		}
		public virtual void revokeUpgrade() {
			isPurchased = false;
		}
		public bool getIsPurchased() {
			return isPurchased;
		}
		public void setIsPurchased(bool b) {
			isPurchased = b;
		}

		public virtual string getIconName() {
			return "";
		}

		public virtual string getTooltip() {
			return "";
		}
	}
}
