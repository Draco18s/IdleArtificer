using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.items;
using Assets.draco18s.config;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeIndustryValue : Upgrade {
		public readonly Industry affectedIndustry;
		public readonly float multiplier;

		public UpgradeIndustryValue(BigInteger upgradeCost, float moneyMultiplier, Industry affects, string saveName) : base(UpgradeType.MISC, upgradeCost, moneyMultiplier + "x " + Main.ToTitleCase(Localization.translateToLocal(affects.unlocalizedName)) + " Value", saveName) {
			affectedIndustry = affects;
			multiplier = moneyMultiplier;
		}
		public override void applyUpgrade() {
			base.applyUpgrade();
			affectedIndustry.addValueMultiplier(multiplier);
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			affectedIndustry.addValueMultiplier(1/multiplier);
		}

		public override string getIconName() {
			return affectedIndustry.saveName;
		}
	}
}
