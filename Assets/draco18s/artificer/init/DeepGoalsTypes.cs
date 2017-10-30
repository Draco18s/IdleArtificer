using Assets.draco18s.artificer.quests.challenge.goals.DeepGoals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public class DeepGoalsTypes {
		private static List<IDeepGoal> allDeepGoals = new List<IDeepGoal>();
		public static IDeepGoal THE_CURSED_WOODS = new CursedWood();

		public static void register(IDeepGoal goal) {
			allDeepGoals.Add(goal);
		}

		public static void serialize(ref SerializationInfo info, ref StreamingContext context) {
			foreach(IDeepGoal s in allDeepGoals) {
				s.serialize(ref info, ref context);
			}
		}

		public static void deserialize(ref SerializationInfo info, ref StreamingContext context) {
			SerializationInfoEnumerator infoEnum = info.GetEnumerator();
			Hashtable values = new Hashtable();
			while(infoEnum.MoveNext()) {
				SerializationEntry val = infoEnum.Current;
				values.Add(val.Name, val.Value);
			}
			foreach(IDeepGoal s in allDeepGoals) {
				s.deserialize(values);
			}
		}
	}
}
