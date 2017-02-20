using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.draco18s.util;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.game;
using System.Runtime.Serialization;

namespace Assets.draco18s.artificer.items {
	[Serializable]
	public class Industry : ISerializable {
		public int ID;
		
		public readonly BigInteger cost;
		protected readonly BigInteger value;
		public readonly List<IndustryInput> inputs;
		public readonly int output;
		public readonly String name;

		public int level;
		public BigInteger quantityStored = new BigInteger(0);
		public int apprentices;
		//public bool isSellingOutput = true;
		public bool isSellingStores = false;
		public bool isConsumersHalted = false;
		public bool isProductionHalted = false;
		public bool doAutobuild = false;
		public int autoBuildLevel = 0;
		public int autoBuildMagnitude;
		protected int vendors;
		protected float timeRemaining;
		protected int halvesAndDoubles = 1;
		
		[NonSerialized]
		public readonly Scalar productType;
		[NonSerialized]
		public readonly Item industryItem;

		public int consumeAmount = 0;
		public int didComplete = 0;
		[NonSerialized]
		public GameObject listObj;
		[NonSerialized]
		public GameObject questInvenListObj;
		[NonSerialized]
		public GameObject enchantInvenListObj;
		[NonSerialized]
		public GameObject guiObj;

		[NonSerialized]
		protected RequirementType reqProperties;
		[NonSerialized]
		protected AidType aidProperties;
		protected bool isConsumableItem;
		protected int maxStack = 20;
		protected int questSize = 20;
		protected bool isViableRelic = false;
		protected float valueMulti = 1;
		protected float vendorSizeMulti = 1;
		protected Vector3 gridPos = Vector3.zero;

		public Industry(String name, BigInteger price, BigInteger sell, int outnum, Scalar scale) {
			this.name = name;
			cost = price;
			value = sell;
			output = outnum;
			productType = scale;
			inputs = new List<IndustryInput>();
			GameRegistry.RegisterIndustry(this);
			industryItem = new Item(this);
			industryItem.isConsumable = this.isConsumableItem;
		}

		public Industry(String name, BigInteger price, BigInteger sell, int outnum, Scalar scale, params IndustryInput[] list) {
			this.name = name;
			cost = price;
			value = sell;
			output = outnum;
			productType = scale;
			inputs = new List<IndustryInput>();
			foreach(IndustryInput i in list) {
				inputs.Add(i);
			}
			GameRegistry.RegisterIndustry(this);
			industryItem = new Item(this);
			industryItem.isConsumable = this.isConsumableItem;
		}

		public virtual BigInteger GetScaledCost() {
			return Math.Pow(productType.amount, level) * (cost * halvesAndDoubles);
		}

		public virtual BigInteger GetScaledCost(int n) {
			BigInteger b = (cost * halvesAndDoubles);
			return ((Math.Pow(productType.amount, level) * (Math.Pow(productType.amount, n) - 1)) / (productType.amount - 1)) * b;
		}

		public virtual BigInteger ProduceOutput() {
			return output * level;
		}

		public virtual BigInteger GetBaseSellValue() {
			return value * halvesAndDoubles;
		}

		public void addTime(float t) {
			timeRemaining += (t * halvesAndDoubles);
		}

		public void tickApprentices() {
			timeRemaining -= apprentices * Main.instance.GetClickRate();
		}

		public void addTimeRaw(float t) {
			timeRemaining += t;
		}

		public void setTimeRemaining(float v) {
			timeRemaining = v;
		}

		public float getTimeRemaining() {
			return timeRemaining;
		}

		public void halveAndDouble(int v) {
			if(v > 0) {
				bool wasOdd = (level % 2 == 1);
				halvesAndDoubles = halvesAndDoubles << 1;
				level = level >> 1;
				if(wasOdd) level += 1;
			}
			else {
				halvesAndDoubles = halvesAndDoubles >> 1;
				level = level << 1;
			}
		}

		public int getHalveAndDouble() {
			return halvesAndDoubles;
		}

		public virtual BigInteger GetSellValue() {
			//BigInteger sell = GetBaseSellValue() * Main.instance.GetSellMultiplier();
			//BigInteger frac = Main.instance.GetSellMultiplierMicro() * GetBaseSellValue();
			//return sell + frac;
			BigInteger frac = Main.instance.GetSellMultiplierMicro() * valueMulti * GetBaseSellValue();
			BigInteger ret = (valueMulti * GetBaseSellValue() * Main.instance.GetSellMultiplier()) + frac;
			//if(ret < 1) ret = 1;
			return ret;
		}

		public Industry addReqType(RequirementType type) {
			reqProperties |= type;
			industryItem.addReqType(type);
			return this;
		}

