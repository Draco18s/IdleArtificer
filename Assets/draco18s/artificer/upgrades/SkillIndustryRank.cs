using Assets.draco18s.artificer.items;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class SkillIndustryRank :Skill {
		protected readonly Scalar rankType;
		public SkillIndustryRank(string name, double multi, BigInteger cost, double costIncrease, Scalar type) :base(name, multi, cost, costIncrease) {
			rankType = type;
		}

		public double getMultiplier(Scalar type) {
			if(rankType == type)
				return ranks * multiplier;
			return 0;
		}
	}
}
