using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.items {
	public class Enchantment {
		public readonly Item ingredient;
		public readonly int ingredientQuantity;
		public readonly ItemEquipType enchantSlotRestriction;
		public readonly string name;
		public readonly RequirementType reqTypes;
		public int maxConcurrent = 1;
		public int ID;
		protected float effectivenessMulti = 1;
		public Enchantment(string enchantName, Item ingredientHerb, int quantity, ItemEquipType restrictions, RequirementType satisfiesReq) {
			name = enchantName;
			ingredient = ingredientHerb;
			ingredientQuantity = quantity;
			enchantSlotRestriction = restrictions;
			reqTypes = satisfiesReq;
			GameRegistry.RegisterEnchantment(this);
		}

		public Enchantment setStacksTo(int v) {
			maxConcurrent = v;
			return this;
		}

		public Enchantment setEffectiveness(float val) {
			effectivenessMulti = val;
			return this;
		}

		public float getEffectiveness() {
			return effectivenessMulti;
		}
	}

	[Flags]
	public enum ItemEquipType {
		HELMET =	(1 << 0),
		ARMOR =		(1 << 1),
		SHIELD =	(1 << 2),
		RING =		(1 << 3),
		BOOTS =		(1 << 4),
		WEAPON =	(1 << 5),
		RANGED =	(1 << 6),
		CLOAK =		(1 << 7),
		MISC =		(1 << 8)
	}
}
