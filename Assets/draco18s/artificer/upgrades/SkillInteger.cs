using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class SkillInteger : Skill {
		protected readonly int amount;
		public SkillInteger(string name, int boost, BigInteger cost, double costIncrease) : base(name, 1, cost, costIncrease) {
			amount = boost;
		}

		new public int getMultiplier() {
			return ranks * amount;
		}

		public override string getMultiplierForDisplay() {
			return amount.ToString();
		}
	}
}
