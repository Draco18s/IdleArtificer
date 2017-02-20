using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{
    public class FantasyNameSettings
    {
        public Classes ChosenClass { get; set; }
        public Race ChosenRace { get; set; }
        public bool IncludeHomeland { get; set; }
        public bool IncludePostfix { get; set; }
        public Gender Gender { get; set; }

        public FantasyNameSettings(Classes chosenclass, Race race, bool includeHomeland, bool includePostfix, Gender gender)
        {
            ChosenClass = chosenclass;
            ChosenRace = race;
            IncludeHomeland = includeHomeland;
            IncludePostfix = includePostfix;
            Gender = gender;
        }

        public static FantasyNameSettings DefaultSettings()
        {
            return new FantasyNameSettings(Classes.Warrior, Race.None, true, true, Gender.Male);
        }

        public override string ToString()
        {
            return string.Format("[FantasyNameSettings: ChosenClass={0}, ChosenRace={1}, IncludeHomeland={2}, IncludePostfix={3}, Gender={4}]", ChosenClass, ChosenRace, IncludeHomeland, IncludePostfix, Gender);
        }
    }
}