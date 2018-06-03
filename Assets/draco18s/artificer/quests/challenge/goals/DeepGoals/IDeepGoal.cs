using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.upgrades;
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
		IDeepGoal register();
		ObstacleType getQuestType();
		void finalizeQuest(ref Quest theQuest);

		void onSuccessfulQuest(Quest theQuest);
		void onFailedQuest(Quest theQuest);

		bool isActive();

		//UpgradeValueWrapper getModifier(UpgradeType type);

		float getSpeedModifier(Industry industry);
		float getValuedModifier(Industry industry);
		void serialize(ref SerializationInfo info, ref StreamingContext context);
		void deserialize(Hashtable info);
		int minQuestDifficulty();
		void modifyQuest(Quest theQuest);
	}
}
