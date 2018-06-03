using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalHeartTree : ObstacleType, IQuestGoal {
		public GoalHeartTree() : base("killing a hearttree", new RequireWrapper(RequirementType.FIRE_DAMAGE), new RequireWrapper(RequirementType.MIND_SHIELD)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.SUCCESS - fails;
			if(!theQuest.testCharisma(-2)) result--;
			if(theQuest.testStrength(-2)) result++;
			else theQuest.harmHero(10, DamageType.FIRE);
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			object corwrap;
			int cor = 0;
			if(theQuest.miscData.TryGetValue("forest_corruption", out corwrap)) {
				cor = (int)corwrap;
				theQuest.miscData.Remove("forest_corruption");
			}
			switch(result) {
				case EnumResult.CRIT_FAIL:
				case EnumResult.FAIL:
					if(cor < 5) {
						if(result == EnumResult.CRIT_FAIL) cor += 1;
						cor += 1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.MIXED:
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
			theQuest.miscData.Add("forest_corruption",cor);
		}

		public string relicDescription(ItemStack stack) {
			return "Burned a heart tree";
		}

		public string relicNames(ItemStack stack) {
			return "Blighted";
		}

		public int getNumTotalEncounters() {
			return 13;
		}
	}
}
