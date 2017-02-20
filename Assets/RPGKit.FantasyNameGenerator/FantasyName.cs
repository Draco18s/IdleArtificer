using System;
using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{
    public class FantasyName
	{
        public Gender Gender { get; set; }
		public String Prefix { get; set; }
		public String FirstName { get; set; }
		public String LastName { get; set; }
		public String Postfix { get; set; }
		public String Land { get; set; }

		public String FullName
		{
			get { return FirstName + " " + LastName + " " + Postfix; }
		}

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", Gender, FirstName, LastName, Postfix, Land);
        }
	}
}
