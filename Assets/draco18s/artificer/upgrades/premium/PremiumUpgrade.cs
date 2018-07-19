using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.upgrades.premium {
	public class PremiumUpgrade : Upgrade {
		public PremiumUpgrade(UpgradeType ty, BigInteger upgradeCost, string dispName, string saveName) : base(ty, upgradeCost, dispName, saveName) {
		}
	}
}
