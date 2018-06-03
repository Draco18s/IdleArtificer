using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalRescueCaptured : ObstacleType, IQuestGoal {
		public GoalRescueCaptured() : base("rescuing someone captured", new RequireWrapper(RequirementType.ARMOR), new RequireWrapper(RequirementType.WEAPON), new RequireWrapper(RequirementType.FIRM_RESOLVE)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 2) result = EnumResult.CRIT_FAIL;
			else if(fails > 0) result = EnumResult.FAIL;
			else result = EnumResult.MIXED;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
		
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(10, DamageType.POISON);
					object corwrap;
					int cor = 0;
					if(theQuest.miscData.TryGetValue("forest_corruption", out corwrap)) {
						cor = (int)corwrap;
						theQuest.miscData.Remove("forest_corruption");
					}
					cor++;
					theQuest.miscData.Add("forest_corruption", cor);
					questBonus--;
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.harmHero(5, DamageType.POISON);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddUncommonResource(theQuest);
					ChallengeTypes.Loot.AddRareResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Rescued someone";
		}

		public string relicNames(ItemStack stack) {
			return "Heroic";
		}

		public int getNumTotalEncounters() {
			return 9;
		}
	}
}
