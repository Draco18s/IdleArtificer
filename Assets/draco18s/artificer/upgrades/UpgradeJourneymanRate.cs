using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeJourneymanRate : Upgrade {
		protected readonly float time;
		public UpgradeJourneymanRate(BigInteger upgradeCost, float timeDelta, string saveName) : base(UpgradeType.JOURNEYMAN_RATE, upgradeCost, "Increases journemen equip rate by " + Main.SecondsToTime(timeDelta), saveName) {
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
			return "Reduces the amount of time it takes for journeymen to equip items to quests.";
		}

		public override string getIconName() {
			return "upgrades/quest_speed";
		}
	}
}
