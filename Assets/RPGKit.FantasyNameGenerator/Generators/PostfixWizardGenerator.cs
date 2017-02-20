using System.Collections.Generic;

namespace RPGKit.FantasyNameGenerator.Generators
{
    public class PostfixWizardGenerator : NameGenerator
    {
        private static readonly List<string> _postfixes = new List<string>() { "Abjurer", "Black Mage", "Blue Mage", "Conjurer", "Dark Wizard", "Diviner", "Elementalist", "Enchanter", "Illusionist", "Mage", "Magician", "Necromancer", "Red Mage", "Sorcerer", "Summoner", "Transmuter", "Warlock" };

        public override string GetName()
        {
            int length = _postfixes.Count;
            return "the " + _postfixes[GetRandomNumber(length)];
        }
    }
}
