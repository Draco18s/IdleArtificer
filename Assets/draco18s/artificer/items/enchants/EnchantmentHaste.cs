using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.quests;

namespace Assets.draco18s.artificer.items.enchants {
	class EnchantmentHaste : Enchantment {
		public EnchantmentHaste(string enchantName, Item ingredientHerb, int quantity, ItemEquipType restrictions) : base(enchantName, ingredientHerb, quantity, restrictions, 0) {
		}

		public override void onUsedDuringQuest(Quest quest, ItemStack itemStack) {
			quest.hastenQuestEnding(-30);
		}
	}
}
