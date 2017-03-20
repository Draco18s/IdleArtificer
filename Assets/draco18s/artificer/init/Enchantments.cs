using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public class Enchantments {
		//armor
		public static Enchantment HEALING = new Enchantment("Healing", Items.ROYAL_JELLY, 1, ItemEquipType.ARMOR | ItemEquipType.RING, RequirementType.HEALING);
		public static Enchantment FIRE_RESISTANCE = new Enchantment("Fire Resistance", Items.DRAGON_SCALES, 200, ItemEquipType.ARMOR | ItemEquipType.CLOAK, RequirementType.FIRE_IMMUNE).setEffectiveness(0.25f).setStacksTo(7);
		public static Enchantment COLD_RESISTANCE = new Enchantment("Cold Resistance", Items.ROOSTER_TEETH, 200, ItemEquipType.ARMOR | ItemEquipType.CLOAK, RequirementType.COLD_IMMUNE).setEffectiveness(0.25f).setStacksTo(7);
		public static Enchantment POISON_RESISTANCE = new Enchantment("Poison Resistance", Items.VENOM, 200, ItemEquipType.ARMOR | ItemEquipType.CLOAK, RequirementType.POISON_IMMUNE).setEffectiveness(0.25f).setStacksTo(7);
		public static Enchantment ACID_RESISTANCE = new Enchantment("Acid Resistance", Items.DIATOM_EARTH, 200, ItemEquipType.ARMOR | ItemEquipType.CLOAK, RequirementType.ACID_IMMUNE).setEffectiveness(0.25f).setStacksTo(7);
		public static Enchantment SPELL_RESIST = new Enchantment("Spell Resistance", Items.MUMMY_WRAPPING, 200, ItemEquipType.ARMOR | ItemEquipType.SHIELD, RequirementType.SPELL_RESIST);

		//cloak
		public static Enchantment ETHEREAL = new Enchantment("Ethereal", Items.BLACK_PEARLS, 200, ItemEquipType.CLOAK, RequirementType.ETHEREALNESS);
		public static Enchantment ENDURANCE = new Enchantment("Endurance", Items.CORLY_ROOT, 200, ItemEquipType.CLOAK | ItemEquipType.RING, RequirementType.ENDURANCE);
		//HOLY/UNHOLY resist

		//weapon
		public static Enchantment BRIGHT = new Enchantment("Bright", Items.STARDUST, 200, ItemEquipType.RING | ItemEquipType.WEAPON, RequirementType.LIGHT);
		public static Enchantment ENHANCEMENT = new Enchantment("Enhancement", Items.DIAMONDS, 200, ItemEquipType.WEAPON | ItemEquipType.RANGED | ItemEquipType.ARMOR | ItemEquipType.SHIELD, 0).setStacksTo(5);
		public static Enchantment DISRUPTION = new Enchantment("Disruption", Items.UNICORN_HAIR, 200, ItemEquipType.WEAPON, RequirementType.DISRUPTION);

		public static Enchantment POISON_DMG = new Enchantment("Poisonous", Items.WOLFSBANE, 200, ItemEquipType.WEAPON | ItemEquipType.RANGED, RequirementType.POISON_DAMAGE);
		public static Enchantment FIRE_DMG = new Enchantment("Flaming", Items.HELLSPARKS, 200, ItemEquipType.WEAPON | ItemEquipType.RANGED, RequirementType.FIRE_DAMAGE);
		public static Enchantment COLD_DMG = new Enchantment("Frosted", Items.HOARFROST, 200, ItemEquipType.WEAPON | ItemEquipType.RANGED, RequirementType.COLD_DAMAGE);
		public static Enchantment ACID_DMG = new Enchantment("Acidic", Items.SLIME_GOO, 200, ItemEquipType.WEAPON | ItemEquipType.RANGED, RequirementType.ACID_DAMAGE);

		public static Enchantment VORPAL = new Enchantment("Vorpal", Items.OBSIDIAN_SHARD, 200, ItemEquipType.WEAPON, RequirementType.VORPAL);
		public static Enchantment KEEN = new Enchantment("Keen", Items.TOPAZ, 200, ItemEquipType.WEAPON, 0);
		public static Enchantment BRILIANT_ENERGY = new Enchantment("Briliant Energy", Items.RED_MERCURY, 200, ItemEquipType.WEAPON, RequirementType.BRILIANT_ENERGY);

		//ranged

		//helmet
		public static Enchantment ALERTNESS = new Enchantment("Alertness", Items.CYCLOPS_EYE, 200, ItemEquipType.HELMET | ItemEquipType.RING, RequirementType.DETECTION);
		public static Enchantment DETERMINATION = new Enchantment("Determination", Items.DRAGON_TEARS, 200, ItemEquipType.HELMET | ItemEquipType.MISC | ItemEquipType.SHIELD, RequirementType.FIRM_RESOLVE);
		public static Enchantment DANGER_SENSE = new Enchantment("Danger Sense", Items.GOLDEN_SILK, 200, ItemEquipType.HELMET, RequirementType.DANGER_SENSE);
		public static Enchantment MIND_SHIELD = new Enchantment("Mind Shield", Items.BANSHEE_WAIL, 200, ItemEquipType.HELMET, RequirementType.MIND_SHIELD);
		public static Enchantment CHARISMATIC = new Enchantment("Charismatic", Industries.POT_CHARISMA.industryItem, 25000, ItemEquipType.HELMET, RequirementType.CHARISMA);
		public static Enchantment INTELLECTUAL = new Enchantment("Intellectual", Industries.POT_INTELLIGENCE.industryItem, 25000, ItemEquipType.HELMET, RequirementType.INTELLIGENCE);

		//ring
		public static Enchantment HERBALISM = new Enchantment("Herbalism", Items.GLOWING_FUNGS, 200, ItemEquipType.RING, RequirementType.HERB);
		public static Enchantment FEATHER_FALL = new Enchantment("Feather Fall", Items.GRYPHON_TECTRICES, 200, ItemEquipType.BOOTS | ItemEquipType.RING, RequirementType.FEATHER_FALL);
		public static Enchantment FREEDOM = new Enchantment("Freedom", Items.MANOWAR, 200, ItemEquipType.RING, RequirementType.FREE_MOVEMENT);
		public static Enchantment SHADOWS = new Enchantment("Shadows", Items.MIDNIGHT_SUN, 200, ItemEquipType.RING, RequirementType.STEALTH);

		//shield
		public static Enchantment MIRRORED = new Enchantment("Mirrored", Items.MOON_SLIVER, 200, ItemEquipType.SHIELD, RequirementType.MIRRORED);
		public static Enchantment GLOWING = new Enchantment("Glowing", Items.FIRE_OPAL, 200, ItemEquipType.SHIELD, RequirementType.LIGHT);

		//boots
		public static Enchantment STRONG = new Enchantment("Strong", Industries.POT_STRENGTH.industryItem, 25000, ItemEquipType.BOOTS, RequirementType.STRENGTH);
		public static Enchantment AGILE = new Enchantment("Agile", Industries.POT_AGILITY.industryItem, 5, ItemEquipType.ARMOR, RequirementType.AGILITY);

		//red mercury ~ 25-100
	}
}
