using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeQuestReqs : Upgrade {
		public UpgradeQuestReqs(BigInteger upgradeCost, string saveName) : base(UpgradeType.MISC, upgradeCost, "Show better quest details", saveName){

		}

		public override string getIconName() {
			return "upgrades/quest_reqs";
		}

		public override string getTooltip() {
			return "Look into the future and know which items will help the hero the most often.\nQuest requirements attempts to show the most common requirements, instead of only the requirements from the hero's starting location and their goal.";
		}
	}
}
