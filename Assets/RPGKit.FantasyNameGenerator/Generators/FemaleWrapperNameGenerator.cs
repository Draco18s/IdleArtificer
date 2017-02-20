using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{
    public class FemaleWrapperNameGenerator : NameGenerator
    {
        public static string[] femaleNameEndings = new string[]{"llyn", "a", "in", "lene", "endra", "rin", "iana", "andra", "ia"};

        private INameGenerator _innerGenerator;

        public FemaleWrapperNameGenerator(INameGenerator innerGenerator)
        {
            _innerGenerator = innerGenerator;
        }

        public override string GetName()
        {
            string name = _innerGenerator.GetName();

            name += GetRandomElement(femaleNameEndings);

            return name;
        }
    }
}
