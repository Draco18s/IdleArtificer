using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.draco18s.util;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.game;
using System.Runtime.Serialization;
using Koopakiller.Numerics;

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
		public int bonusLevel = 0;
		protected int vendors;
		protected float timeRemaining;
		protected int halvesAndDoubles = 1;

		[NonSerialized]
		public readonly Scalar productType;
		[NonSerialized]
		public readonly Item industryItem;
		[NonSerialized]
		public Industries.IndustryTypesEnum industryType = Industries.IndustryTypesEnum.NONE;

		public int consumeAmount = 0;
		public int didComplete = 0;
		/*[NonSerialized]
		public GameObject listObj;
		[NonSerialized]
		public GameObject questInvenListObj;
		[NonSerialized]
		public GameObject enchantInvenListObj;*/
		[NonSerialized]
		public GameObject craftingGridGO;

		[NonSerialized]
		protected RequirementType reqProperties;
		[NonSerialized]
		protected AidType aidProperties;
		protected bool isConsumableItem = false;
		protected int maxStack = 20;
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
			industryItem.isConsumable = isConsumableItem;
			pow50 = Math.Pow(productType.amount, 50);
			pow10 = Math.Pow(productType.amount, 10);
			pow1 = productType.amount;
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
			industryItem.isConsumable = isConsumableItem;
			pow50 = Math.Pow(productType.amount, 50);
			pow10 = Math.Pow(productType.amount, 10);
			pow1 = productType.amount;
		}

		private double pow1;
		private double pow10;
		private double pow50;
		private int lastLevel = -1;
		private double powLastLevel = -1;
		private int lastN = -1;
		private BigRational lastTotal = -1;

		public virtual BigRational GetScaledCost() {
			/*BigRational c = cost * halvesAndDoubles;
			c = (BigRational.Pow(productType.amount, level) * c);
			powCurLevel = new BigRational(c); //ensure cloning
			return c;*/
			return GetScaledCost(1);
		}

		public virtual BigRational GetScaledCost(int n) {
			Profiler.BeginSample("Recalc");
			if(level == lastLevel && n == lastN) {
				return lastTotal;
			}
			lastN = n;
			if(level != lastLevel) {
				lastLevel = level;
				powLastLevel = Math.Pow(productType.amount, level);
			}
			Profiler.EndSample();
			Profiler.BeginSample("Return");
			BigRational b = (cost * halvesAndDoubles);
			BigRational dd = new BigRational(productType.amount);
			dd -= 1;
			//+0.51?
			lastTotal = b * ((powLastLevel * ((n == 1 ? pow1 : (n == 10 ? pow10 : (n == 50 ? pow50 : BigRational.Pow(productType.amount, n)))) - 1) / (dd)));
			Profiler.EndSample();
			return lastTotal;
		}

		public virtual BigInteger ProduceOutput() {
			return output * (level + bonusLevel);
		}

		public virtual BigInteger GetBaseSellValue() {
			return value;
		}

		/// <summary>
		/// Returns true if needed synchro this tick
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public bool addTime(float t) {
			bool ret = false;
			if(CraftingManager.doSynchronize && apprentices == 0) {
				int intTime = Mathf.FloorToInt(Time.time*Main.instance.GetSpeedMultiplier() * halvesAndDoubles);
				float synchTime = (intTime % 10) + (Time.time * Main.instance.GetSpeedMultiplier() * halvesAndDoubles) - intTime;

				int diff = Mathf.Abs((100 - Mathf.RoundToInt(timeRemaining * 10)) - Mathf.RoundToInt(synchTime * 10));

				if(diff > 1) {
					t *= 0.25f;
					ret = true;
					//Debug.Log(name + ": still syncing (big) " + (100 - Mathf.RoundToInt(timeRemaining * 10)) + "~" + Mathf.RoundToInt(synchTime*10));
				}
				else if(diff > 0) {
					if((100 - Mathf.RoundToInt(timeRemaining * 10)) > Mathf.RoundToInt(synchTime * 10)) {
						t *= 0.95f;
					}
					else {
						t *= 1.05f;
					}
					ret = true;
					//Debug.Log(name + ": still syncing (micro) " + (100 - Mathf.RoundToInt(timeRemaining * 10)) + "~" + Mathf.RoundToInt(synchTime * 10));
				}
				/*if(100 - Mathf.RoundToInt(timeRemaining * 10) > Mathf.RoundToInt(synchTime *10)) {
					t *= 0.5f;
					ret = true;
					Debug.Log(name + ": still syncing (fast)" + (100 - Mathf.RoundToInt(timeRemaining * 10)) + "~" + Mathf.RoundToInt(synchTime*10));
				}*/
				/*synchTime = 100 - Mathf.RoundToInt(synchTime * 10);
				if(Mathf.RoundToInt(timeRemaining * 10) > synchTime) {
					t *= 2;
					ret = true;
				}
				else if(Mathf.RoundToInt(timeRemaining * 10) < synchTime) {
					t /= 2;
					ret = true;
				}*/
			}
			timeRemaining += (t * halvesAndDoubles);
			return ret;
		}

		public void tickApprentices() {
			timeRemaining -= apprentices * Main.instance.GetClickRate() * Main.instance.player.currentGuildmaster.apprenticeRateMultiplier();
		}

		public void addTimeRaw(float t) {
			if(timeRemaining > float.MinValue)
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

		public int getTotalLevel() {
			return level + bonusLevel;
		}

		public virtual BigRational GetSellValue() {
			//BigRational sell = ;
			return Main.instance.GetSellMultiplierFull() * valueMulti * (BigRational)GetBaseSellValue() * Main.instance.player.currentGuildmaster.industryTypeMultiplier(industryType) * SkillList.getScalarTypeMulti(productType);
			//BigInteger frac = (BigInteger)(Main.instance.GetSellMultiplierMicro() * valueMulti * (BigRational)GetBaseSellValue());
			//BigInteger ret = (BigInteger)(valueMulti * (BigRational)GetBaseSellValue() * Main.instance.GetSellMultiplier()) + frac;
			//return ret;
		}

		public Industry setIndustryType(Industries.IndustryTypesEnum type) {
			industryType = type;
			return this;
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
			if((type & AidType.RANGED_WEAPON) > 0) {
				this.addReqType(RequirementType.RANGED);
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
			industryItem.setStackSizeForQuest(size);
			return this;
		}

		public Industry setVendorSizeMulti(float v) {
			vendorSizeMulti = v;
			return this;
		}

		public int getVendors() {
			return Mathf.RoundToInt(vendors * vendorSizeMulti);
		}

		public int getOneVendor() {
			return Mathf.RoundToInt(1 * vendorSizeMulti);
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
			return industryItem.getStackSizeForQuest();
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

		public Industry setEffectiveness(float val) {
			industryItem.setEffectiveness(val);
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
			info.AddValue("bonusLevel", bonusLevel);
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
			Vector3 p = craftingGridGO.transform.GetChild(0).localPosition;
			info.AddValue("posX", p.x);
			info.AddValue("posY", p.y);
		}
		public Industry(SerializationInfo info, StreamingContext context) {
			level = info.GetInt32("level");
			quantityStored = BigInteger.Parse(info.GetString("quantityStored"));
			isSellingStores = info.GetBoolean("isSellingStores");
			isConsumersHalted = info.GetBoolean("isConsumersHalted");
			isProductionHalted = info.GetBoolean("isProductionHalted");
			doAutobuild = info.GetBoolean("doAutobuild");
			autoBuildLevel = info.GetInt32("autoBuildLevel");
			if(Main.saveVersionFromDisk >= 3) {
				autoBuildMagnitude = info.GetInt32("autoBuildMagnitude");
			}
			if(Main.saveVersionFromDisk >= 6) {
				bonusLevel = info.GetInt32("bonusLevel");
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
			//halvesAndDoubles = copy.halvesAndDoubles; //applied via upgrade deserialization
			gridPos = MathHelper.snap(copy.gridPos,24);
		}
	}
}
 