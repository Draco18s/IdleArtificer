using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.quests;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.items.item {
	public class ItemScepter : Item {
		public ItemScepter() : base("scepter") {
			addReqType(quests.requirement.RequirementType.CHARISMA);
		}

		public override void onUsedDuringQuest(Quest quest, ItemStack itemStack) {
			Main.instance.player.renown += 1;
			Main.instance.player.totalRenown += 1;
		}

		public override BigInteger getBaseValue() {
			return 10;
		}
	}
}
