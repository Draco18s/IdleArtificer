using Assets.draco18s.config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.statistics {
	public class StatLogin : StatHighscore {
		 public override string description {
			get {
				long ll = StatisticsTracker.lastDailyLogin.value * 5;
				if(ll < 1) return Localization.translateToLocal(_description);
				DateTime lastLogin = DateTime.ParseExact(ll.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture) + TimeSpan.FromDays(1);
				return string.Format(Localization.translateToLocal(_description), lastLogin.ToString("h:mm tt, M/d/yy"));
			}
		}
		public StatLogin(string name) : base(name) {
		}
	}
}
