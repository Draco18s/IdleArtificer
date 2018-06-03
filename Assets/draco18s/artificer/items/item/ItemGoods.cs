using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.items {
	public class ItemGoods : Item {

		public ItemGoods():base("miscelaneous_goods") {
		}

		public override bool isSpecial() {
			return true;
		}

		public override BigInteger getBaseValue() {
			return 0;
		}
	}
}
