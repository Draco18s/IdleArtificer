using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.statistics;
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
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			int v = StatisticsTracker.moneyMagnitude.value;
			v -= (v % 3);
			v /= 3;
			v = Math.Max(v - 1, 0);
			float b = 0.05f * v * (float)(1 + SkillList.VendorEffectiveness.getMultiplier());
			return "Increases the price vendors sell items by " + (increaseAmt * 100) + "%.\nThe base value is "+((b+1)*100)+"%, currently it is " + ((((UpgradeFloatValue)wrap).value+b) * 100) + "%, and with this upgrade it would be " + ((((UpgradeFloatValue)wrap).value + increaseAmt + b) * 100) + "%";
			//return "Increases the price vendors sell items at.";
		}
	}
}
