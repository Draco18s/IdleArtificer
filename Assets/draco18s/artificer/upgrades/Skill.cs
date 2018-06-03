using Assets.draco18s.artificer.init;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.upgrades {
	public class Skill {
		public readonly string name;
		public readonly string description;
		public GameObject guiItem;

		protected int ranks;
		protected readonly double multiplier;
		protected readonly BigInteger baseCost;
		protected readonly double costMultiplier;

		private int lastN;
		private int lastranks = -1;
		private BigRational lastTotal;
		private double powLastranks;
		private double pow1;
		private double pow10;
		private double pow50;

		public Skill(string skillName, double multi, BigInteger cost, double costIncrease) {
			name = "skill." + skillName + ".name";
			description = "skill." + skillName + ".desc";
			ranks = 0;
			multiplier = multi;
			baseCost = cost;
			costMultiplier = costIncrease;
			pow1 = costMultiplier;
			pow10 = Math.Pow(costMultiplier, 10);
			pow50 = Math.Pow(costMultiplier, 50);
		}

		public Skill register() {
			SkillList.register(this);
			return this;
		}

		public void increaseRank(int n) {
			ranks += n;
		}

		public BigRational getCost(int n) {
			Profiler.BeginSample("Recalc");
			if(ranks == lastranks && n == lastN) {
				return lastTotal;
			}
			lastN = n;
			if(ranks != lastranks) {
				lastranks = ranks;
				powLastranks = Math.Pow(costMultiplier, ranks);
			}
			Profiler.EndSample();
			Profiler.BeginSample("Return");
			BigRational b = baseCost;
			BigRational dd = new BigRational(costMultiplier);
			dd -= 1;
			//+0.51?
			lastTotal = b * ((powLastranks * ((n == 1 ? pow1 : (n == 10 ? pow10 : (n == 50 ? pow50 : BigRational.Pow(costMultiplier, n)))) - 1) / (dd)));
			Profiler.EndSample();
			return lastTotal;
		}

		internal int getRanks() {
			return ranks;
		}

		internal void setRanks(int v) {
			ranks = v;
		}

		public double getMultiplier() {
			return ranks * multiplier;
		}

		public virtual string getMultiplierForDisplay() {
			return Mathf.RoundToInt((float)multiplier*100).ToString();
		}
	}
}
