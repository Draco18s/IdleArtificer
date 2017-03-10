using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeVendorEffectiveness:Upgrade {
		public readonly float increaseAmt;
		public UpgradeVendorEffectiveness(BigInteger upgradeCost, float increase, string saveName):base(UpgradeType.VENDOR_SELL_VALUE, upgradeCost, "Vendor Effectiveness", saveName) {
			increaseAmt = increase;
		}
		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value += increaseAmt;
		}
		public override void revokeUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value -= increaseAmt;
		}
		public override string getTooltip() {
			return "Increases the price vendors sell items at.";
		}
	}
}
