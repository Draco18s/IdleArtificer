using Assets.draco18s.artificer.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public interface IRelicMaker {
		string relicNames(ItemStack stack);
		string relicDescription(ItemStack stack);
	}
}
