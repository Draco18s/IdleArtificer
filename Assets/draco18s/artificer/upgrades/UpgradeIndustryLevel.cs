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
	public class UpgradeIndustryLevel : Upgrade {
		public readonly Industry affectedIndustry;
		public readonly int bonus;

		public UpgradeIndustryLevel(BigInteger upgradeCost, int bonusValue, Industry affects, string saveName) : base(UpgradeType.MISC, upgradeCost, "+" + bonusValue + " "  + Main.ToTitleCase(Localization.translateToLocal(affects.unlocalizedName)) + " Level", saveName) {
			affectedIndustry = affects;
			bonus = bonusValue;
		}
		public override void applyUpgrade() {
			base.applyUpgrade();
			affectedIndustry.bonusLevel += bonus;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			affectedIndustry.bonusLevel -= bonus;
		}

		public override string getIconName() {
			return affectedIndustry.saveName;
		}
	}
}
