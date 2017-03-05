using Assets.draco18s.artificer.game;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeQuestSpeed : Upgrade {
		protected readonly float time;
		public UpgradeQuestSpeed(BigInteger upgradeCost, float timeDelta, string saveName) : base(upgradeCost, "Reduces new quest delay by " + Main.SecondsToTime(timeDelta), saveName) {
			time = timeDelta;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			QuestManager.alterQuestTimer(time);
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			QuestManager.alterQuestTimer(-time);
		}

		public override string getTooltip() {
			return "Reduces the amount of time for a new quest to appear.";
		}

		public override string getIconName() {
			return "upgrades/quest_speed";
		}
	}
}
