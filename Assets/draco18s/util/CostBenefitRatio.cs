using Assets.draco18s.artificer.items;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.util {
	public class CostBenefitRatio : IComparable {
		public BigInteger cost;
		public BigInteger benefit;
		public Industry indust;

		public CostBenefitRatio(BigInteger c, BigInteger b, Industry i) {
			this.cost = c;
			this.benefit = b;
			indust = i;
		}

		public int CompareTo(object obj) {
			if(obj == null) return 1;
			if(obj is CostBenefitRatio) {
				CostBenefitRatio other = (CostBenefitRatio)obj;
				BigInteger rat1 = cost / benefit;
				BigInteger rat2 = other.cost / other.benefit;
				return rat1.CompareTo(rat2);
			}
			return 0;
		}
	}
}
