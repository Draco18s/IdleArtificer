using Assets.draco18s.artificer.items;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public interface IQuestGoal : IRelicMaker {
		int getNumTotalEncounters();
	}
}