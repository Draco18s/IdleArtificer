namespace RPGKit.FantasyNameGenerator.Generators
{
	public class VilePostfixGenerator : NameGenerator
	{
		private string[] _namePostfixes;
		
		public VilePostfixGenerator()
		{
			_namePostfixes = new string[] { "Unholy", "Crazed", "Terrible", "Cruel", "Gruesome", "Boneless", "Gnarled", "Broken", "Underhanded", "Scalper", "Tormented"};				
		}

		public override string GetName ()
		{
			return "the " + GetRandomElement (_namePostfixes);
		}
	}

	public class DragonPostfixGenerator : NameGenerator {
		private string[] _namePostfixes;

		public DragonPostfixGenerator() {
			_namePostfixes = new string[] { "Crazed", "Terrible", "Cruel", "Gruesome", "Gnarled", "Avenger", "Juggernaut", "Slayer", "Fierce", "Destroyer",
				"Mighty", "Unavoidable", "Archmage","Deathless", "Margrave", "Vanquisher", "Hoarder", "Vintner", "Harbinger", "Haruspex", "Encroacher",
				"Eggsitter", "Frightful", "Clever", "Firestarter", "Ancient", "Grumpy", "Hotblood", "Lewd", "Green", "Red", "Gold", "Black", "Mottled" };
		}//Moneybags

		public override string GetName() {
			return "the " + GetRandomElement(_namePostfixes);
		}
	}


	public class PostfixGenerator : NameGenerator
	{
		private string[] _namePostfixes;
		
		public PostfixGenerator()
		{
			_namePostfixes = new string[] { "the Virtuous", "the Valiant", "the Unholy", "the Holy", "the Fierce", "the Golden", "the Bold", "the Brash", "the Crazed", "the Great",
		"the Terrible", "the Cruel", "the Destroyer", "the Magnificent", "the Brave", "the Cowardly", "the Mighty", "the One", "the Unavoidable", "the Lion",
		"the Pious", "I", "II", "III", "IV", "V", "the Black", "the Red", "the Good", "the Boneless",
		"the Great King", "the Restless", "the Peaceful", "the Traveled", "the Deposed", "the Pacifist" };				
		}

		public override string GetName ()
		{
			return GetRandomElement (_namePostfixes);
		}
		
	}
}
