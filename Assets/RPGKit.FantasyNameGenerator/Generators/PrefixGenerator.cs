namespace RPGKit.FantasyNameGenerator.Generators
{
    public class PrefixGenerator : NameGenerator
    {
        public static string[] NamePrefix = new string[] { "Sir" };

        public override string GetName()
        {
            return GetRandomElement(NamePrefix);
        }
    }
}
