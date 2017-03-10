using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.draco18s.util;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.game;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeHalveDouble : Upgrade {
		public readonly Industry affectedIndustry;
		public UpgradeHalveDouble(BigInteger upgradeCost, Industry affects, string saveName) : base(UpgradeType.MISC, upgradeCost, "Halve-and-Double " + Main.ToTitleCase(affects.name), saveName) {
			affectedIndustry = affects;
		}
		public override void applyUpgrade() {
			base.applyUpgrade();
			affectedIndustry.halveAndDouble(1);
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			affectedIndustry.halveAndDouble(-1);
		}

		public override string getIconName() {
			return affectedIndustry.name;
		}

		public override string getTooltip() {
			return "Doubles the speed at which the item is produced, but doubles the base cost and halves its upgrade level.\nNet output remains the same.";
		}
	}
}
