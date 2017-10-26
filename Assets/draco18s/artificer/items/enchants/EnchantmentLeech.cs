using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.quests;

namespace Assets.draco18s.artificer.items.enchants {
	public class EnchantmentLeech : Enchantment {
		public EnchantmentLeech(string enchantName, Item ingredientHerb, int quantity, ItemEquipType restrictions, RequirementType satisfiesReq) : base(enchantName, ingredientHerb, quantity, restrictions, satisfiesReq) {
		}

		public override void onUsedDuringQuest(Quest quest, ItemStack itemStack) {
			if(quest.heroCurHealth < quest.heroMaxHealth) {
				quest.heroCurHealth = Math.Min(quest.heroCurHealth + 5, quest.heroMaxHealth);
			}
		}
	}
}
