using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Assets.draco18s.artificer.statistics {
	public class StatResetable : StatBase {
		protected int bestValue;
		private int initialValue = 0;

		public StatResetable(string name):base(name, EnumResetType.SHOP) {
		}


		public StatResetable(string name, EnumResetType resets) : base(name, resets) {
		}

		public StatResetable setInitialValue(int v) {
			statValue = bestValue = initialValue = v;
			return this;
		}

		public override void resetValue() {
			if(statValue > bestValue) bestValue = statValue;
			statValue = initialValue;
		}
	}
}
