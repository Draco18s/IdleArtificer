using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	public class CursedWood : IDeepGoal {
		protected string _name;
		protected string _description;

		protected int HeartTrees;

		public string description {
			get {
				return _description;
			}
		}

		public string name {
			get {
				return _name;
			}
		}

		public CursedWood() {
			_name = "Cursed Woods";
			_description = "A malevolent dark forest";
			HeartTrees = 5;
		}

		public bool isActive() {
			return false;
		}

		public T getQuestType<T>() where T : ObstacleType, IQuestGoal {
			//killing heart tree
			//killing corrupted hero
			//burn back the wood
			//rescule someone taken
			//purge corruption
			//deceptions:
			//  slay named character (king, etc)
			throw new NotImplementedException();
		}

		public void onSuccessfulQuest(Quest theQuest) {
			throw new NotImplementedException();
		}

		public void onFailedQuest(Quest theQuest) {
			throw new NotImplementedException();
		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {
			info.AddValue(name + "_HeartTrees", HeartTrees);
		}

		public void deserialize(Hashtable info) {
			HeartTrees = (int)info[name + "_HeartTrees"];
		}
	}
}
