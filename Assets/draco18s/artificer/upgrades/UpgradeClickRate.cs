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
		public UpgradeClickRate(BigInteger upgradeCost, float timeDelta, string saveName) : base(upgradeCost, "Increase Click Rate by " + Main.SecondsToTime(timeDelta), saveName) {
			time = timeDelta;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			Main.instance.player.adjustClickRate(time);
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			Main.instance.player.adjustClickRate(-time);
		}

		public override string getTooltip() {
			return "Increases the effectiveness of clicking, including Apprentices.";
		}

		public override string getIconName() {
			return "upgrades/click_rate";
		}
	}
}
