using Assets.draco18s.artificer.upgrades;
using Assets.draco18s.artificer.upgrades.premium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public static class PremiumUpgrades {
		public static List<Upgrade> AllPremiumUps = new List<Upgrade>();

		public static PremiumUpgrade CLICKTHROUGH = (PremiumUpgrade)new PremiumUpgrade(UpgradeType.MISC,99,"Clickthrough", "CLICKTHROUGH").register(AllPremiumUps);
		public static PremiumUpgrade VENDORREASSIGN = (PremiumUpgrade)new PremiumUpgrade(UpgradeType.MISC, 99, "VendorReassign", "VENDORREASSIGN").register(AllPremiumUps);

	}
}
