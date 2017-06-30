using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public class Items {
		public static Item ROOSTER_TEETH = new Item("rooster_teeth").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item GLOWING_FUNGS = new Item("glowing_fungus").setRandomSize(4, 6).setConsumable(true).addReqType(RequirementType.HERB).setStackSizeForQuest(5);//
		public static Item DIATOM_EARTH = new Item("diatomaceous_earth").setRandomSize(4, 6).setConsumable(true).addReqType(RequirementType.ACID_IMMUNE).setStackSizeForQuest(5).setEffectiveness(0.5f);//
		public static Item WOLFSBANE = new Item("wolfsbane").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item OBSIDIAN_SHARD = new Item("obsidian_shards").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item ROYAL_JELLY = new Item("royal_jelly").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item HOARFROST = new Item("hoarfrost").setRandomSize(4, 6).setConsumable(true).addReqType(RequirementType.COLD_DAMAGE).setStackSizeForQuest(10);//consumable item for cold damage?
		public static Item MIDNIGHT_SUN = new Item("midnight_sun").setRandomSize(6, 9).setDisallowedForQuests();//
		public static Item CORLY_ROOT = new Item("corly_root").setRandomSize(6, 9).setConsumable(true).addReqType(RequirementType.POISON_IMMUNE).addReqType(RequirementType.HERB).setStackSizeForQuest(5).setEffectiveness(0.5f);
		public static Item GLASS_LENS = new Item("glass_lens").setRandomSize(5, 7).setDisallowedForQuests();//

		public static Item TANGLED_ROSE = new Item("tangled_rose").setRandomSize(5, 7).setDisallowedForQuests();//
		public static Item STARDUST = new Item("stardust").setRandomSize(5, 7).setDisallowedForQuests();//
		public static Item DIAMONDS = new Item("diamonds").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item VENOM = new Item("venom").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item DRAGON_SCALES = new Item("dragon_scales").setRandomSize(4, 8).setDisallowedForQuests();//
		public static Item MUMMY_WRAPPING = new Item("mummy_wrappings").setRandomSize(4, 8).setDisallowedForQuests();//
		public static Item UNICORN_HAIR = new Item("unicorn_mane").setRandomSize(4, 8).setDisallowedForQuests();//
		public static Item HELLSPARKS = new Item("hellsparks").setRandomSize(4, 8).setConsumable(true).addReqType(RequirementType.FIRE_DAMAGE).setStackSizeForQuest(10);
		public static Item SLIME_GOO = new Item("slime_goo").setRandomSize(4, 6).setConsumable(true).addReqType(RequirementType.ACID_DAMAGE).setStackSizeForQuest(10);//
		public static Item TOPAZ = new Item("topaz").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item FIRE_OPAL = new Item("fire_opal").setRandomSize(4, 6).setDisallowedForQuests();//
		public static Item LOADSTONE = new Item("loadstone").setRandomSize(4, 6).setDisallowedForQuests();//

		public static Item CYCLOPS_EYE = new Item("eye_of_cyclops").setRandomSize(1, 1).setDisallowedForQuests();//
		public static Item DRAGON_TEARS = new Item("dragon_tears").setRandomSize(3, 6).setDisallowedForQuests();//
		public static Item GOLDEN_SILK = new Item("golden_silk").setRandomSize(3, 6).setDisallowedForQuests();//
		public static Item BANSHEE_WAIL = new Item("bottled_banshee_wail").setRandomSize(3, 6).setDisallowedForQuests();//
		public static Item GRYPHON_TECTRICES = new Item("gryphon_tectrices").setRandomSize(3, 6).setDisallowedForQuests();//
		public static Item MANOWAR = new Item("Manowar").setRandomSize(3, 6).setDisallowedForQuests();//
		public static Item MOON_SLIVER = new Item("sliver_of_moonshine").setRandomSize(3, 5).setDisallowedForQuests();//
		public static Item RED_MERCURY = new Item("red_mercury").setRandomSize(3, 6).setDisallowedForQuests();//
		public static Item BLACK_PEARLS = new Item("black_pearls").setRandomSize(3, 5).setDisallowedForQuests();//
		public static Item FOURFOIL = new Item("fourfoil").setRandomSize(1, 1).setConsumable(true).addAidType(AidType.RETRY_FAILURE).setStackSizeForQuest(1);
		//new, won't drop yet

		public static Item getRandom(Random rand) {
			FieldInfo[] fields = typeof(Items).GetFields();
			return getRandom(rand, 0, fields.Length);
		}
		public static Item getRandom(Random rand, int min, int max) {
			FieldInfo[] fields = typeof(Items).GetFields();
			if(max > fields.Length) max = fields.Length;
			FieldInfo field = fields[rand.Next(max-min)+min];
			Item item = (Item)field.GetValue(null);
			return item;
		}

		private static Item[] herbs = { GLOWING_FUNGS, WOLFSBANE, CORLY_ROOT, TANGLED_ROSE };
		private static Item[] gems = { DIAMONDS, TOPAZ, BLACK_PEARLS, FIRE_OPAL, LOADSTONE, GLASS_LENS };
		private static Item[] animal = { ROOSTER_TEETH, ROYAL_JELLY, VENOM, DRAGON_SCALES, UNICORN_HAIR, SLIME_GOO, CYCLOPS_EYE, GRYPHON_TECTRICES, MANOWAR };

		public static Item getRandomType(Random rand, ItemType type) {
			switch(type) {
				case ItemType.HERB:
					return herbs[rand.Next(herbs.Length)];
				case ItemType.GEM:
					return gems[rand.Next(gems.Length)];
				case ItemType.ANIMAL:
					return animal[rand.Next(animal.Length)];
			}
			return null;
		}

		public enum ItemType {
			HERB,
			GEM,
			ANIMAL
		}
	}
}
