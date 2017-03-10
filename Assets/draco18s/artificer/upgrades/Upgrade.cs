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
		public readonly UpgradeType upgradeType;

		public GameObject upgradListGui;

		private bool isPurchased = false;

		public Upgrade(UpgradeType ty, BigInteger upgradeCost, string dispName, string saveName) {
			upgradeType = ty;
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

	public enum UpgradeType {
		MISC,
		START_CASH,
		CLICK_RATE,
		VENDOR_SIZE,
		VENDOR_SELL_VALUE,
		MONEY_INCOME,
		RENOWN_INCOME,
		RENOWN_MULTI,
		TICK_RATE,
		QUEST_SCALAR,
		QUEST_SPEED,
		QUEST_DIFFICULTY
	}

	public abstract class UpgradeValueWrapper {

	}

	public class UpgradeIntValue : UpgradeValueWrapper {
		public int value;
		public UpgradeIntValue(int v) {
			value = v;
		}
	}

	public class UpgradeFloatValue : UpgradeValueWrapper {
		public float value;
		public UpgradeFloatValue(float v) {
			value = v;
		}
	}
}
