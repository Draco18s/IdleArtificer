using Assets.draco18s.artificer.quests.challenge.goals.DeepGoals;
using Assets.draco18s.artificer.upgrades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public class DeepGoalsTypes {
		private static List<IDeepGoal> allDeepGoals = new List<IDeepGoal>();
		public static IDeepGoal NONE = new NoDeepGoal().register();
		public static IDeepGoal WAR_PREPARATIONS = new WarPreparations().register();
		public static IDeepGoal SPIRIT_UPRISING = new SpiritUprising().register();
		public static IDeepGoal THE_CURSED_WOODS = new CursedWood().register();
		public static IDeepGoal CAPITALISM = new Captialists().register();

		public static void register(IDeepGoal goal) {
			allDeepGoals.Add(goal);
		}

		private static IDeepGoal recentlyActive = null;
		public static IDeepGoal getFirstActiveGoal() {
			if(recentlyActive != null && !recentlyActive.isActive()) return recentlyActive;
			foreach(IDeepGoal goal in allDeepGoals) {
				if(goal != NONE && goal.isActive()) {
					if(recentlyActive == null) recentlyActive = goal;
					else if(goal != recentlyActive) return recentlyActive;
					return goal;
				}
			}
			return NONE;
		}

		public static void clearActiveGoal() {
			recentlyActive = null;
		}

		public static void serialize(ref SerializationInfo info, ref StreamingContext context) {
			foreach(IDeepGoal s in allDeepGoals) {
				s.serialize(ref info, ref context);
			}
			info.AddValue("recentlyActive", recentlyActive == null? "null" : recentlyActive.name);
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
			if(values.Contains("recentlyActive")) {
				string ra = values["recentlyActive"] as string;
				if(!ra.Equals("null")) {
					recentlyActive = allDeepGoals.Find(x => x.name == ra);
				}
			}
		}
	}
}
