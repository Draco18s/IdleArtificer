using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System.Collections.Generic;

namespace Assets.draco18s.artificer.items {
	public class Item {
		public int ID;
		public readonly string name;
		public bool isConsumable = true;
		public bool isViableRelic = false;
		public readonly Industry industry;
		public int minStackSize;
		public int maxStackSize;
		public bool canBeGivenToQuests = true;
		public ItemEquipType equipType;

		protected RequirementType reqProperties;
		protected AidType aidProperties;
		protected int questSize = 1;
		protected float effectivenessMultiplier = 1;

		public Item(string name) {
			this.name = name;
			GameRegistry.RegisterItem(this);
		}

		public Item(Industry indust) {
			industry = indust;
			name = indust.name;
			//GameRegistry.RegisterItem(this);
		}

		public Item addReqType(RequirementType type) {
			reqProperties |= type;
			return this;
		}

		public Item addAidType(AidType type) {
			aidProperties |= type;
			return this;
		}
		public Item setStackSizeForQuest(int size) {
			questSize = size;
			return this;
		}

		public int getStackSizeForQuest() {
			return questSize;
		}

		public bool hasReqType(RequirementType type) {
			return (reqProperties & type) > 0;
		}

		public bool hasAidType(AidType type) {
			return (aidProperties & type) > 0;
		}

		public RequirementType getAllReqs() {
			return reqProperties;
		}

		public float getEffectiveness() {
			return effectivenessMultiplier;
		}

		public AidType getAllAids() {
			return aidProperties;
		}

		public Item setRandomSize(int min, int max) {
			minStackSize = min;
			maxStackSize = max;
			return this;
		}

		public Item setDisallowedForQuests() {
			canBeGivenToQuests = false;
			return this;
		}

		public Item setConsumable(bool val) {
			isConsumable = val;
			return this;
		}

		public Item setEquipType(ItemEquipType type) {
			equipType = type;
			return this;
		}

		public Item setIsViableRelic(bool val) {
			isViableRelic = val;
			return this;
		}

		public Item setEffectiveness(float val) {
			effectivenessMultiplier = val;
			return this;
		}
	}
}