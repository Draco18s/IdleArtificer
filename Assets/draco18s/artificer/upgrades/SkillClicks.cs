using UnityEngine;

namespace Assets.draco18s.artificer.upgrades {
	internal class SkillClicks : Skill {

		public SkillClicks(string skillName, double multi, int cost, double costIncrease) : base(skillName, multi, cost, costIncrease) {

		}

		public override string getMultiplierForDisplay() {
			return (Mathf.Round((float)multiplier*100)/100).ToString();
		}
	}
}