using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{

	public class MaleClericFirstNameGenerator : NameGenerator
	{
		public static string[] Cleric_FirstNames = new string[] { "Tristan", "Harvord", "Diplo", "Koushik", "Votolado", "Hareck", "Clifton", "Werto", "Exclides", "Mathewn","Matthorn", "Franz", "Franx", "Logstrom", "Montreno" };
		
		public static string[] Cleric_FirstNamesPart1 = new string[] { "Strum", /*"Halo",*/ "Car", /*"Heva",*/ "Men", "Gre", "Ko", "Hay", "Day", "Kee", "Zel", "Sam", "Hed", "Bo", "Cas", "Rav" };
		public static string[] Cleric_FirstNamesPart2 = new string[] { "bright", /*"gold",*/ "burnd", "del", "gor", "bold", "ther", "ton", "field", "lant", "rus" };
		
		public override string GetName ()
		{
			int weight = GetWeight();
			if(weight<15)
				return GetRandomElement(Cleric_FirstNames);
			else
				return GetRandomElement(Cleric_FirstNamesPart1) + GetRandomElement(Cleric_FirstNamesPart2);
		}		
	}
}
