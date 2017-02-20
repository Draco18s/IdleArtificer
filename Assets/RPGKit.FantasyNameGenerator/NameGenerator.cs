using System;
using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{
    public abstract class NameGenerator : INameGenerator
    {
        private static readonly Random _random = new Random();

        public static Func<int, int> GetRandomNumber = (maxValue) => { return _random.Next(maxValue); };

        protected string GetRandomElement (string[] array)
		{
		    int index = GetRandomNumber(array.Length);
		    return array[index];
		}

        protected int GetWeight()
		{
		    return GetRandomNumber(100);		
		}

        public abstract string GetName();
    }
}
