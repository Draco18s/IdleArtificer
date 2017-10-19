using System;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.draco18s.artificer.statistics {
	public class StatBase : IStat {
		private string _statName;
		private string _description;
		public string statName {
			get { return _statName; }
			set { _statName = value; }
		}
		public string description {
			get { return _description; }
			set { _description = value; }
		}
		public readonly bool shouldResetOnNewLevel;
		protected bool shouldReadAsFloat = false;
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

		public object serializedValue {
			get {
				return value;
			}
		}

		public bool isHidden { get; set; }

		public virtual string getDisplay() {
			return "" + (shouldReadAsFloat ? floatValue : value);
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
			shouldReadAsFloat = true;
			addValue(Mathf.RoundToInt(v * 10000));
		}

		public virtual void setValue(int v) {
			statValue = v;
		}

		public virtual void setValue(object v) {
			if(v is int)
				statValue = (int)v;
		}

		public virtual void resetValue() {
			statValue = 0;
		}

		public StatBase setHidden() {
			isHidden = true;
			return this;
		}

		public StatBase register() {
			StatisticsTracker.register(this);
			return this;
		}

		public bool isGreaterThan(object v) {
			if(v is int) {
				return value >= (int)v;
			}
			if(v is float) {
				return floatValue >= (float)v;
			}
			return false;
		}
	}
}