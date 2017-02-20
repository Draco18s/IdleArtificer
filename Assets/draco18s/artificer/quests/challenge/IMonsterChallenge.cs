using Assets.draco18s.artificer.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public interface IMonsterChallenge {
		int getMonsterTotalHealth();
		int getRangedDamage(EnumResult result, Quest theQuest, ref int questBonus, ItemStack rangedItem);
		int getDamageDealtToMonster(EnumResult result, Quest theQuest, ref int questBonus, ItemStack meleeItem);
		void getLootDrops(EnumResult result, Quest theQuest, ref int questBonus);
	}
}
