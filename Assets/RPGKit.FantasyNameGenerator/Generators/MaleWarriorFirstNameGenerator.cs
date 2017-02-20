namespace RPGKit.FantasyNameGenerator.Generators
{
	public class MaleWarriorFirstNameGenerator : NameGenerator
	{
		public static string[] Warrior_FirstNames = new string[] { "Brax", "Sole", "Ajax", "Jquery", "Orpheus", "Bane", "Crayton", "Guurgle", "Norax", "Krag",
		"Gwargong", "Kabar", "Klaver", "Demonstrus", "Krag", "Krashnor", "Strider", "Askold", "Gorm", "Harald",
		"Svein", "Cnut", "Harthacnut", "Magnus", "Olaf", "Erik", "Hakon", "Ragnall", "Ivar", "Bardr",
		"Sigtryg", "Blacaire", "Eystein", "Sigfrid", "Rurik", "Vladimir", "Yaroslav", "Yaropolk", "Rollo", "Rolando",
		"Daryen", "Clifton", "Igor", "Guthrum", "Ingvar", "Ivar", "Leif", "Skagul", "Thorfinn" };

		public static string[] Warrior_FirstNamesPart1 = new string[] { "Bra", "Bro", "Gi", "Demo", "Cla", "Clai", "Hee", "Crio", "Die", "Deno",
		"Con", "Wolf", "Zar", "Zer", "War", "Nar", "Thay" };
		public static string[] Warrior_FirstNamesPart2 = new string[] { "nstrus", "x", "lax", "ndor", "ton", "seus", "zzt", "borne", "nan", "run" };

		public override string GetName ()
		{
			if (GetWeight () < 35)
				return GetRandomElement (Warrior_FirstNames);
			else
				return GetRandomElement (Warrior_FirstNamesPart1) + GetRandomElement (Warrior_FirstNamesPart2);
		}
		
	}
}