		public Industry addAidType(AidType type) {
			aidProperties |= type;
			industryItem.addAidType(type);

			if((type & (AidType.HEALING_LARGE | AidType.HEALING_MEDIUM | AidType.HEALING_SMALL | AidType.HEALING_TINY)) > 0) {
				this.addReqType(RequirementType.HEALING);
			}
			if((type & (AidType.MANA_LARGE | AidType.MANA_MEDIUM | AidType.MANA_SMALL | AidType.MANA_TINY)) > 0) {
				this.addReqType(RequirementType.MANA);
			}
			if((type & AidType.WEAPON) > 0) {
				this.addReqType(RequirementType.WEAPON);
			}
			if((type & (AidType.LIGHT_ARMOR | AidType.MEDIUM_ARMOR | AidType.HEAVY_ARMOR | AidType.BARKSKIN | AidType.LIGHT_SHIELD | AidType.HEAVY_SHIELD)) > 0) {
				this.addReqType(RequirementType.ARMOR);
			}
			return this;
		}

		public Industry setConsumable(bool val) {
			isConsumableItem = val;
			industryItem.isConsumable = this.isConsumableItem;
			return this;
		}

		public Industry setIsViableRelic(bool val) {
			industryItem.isViableRelic = val;
			isViableRelic = val;
			return this;
		}

		public Industry setMaxStackSize(int size) {
			maxStack = size;
			return this;
		}

		public Industry setStackSizeForQuest(int size) {
			questSize = size;
			return this;
		}

		public Industry setVendorSizeMulti(float v) {
			vendorSizeMulti = v;
			return this;
		}

		public int getVendors() {
			return Mathf.RoundToInt(vendors * vendorSizeMulti);
		}

		public int getRawVendors() {
			return vendors;
		}

		public void AdjustVendors(int v) {
			vendors = v;
		}

		public int getMaxStackSize() {
			return maxStack;
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

		public Industry setDisallowedForQuests() {
			industryItem.canBeGivenToQuests = false;
			return this;
		}

		public Industry setEquipType(ItemEquipType type) {
			industryItem.setEquipType(type);
			return this;
		}

		public void addValueMultiplier(float multiplier) {
			valueMulti *= multiplier;
		}
		
		public Vector3 getGridPos() {
			return gridPos;
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("level", level);
			info.AddValue("quantityStored", quantityStored.ToString());
			info.AddValue("isSellingStores", isSellingStores);
			info.AddValue("isConsumersHalted", isConsumersHalted);
			info.AddValue("isProductionHalted", isProductionHalted);
			info.AddValue("doAutobuild", doAutobuild);
			info.AddValue("autoBuildLevel", autoBuildLevel);
			info.AddValue("autoBuildMagnitude", autoBuildMagnitude);
			info.AddValue("apprentices", apprentices);
			info.AddValue("vendors", vendors);
			info.AddValue("timeRemaining", timeRemaining);
			info.AddValue("halvesAndDoubles", halvesAndDoubles);
			Vector3 p = guiObj.transform.GetChild(0).localPosition;
			info.AddValue("posX", p.x);
			info.AddValue("posY", p.y);
		}
		public Industry(SerializationInfo info, StreamingContext context) {
			level = info.GetInt32("level");
			quantityStored = new BigInteger(info.GetString("quantityStored"));
			isSellingStores = info.GetBoolean("isSellingStores");
			isConsumersHalted = info.GetBoolean("isConsumersHalted");
			isProductionHalted = info.GetBoolean("isProductionHalted");
			doAutobuild = info.GetBoolean("doAutobuild");
			autoBuildLevel = info.GetInt32("autoBuildLevel");
			if(Main.saveVersionFromDisk >= 3) {
				autoBuildMagnitude = info.GetInt32("autoBuildMagnitude");
			}
			apprentices = info.GetInt32("apprentices");
			vendors = info.GetInt32("vendors");
			timeRemaining = (float)info.GetDouble("timeRemaining");
			halvesAndDoubles = info.GetInt32("halvesAndDoubles");
			gridPos = new Vector3((float)info.GetDouble("posX"), (float)info.GetDouble("posY"), 0);
		}

		public void ReadFromCopy(Industry copy) {
			level = copy.level;
			quantityStored = copy.quantityStored;
			if(quantityStored == null) quantityStored = 0;
			isSellingStores = copy.isSellingStores;
			isConsumersHalted = copy.isConsumersHalted;
			isProductionHalted = copy.isProductionHalted;
			doAutobuild = copy.doAutobuild;
			autoBuildLevel = copy.autoBuildLevel;
			autoBuildMagnitude = copy.autoBuildMagnitude;
			apprentices = copy.apprentices;
			vendors = copy.vendors;
			timeRemaining = copy.timeRemaining;
			halvesAndDoubles = copy.halvesAndDoubles;
			gridPos = MathHelper.snap(copy.gridPos,24);
		}
	}
}
 