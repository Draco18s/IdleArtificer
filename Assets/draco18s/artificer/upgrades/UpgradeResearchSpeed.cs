using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeResearchSpeed : Upgrade {
		protected readonly float value;
		public UpgradeResearchSpeed(BigInteger upgradeCost, float val, string saveName) : base(UpgradeType.RESEARCH_RATE, upgradeCost, "Increases relic ID speed by " + (val*100) + "%", saveName) {
			value = val;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value += value;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value -= value;
		}

		public override string getTooltip() {
			return "Reduces the time it takes to identify relics.";
		}

		public override string getIconName() {
			return "upgrades/quest_speed";
		}
	}
}
