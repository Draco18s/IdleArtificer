using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics;
using Assets.draco18s.artificer.game;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeVendorSize : Upgrade {
		protected readonly int amount;
		public UpgradeVendorSize(BigInteger upgradeCost, int amt, string saveName) : base(UpgradeType.VENDOR_SIZE, upgradeCost, "Increases the number of items vendors can sell by " + amt + ".", saveName) {
			amount = amt;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeIntValue)wrap).value += 2;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeIntValue)wrap).value -= 2;
		}

		public override string getTooltip() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			int baseval = ((UpgradeIntValue)wrap).value;
			return "Increases the number of items vendors can sell by " + amount + ".\nThe base value is 5, currently it is " + (baseval) + ", and with this upgrade it would be " + (baseval + amount) + ".\nSome industries (typically plants) have an additional multiplier.";
		}

		public override string getIconName() {
			return "upgrades/vendor_size";
		}
	}
}
