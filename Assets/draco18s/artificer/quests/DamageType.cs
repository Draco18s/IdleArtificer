using Assets.draco18s.artificer.quests.requirement;

namespace Assets.draco18s.artificer.quests {
	public class DamageType {
		public static DamageType GENERIC = new DamageType();
		public static DamageType STARVE = new DamageType().setBypassesArmor();
		public static DamageType DROWN = new DamageType(RequirementType.WATER_BREATH).setBypassesArmor();
		public static DamageType FIRE = new DamageType(RequirementType.FIRE_IMMUNE).setBypassesArmor();
		public static DamageType COLD = new DamageType(RequirementType.COLD_IMMUNE).setBypassesArmor();
		public static DamageType ACID = new DamageType(RequirementType.ACID_IMMUNE).setBypassesArmor();
		public static DamageType HOLY = new DamageType(RequirementType.HOLY_IMMUNE).setBypassesArmor();
		public static DamageType UNHOLY = new DamageType(RequirementType.UNHOLY_IMMUNE).setBypassesArmor();
		public static DamageType POISON = new DamageType(RequirementType.POISON_IMMUNE).setBypassesArmor();
		public static DamageType FALL = new DamageType(RequirementType.FEATHER_FALL).setBypassesArmor();
		public static DamageType PETRIFY = new DamageType().setBypassesArmor();

		protected RequirementType immune;
		protected bool bypassArmor = false;
		
		public DamageType() {

		}

		public DamageType(RequirementType ty) {
			immune = ty;
		}

		public DamageType setBypassesArmor() {
			bypassArmor = true;
			return this;
		}

		public RequirementType getImmunityType() {
			return immune;
		}

		public bool getBypassesArmor() {
			return bypassArmor;
		}

		public override string ToString() {
			return immune.ToString();
		}
	}
}