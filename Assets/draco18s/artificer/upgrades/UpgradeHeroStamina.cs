using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.upgrades;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.upgrades {
	public class UpgradeHeroStamina : Upgrade {
		private int amount;

		public UpgradeHeroStamina(BigInteger cost, int v, string saveName):base(UpgradeType.HERO_STAMINA, cost, "Increases hero starting mana by " + v, saveName) {
			amount = v;
		}

		public override string getIconName() {
			return "upgrades/hero_mp";
		}

		public override string getTooltip() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			return "Increases hero starting mana by " + (amount) + ".\nThe base value is 2000, currently it is " + Quest.GetQuestMaxTime() + ", and with this upgrade it would be " + (Quest.GetQuestMaxTime() + amount) + ". Existing quests hero's MP cap will be increased, but will retain their current amount.";
		}
	}
}