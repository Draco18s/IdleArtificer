namespace RPGKit.FantasyNameGenerator.Generators
{
	public class MaleWizardFirstNameGenerator : NameGenerator
	{
		public static string[] Wizard_FirstNames = new string[] { "Voltar", "Vultrax", "Frizban", "Rasputin", "Rostor", "Simonexto", "Guurglex" };

		public static string[] Wizard_FirstNamesPart1 = new string[] { "Ard", "Alf", "Fiz", "Risa", "Warran", "Kel", "Wren", "Kan", "Can", "Gy", "Dero", "Ak",
		"Dall", "Dell", "Mil", "Ward" };
		public static string[] Wizard_FirstNamesPart2 = new string[] { "bald", "ban", "buck", "tor", "van", "gax", "trandor", "thuri", "ben", "baldar", "may", "lam", "mor", "dard", "burg", "whit" };
		
		public override string GetName ()
		{
			int weight = GetWeight();
			if(weight<15)
				return GetRandomElement(Wizard_FirstNames);
			else
				return GetRandomElement (Wizard_FirstNamesPart1) + GetRandomElement (Wizard_FirstNamesPart2);
		}

	}
}
