using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	class UpgradeStartingCash : Upgrade {
		protected readonly int amount;
		public UpgradeStartingCash(BigInteger upgradeCost, int amount, string saveName) : base(UpgradeType.START_CASH, upgradeCost, "Increase Starting Cash by $" + Main.AsCurrency(amount), saveName) {
			this.amount = amount;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeIntValue)wrap).value += amount;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeIntValue)wrap).value -= amount;
		}

		/*public override string getTooltip() {
			return "Increases the effectiveness of clicking, including Apprentices.";
		}*/

		public override string getIconName() {
			return "upgrades/cash";
		}
	}
}
