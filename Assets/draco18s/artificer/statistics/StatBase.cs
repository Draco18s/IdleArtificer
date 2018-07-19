using Assets.draco18s.config;
using System;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.draco18s.artificer.statistics {
	public class StatBase : IStat {
		protected string _statName;
		protected string _description;
		public virtual string statName {
			get { return _statName; }
		}
		public virtual string description {
			get { return Localization.translateToLocal(_description); }
		}
		public readonly bool _shouldResetOnNewLevel;
		public readonly bool _shouldResetOnNewGuildmaster;
		protected bool shouldReadAsFloat = false;
		public virtual int value {
			get {
				return statValue;
			}
		}
		public virtual float floatValue {
			get {
				return statValue / 10000f;
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

		public virtual object serializedValue {
			get {
				return value;
			}
		}

		public bool isHidden { get; set; }

		public bool shouldResetOnNewLevel {
			get {
				return _shouldResetOnNewLevel;
			}
		}
		public bool shouldResetOnNewGuildmaster {
			get {
				return _shouldResetOnNewGuildmaster;
			}
		}

		public virtual string getDisplay() {
			return (shouldReadAsFloat ? floatValue.ToString("N3",Configuration.NumberFormat) : value.ToString("N0", Configuration.NumberFormat));
		}

		protected int statValue;
		protected int idVal = -1;

		public StatBase(string name) {
			_statName = "stat." + name + ".name";
			_description = "stat." + name + ".desc";
			statValue = 0;
			_shouldResetOnNewLevel = false;
			_shouldResetOnNewGuildmaster = false;
		}

		protected StatBase(string name, EnumResetType resets):this(name) {
			if((resets & EnumResetType.SHOP) > 0)
				_shouldResetOnNewLevel = true;
			if((resets & EnumResetType.GUILDMASTER) > 0)
				_shouldResetOnNewGuildmaster = true;
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