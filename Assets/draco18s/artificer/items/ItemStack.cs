using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.items {
	[Serializable]
	public class ItemStack : ISerializable {
		protected readonly Industry industry;
		public readonly Item item;
		public int stackSize;
		public List<RelicInfo> relicData;
		public List<Enchantment> enchants = new List<Enchantment>();
		public int antiquity = 0;
		public int numTimesUsedOnQuest = 0;
		public bool isIDedByPlayer = true;
		public bool wasAddedByJourneyman = false;
		public ItemStack(Item item, int count) {
			this.item = item;
			stackSize = count;
		}
		public ItemStack(Industry indust, int count) {
			industry = indust;
			item = industry.industryItem;
			 stackSize = count;
		}

		public void setToMaxSize() {
			if(relicData != null) {
				stackSize = 1;
			}
			else if(industry != null) {
				stackSize = industry.getMaxStackSize();
			}
			else if(!item.isConsumable) {
				stackSize = 1;
			}
			else {
				stackSize = item.getStackSizeForQuest();
			}
			numTimesUsedOnQuest = 0;
		}

		public string getDisplayName() {
			if(relicData != null) {
				RelicInfo ri = relicData.OrderByDescending(o => o.notoriety).First();
				return ri.relicName + " " + Main.ToTitleCase(item.name);
			}
			else if(enchants.Count > 0) {
				if(enchants[0] == Enchantments.ENHANCEMENT) {
					int c = 0;
					Enchantment firstNon = null;
					foreach(Enchantment ench1 in enchants) {
						if(ench1 == Enchantments.ENHANCEMENT) {
							c++;
						}
						else if(firstNon == null) {
							firstNon = ench1;
						}
					}
					return "+" + c + " " + (firstNon != null ? firstNon.name : "") + Main.ToTitleCase(item.name);
				}
				else {
					return enchants[0].name + " " + Main.ToTitleCase(item.name);
				}
			}
			return Main.ToTitleCase(item.name);
		}

		public bool doesStackHave(RequirementType type) {
			foreach(Enchantment e in enchants) {
				if((e.reqTypes & type) > 0) {
					return true;
				}
			}
			return item.hasReqType(type);
		}

		public bool doesStackHave(AidType type) {
			/*foreach(Enchantment e in enchants) {
				if((e.reqTypes & type) > 0) {
					return true;
				}
			}*/
			return item.hasAidType(type);
		}

		public bool doesStackHave(Enchantment ench) {
			foreach(Enchantment e in enchants) {
				if(e == ench) {
					return true;
				}
			}
			return false;
		}

		public int getDisplayIndex() {
			int val = 0;
			if(relicData != null) {
				int best = 0;
				foreach(RelicInfo ri in relicData) {
					best = Math.Max(best, ri.notoriety);
				}
				val += 100000 * (best+1);
			}
			else if(enchants.Count > 0) {
				int best = 0;
				foreach(Enchantment en in enchants) {
					best = Math.Max(best, en.ID);
				}
				val += 1000 * (best+1);
			}
			val += (27-(item.name[0]-96)) * 30 + (27-(item.name[1]-96));
			//UnityEngine.Debug.Log(this.getDisplayName() + " = " + val);
			return val;
		}

		public void applyEnchantment(Enchantment enchantment) {
			//if(!enchants.Contains(enchantment))
			enchants.Add(enchantment);
			enchants.Sort((x, y) => x.ID.CompareTo(y.ID));
		}

		public int enchantmentCount(Enchantment ench) {
			int f = 0;
			foreach(Enchantment e in enchants) {
				if(e == ench) {
					f++;
				}
			}
			return f;
		}

		public float getEffectiveness(RequirementType type) {
			float f = 0;
			if(item.hasReqType(type))
				f = item.getEffectiveness();
			foreach(Enchantment e in enchants) {
				if((e.reqTypes & type) > 0) {
					if(f == 0) {
						f = e.getEffectiveness();
					}
					else {
						f *= (1 + e.getEffectiveness());
					}
				}
			}
			return f;
		}

		public ItemStack split(int v) {
			if(v > stackSize) v = stackSize;
			if(v <= 0) return null;
			ItemStack newStack = clone();
			newStack.stackSize = v;
			stackSize -= v;
			return newStack;
		}

		public ItemStack clone() {
			IFormatterConverter converter = new FormatterConverter();
			SerializationInfo info = new SerializationInfo(typeof(ItemStack), converter);
			StreamingContext context = default(StreamingContext);
			this.GetObjectData(info, context);

			ItemStack newStack = new ItemStack(info, context);
			return newStack;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("is_produced", (item.industry != null));
			if(item.industry != null) {
				info.AddValue("item_id", item.industry.ID);
			}
			else {
				info.AddValue("item_id", item.ID);
			}
			info.AddValue("stacksize", stackSize);
			info.AddValue("relic", relicData != null);
			if(relicData != null) {
				info.AddValue("relicDataSize", relicData.Count);
				for(int i = 0; i < relicData.Count; i++) {
					info.AddValue("relicData_" + i, relicData[i], typeof(RelicInfo));
				}
			}
			info.AddValue("antiquity", antiquity);
			info.AddValue("numTimesUsedOnQuest", numTimesUsedOnQuest);
			info.AddValue("isIDedByPlayer", isIDedByPlayer);
			info.AddValue("enchantsSize", enchants.Count);
			for(int i = 0; i < enchants.Count; i++) {
				info.AddValue("enchants_" + i, enchants[i].ID, typeof(Enchantment));
			}
			info.AddValue("wasAddedByJourneyman", wasAddedByJourneyman);
		}

		public ItemStack(SerializationInfo info, StreamingContext context) {
			int i = info.GetInt32("item_id");
			bool b = info.GetBoolean("is_produced");
			if(b) {
				industry = GameRegistry.GetIndustryByID(i);
				item = industry.industryItem;
			}
			else {
				item = GameRegistry.GetItemByID(i);
			}
			stackSize = info.GetInt32("stacksize");
			int num;
			if(info.GetBoolean("relic")) {
				num = info.GetInt32("relicDataSize");
				relicData = new List<RelicInfo>();
				for(int o = 0; o < num; o++) {
					relicData.Add((RelicInfo)info.GetValue("relicData_" + o, typeof(RelicInfo)));
				}
			}
			antiquity = info.GetInt32("antiquity");
			numTimesUsedOnQuest = info.GetInt32("numTimesUsedOnQuest");
			isIDedByPlayer = info.GetBoolean("isIDedByPlayer");
			num = info.GetInt32("enchantsSize");
			enchants = new List<Enchantment>();
			for(int o = 0; o < num; o++) {
				enchants.Add(GameRegistry.GetEnchantmentByID(info.GetInt32("enchants_"+o)));
			}
			if(Main.saveVersionFromDisk >= 6) {
				wasAddedByJourneyman = info.GetBoolean("wasAddedByJourneyman");
			}
		}

		public RequirementType getAllReqs() {
			RequirementType ty = item.getAllReqs();
			foreach(Enchantment en in enchants) {
				ty |= en.reqTypes;
			}
			return ty;
		}

		public AidType getAllAids() {
			AidType ty = item.getAllAids();
			/*foreach(Enchantment en in enchants) {
				ty |= en.reqTypes;
			}*/
			return ty;
		}
	}
}
