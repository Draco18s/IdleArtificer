using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	class UpgradeClickRate : Upgrade {
		protected readonly float time;
		public UpgradeClickRate(BigInteger upgradeCost, float timeDelta, string saveName) : base(UpgradeType.CLICK_RATE, upgradeCost, "Increase Click Rate by " + Main.SecondsToTime(timeDelta), saveName) {
			time = timeDelta;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value += time;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value -= time;
		}

		public override string getTooltip() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			return "Increases the effectiveness of clicking by " + time + "sec, including Apprentices.\nThe base value is 0.2sec, currently it is " + (Main.instance.player.getClickRate()) + "sec, and with this upgrade it would be " + (Main.instance.player.getClickRate() + time) + "sec";
		}

		public override string getIconName() {
			return "upgrades/click_rate";
		}
	}
}
