using System;

namespace Assets.draco18s.artificer.statistics {
	[Flags]
	public enum EnumResetType {
		NONE = 0,
		MANUAL = 1<<0,
		SHOP = 1<<1,
		GUILDMASTER = 1<<2
	}
}