using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.upgrades {
	public class SkillIndustryType : Skill {
		protected readonly Industries.IndustryTypesEnum affectedType;
		public SkillIndustryType(string name, double multi, BigInteger cost, double costIncrease, Industries.IndustryTypesEnum type) : base(name, multi, cost, costIncrease) {
			affectedType = type;
		}

		public double getMultiplier(Industries.IndustryTypesEnum type) {
			if(affectedType == type)
				return ranks * multiplier;
			return 1;
		}
	}
}
