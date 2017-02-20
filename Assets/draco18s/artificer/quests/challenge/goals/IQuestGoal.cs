using Assets.draco18s.artificer.items;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public interface IQuestGoal {
		string relicNames(ItemStack stack);
		string relicDescription(ItemStack stack);
		int getNumTotalEncounters();
	}
}