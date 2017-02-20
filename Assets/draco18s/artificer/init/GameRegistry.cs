using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public class GameRegistry {
		private static List<Item> allItems = new List<Item>();
		private static List<Industry> allIndustries = new List<Industry>();
		private static Dictionary<string, ObstacleType> allObstacles = new Dictionary<string, ObstacleType>();
		public static List<Enchantment> allEnchants = new List<Enchantment>();
		private static Dictionary<Item, Enchantment> itemEnchantMapping = new Dictionary<Item, Enchantment>();

		public static void Reset() {
			allItems = new List<Item>();
			allIndustries = new List<Industry>();
			allObstacles = new Dictionary<string, ObstacleType>();
		}

		public static void RegisterItem(Item i) {
			i.ID = allItems.Count();
			allItems.Add(i);
		}

		internal static void RegisterEnchantment(Enchantment e) {
			e.ID = allEnchants.Count();
			allEnchants.Add(e);
			itemEnchantMapping.Add(e.ingredient, e);
		}

		public static void RegisterIndustry(Industry i) {
			i.ID = allIndustries.Count();
			allIndustries.Add(i);
		}

		public static void RegisterObstacle(ObstacleType i) {
			allObstacles.Add(i.name, i);
		}

		public static Item GetItemByID(int i) {
			return allItems[i];
		}
		public static Industry GetIndustryByID(int i) {
			return allIndustries[i];
		}
		public static ObstacleType GetObstacleByID(string i) {
			ObstacleType obs;
			allObstacles.TryGetValue(i, out obs);
			return obs;
		}

		public static Enchantment GetEnchantmentByID(int i) {
			return allEnchants[i];
		}

		public static Enchantment GetEnchantmentByItem(Item i) {
			Enchantment en;
			itemEnchantMapping.TryGetValue(i, out en);
			return en;
		}
	}
}
