﻿using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.upgrades {
	class UpgradeIncome : Upgrade {
		protected readonly float amount;
		public UpgradeIncome(BigInteger upgradeCost, float amount, string saveName) : base(UpgradeType.MONEY_INCOME, upgradeCost, "Increase all Income by " + amount + "x", saveName) {
			this.amount = amount;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value *= amount;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value /= amount;
		}

		public override string getTooltip() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			return "Increases income for all products sold and vendored. Other sources are not affected.";
		}

		public override string getIconName() {
			return "upgrades/cash_multi";
		}
	}
}
