using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using Assets.draco18s.artificer.init;

namespace Assets.draco18s.artificer.quests.challenge {
	//This is like Item or Block in minecraft
	public abstract class ObstacleType {
		public readonly string name;
		public readonly string desc;
		public readonly RequireWrapper[] requirements;
		protected int questDifficultyScalar = 1;
		protected float questRequirementScalar = 1;
		public int numOfTypeCompleted = 0;

		public ObstacleType(string desc, params RequireWrapper[] reqs) {
			this.desc = desc;
			requirements = reqs;
			name = this.GetType().Name;
			GameRegistry.RegisterObstacle(this);
		}

		public ObstacleType(string desc, string name, params RequireWrapper[] reqs) {
			this.desc = desc;
			requirements = reqs;
			this.name = this.GetType().Name + name;
			GameRegistry.RegisterObstacle(this);
		}


		public abstract EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus);

		public abstract void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus);

		public ObstacleType setRewardScalar(int val) {
			questDifficultyScalar = val;
			return this;
		}
		public int getRewardScalar() {
			return questDifficultyScalar;
		}
		public ObstacleType setReqScalar(float val) {
			questRequirementScalar = val;
			return this;
		}
		public float getReqScalar() {
			return questRequirementScalar;
		}
	}

	public class RequireWrapper {
		public readonly RequirementType req;
		public readonly RequirementType alt;

		public RequireWrapper(RequirementType r) {
			req = r;
		}

		public RequireWrapper(RequirementType r, RequirementType a) {
			req = r;
			alt = a;
		}
	}
}