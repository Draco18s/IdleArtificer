using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeVendorEffectiveness:Upgrade {
		public readonly float increaseAmt;
		public UpgradeVendorEffectiveness(BigInteger upgradeCost, float increase, string saveName):base(upgradeCost, "Vendor Effectiveness", saveName) {
			increaseAmt = increase;
		}
		public override void applyUpgrade() {
			base.applyUpgrade();
			Main.instance.player.IncreaseVendorValue(increaseAmt);
		}
		public override void revokeUpgrade() {
			base.applyUpgrade();
			Main.instance.player.IncreaseVendorValue(-increaseAmt);
		}
		public override string getTooltip() {
			return "Increases the price vendors sell items at.";
		}
	}
}
