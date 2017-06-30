using Assets.draco18s.artificer.game;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	class UpgradeTheLoots : Upgrade {

		protected readonly float amount;
		public UpgradeTheLoots(BigInteger upgradeCost, float amount, string saveName):base(UpgradeType.QUEST_LOOT,upgradeCost,"Items Found " + (1+amount) + "x", saveName) {
			this.amount = 1+amount;
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
			return "Increases the amount of rare items found by heroes by " + (amount * 100) + "%.\nThe base value is 100%, currently it is " + (((UpgradeFloatValue)wrap).value * 100) + "%, and with this upgrade it would be " + ((((UpgradeFloatValue)wrap).value * amount) * 100) + "%";
			//return "Increases the amount of rare items found by heroes by " + (amount*100) + "%, the base value is 100%, and with this upgrade it would be " + ((((UpgradeFloatValue)wrap).value + amount) * 100) + " % ";
		}

		public override string getIconName() {
			return "upgrades/item_multi";
		}
	}
}
