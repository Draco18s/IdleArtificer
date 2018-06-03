using System;

namespace Assets.draco18s.artificer.statistics {
	[Flags]
	public enum EnumResetType {
		NONE = 0,
		SHOP = 1 << 0,
		GUILDMASTER = 1 << 1
	}
}