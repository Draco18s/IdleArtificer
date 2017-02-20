using System;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.draco18s.artificer.statistics {
	public class StatBase {
		public readonly string statName;
		public readonly string description;
		public readonly bool shouldResetOnNewLevel;

		public virtual int value {
			get {
				return statValue;
			}
			set {

			}
		}
		public virtual float floatValue {
			get {
				return statValue / 10000f;
			}
			set {

			}
		}
		public int ID {
			get {
				return idVal;
			}
			set {
				if(idVal == -1) {
					idVal = value;
				}
			}
		}

		protected int statValue;
		protected int idVal = -1;

		public StatBase(string name) {
			statName = "stat." + name + ".name";
			description = "stat." + name + ".desc";
			statValue = 0;
			shouldResetOnNewLevel = false;
		}

		protected StatBase(string name, bool resets):this(name) {
			shouldResetOnNewLevel = resets;
		}

		/// <summary>
		/// Updates the statistic, handles checking if the game is in Editor testing mode.
		/// </summary>
		/// <param name="v">Amount to add</param>
		public virtual void addValue(int v) {
			statValue += v;
		}

		public virtual void addValue(float v) {
			addValue(Mathf.RoundToInt(v * 10000));
		}

		public virtual void setValue(int v) {

		}

		public virtual void resetValue() {
			statValue = 0;
		}

		public StatBase register() {
			StatisticsTracker.register(this);
			return this;
		}
	}
}