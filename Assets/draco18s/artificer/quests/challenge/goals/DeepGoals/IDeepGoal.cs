using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	public interface IDeepGoal {
		string name { get; }
		string description { get; }
		//IQuestGoal questType { get; }
		T getQuestType<T>() where T : ObstacleType, IQuestGoal;
		void serialize(ref SerializationInfo info, ref StreamingContext context);
		void deserialize(Hashtable info);

		void onSuccessfulQuest(Quest theQuest);
		void onFailedQuest(Quest theQuest);

		bool isActive();
	}
}
