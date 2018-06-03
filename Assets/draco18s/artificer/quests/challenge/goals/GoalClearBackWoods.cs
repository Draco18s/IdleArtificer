using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalClearBackWoods : ObstacleType, IQuestGoal {
		public GoalClearBackWoods() : base("burning back the cursed wood", new RequireWrapper(RequirementType.FIRE_DAMAGE), new RequireWrapper(RequirementType.DISPELLING)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.SUCCESS - fails;
			if(!theQuest.testCharisma(questBonus - 2)) result--;
			if(theQuest.testStrength(questBonus - 2)) result++;
			else theQuest.harmHero(10, DamageType.FIRE);
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			object corwrap;
			int cor = 0;
			if(theQuest.miscData != null && theQuest.miscData.TryGetValue("forest_corruption", out corwrap)) {
				cor = (int)corwrap;
				theQuest.miscData.Remove("forest_corruption");
			}
			else if(theQuest.miscData == null) {
				theQuest.miscData = new Dictionary<string, object>();
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
					questBonus++;
					if(questBonus < 8)
						theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					questBonus += 2;
					if(questBonus < 8)
						theQuest.repeatTask();
					break;
				case EnumResult.CRIT_SUCCESS:
					questBonus += 4;
					if(questBonus < 8)
						theQuest.repeatTask();
					else
						ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
			theQuest.miscData.Add("forest_corruption", cor);
		}

		public string relicDescription(ItemStack stack) {
			return "Burned back the cursed forest";
		}

		public string relicNames(ItemStack stack) {
			return "Forestry";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
