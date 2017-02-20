namespace RPGKit.FantasyNameGenerator.Generators
{
	public class LandGenerator : NameGenerator
	{
		public static string[] LandsPart1 = new string[] { "Dagger", "Broken", "Shining", /*"Sword", "Dragon",*/ "Forgotten", "Frost", "Black", "North", "South", "White", "Haunted", "Shadow", "Misty", "Splintered", "Sparkling"/*, "Goblin"*/, "Dagger" };
		public static string[] LandsPart2 = new string[] { "Coast", "Hills", "Lands", "Isles", "Keep", "Marsh", "Island", "Vale", "Mountain", "Forest", "Creek", "Falls", "Rift", "Woods", "Harbor", "Cliffs", "Haven", "Dell", "Fjord" };

		public static string[] LandsPartA = new string[] {/*"Cor",*/ "Car", "Cer", /*"Dran",*/ "Drun", "Dren", "Fen", "Fan", /*"Fon",*/ "Len", /*"Lan",*/ "Lon", "An", "En", "Un", /*"Ra", "Ro",*/ "Kan", "Kun", "Kon", "Tur", "Dra" , "Dag"};
		public static string[] LandsPartB = new string[] { "kor", "ker", "kar", "gar", "gor", "ger", "tar", "tor", "ter", "thyr", "myr", /*"ther",*/ "thar", "thor", "ith" };

		public override string GetName ()
		{
			int weight = GetWeight();
			if(weight < 33)
				return "of " + GetRandomElement(LandsPart1) + " " + GetRandomElement(LandsPart2);
			
            if(weight < 66)
				return "of " + GetRandomElement(LandsPartA) + GetRandomElement(LandsPartB);
			
			return "of " + GetRandomElement(LandsPartA) + GetRandomElement(LandsPartB) + " " + GetRandomElement(LandsPart2);
		}
	}
}