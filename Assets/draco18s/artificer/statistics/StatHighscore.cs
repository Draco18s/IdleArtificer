using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.statistics {
	class StatHighscore : StatBase {
		protected int bestValue;
		private int initialValue = 0;
		public override int value {
			get {
				return bestValue;
			}
			set {

			}
		}

		public override float floatValue {
			get {
				return bestValue / 10000f;
			}
			set {

			}
		}

		public StatHighscore(string name):base(name, true) {
		}

		public override void addValue(int v) {
			statValue += v;
			if(statValue > bestValue) bestValue = statValue;
		}

		public override void setValue(int v) {
			statValue = v;
			if(statValue > bestValue) bestValue = statValue;
		}

		public override void resetValue() {
			if(statValue > bestValue) bestValue = statValue;
			statValue = initialValue;
		}
	}
}
