using System;
using System.Collections.Generic;
using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{
    public class FantasyNameGenerator : IFantasyNameGenerator
    {
        //private readonly List<INameGenerator> _nameGenerators;

        public Gender Gender { get; set; }
        public INameGenerator PrefixGenerator { get; set; }
        public INameGenerator FirstNameGenerator { get; set; }
        public INameGenerator LastNameGenerator { get; set; }
        public INameGenerator PostfixNameGenerator { get; set; }
        public INameGenerator LandNameGenerator { get; set; }

        public FantasyName GetFantasyName()
        {
            FantasyName name = new FantasyName();

            name.Gender = Gender;

            if (PrefixGenerator != null)
                name.Prefix = PrefixGenerator.GetName();

            if (FirstNameGenerator != null)
                name.FirstName = FirstNameGenerator.GetName();

            if (LastNameGenerator != null)
                name.LastName = LastNameGenerator.GetName();

            if (PostfixNameGenerator != null)
                name.Postfix = PostfixNameGenerator.GetName();

            if (LandNameGenerator != null)
                name.Land = LandNameGenerator.GetName();

            return name;
        }

        public FantasyNameGenerator()
        {
            //_nameGenerators = new List<INameGenerator>();
        }

        public static FantasyNameGenerator FromSettingsInfo(FantasyNameSettings fantasyNameSettings)
        {
            var fantasyNameGenerator = new FantasyNameGenerator();

            fantasyNameGenerator.Gender = fantasyNameSettings.Gender;

            //use type matching/strategy pattern here or whatever you wanna call it.

            // No prefix included in version 1
            //if(IncludePrefix)
            //	compositeNameGenerator.PrefixGenerator = new PrefixGenerator();

            if (fantasyNameSettings.ChosenClass != Classes.None)
            {
                INameGenerator maleNameGenerator = null;

                if (fantasyNameSettings.ChosenClass == Classes.Cleric)
                    maleNameGenerator = new MaleClericFirstNameGenerator();

                if (fantasyNameSettings.ChosenClass == Classes.Rogue)
                    maleNameGenerator = new MaleRogueFirstNameGenerator();

                if (fantasyNameSettings.ChosenClass == Classes.Warrior)
                    maleNameGenerator = new MaleWarriorFirstNameGenerator();

                if (fantasyNameSettings.ChosenClass == Classes.Wizard)
                    maleNameGenerator = new MaleWizardFirstNameGenerator();

				if(fantasyNameSettings.ChosenClass == Classes.CVC)
					maleNameGenerator = new CVCNameGenerator();

				if (fantasyNameSettings.Gender == Gender.Male)
                {
                    fantasyNameGenerator.FirstNameGenerator = maleNameGenerator;
                }
                else
                {
                    fantasyNameGenerator.FirstNameGenerator = new FemaleWrapperNameGenerator(maleNameGenerator);
                }

                fantasyNameGenerator.LastNameGenerator = new LastNameGenerator();
            }
            else
            {
                fantasyNameGenerator.FirstNameGenerator = new RaceNameGenerator(fantasyNameSettings.ChosenRace);
                fantasyNameGenerator.LastNameGenerator = new RaceNameGenerator(fantasyNameSettings.ChosenRace);
            }


            if (fantasyNameSettings.IncludePostfix)
            {
                if (fantasyNameSettings.ChosenClass == Classes.Wizard)
                    fantasyNameGenerator.PostfixNameGenerator = new PostfixWizardGenerator();
				else if(fantasyNameSettings.ChosenRace == Race.Dragon)
					fantasyNameGenerator.PostfixNameGenerator = new DragonPostfixGenerator();
				else if (fantasyNameSettings.ChosenRace != Race.None)
                    fantasyNameGenerator.PostfixNameGenerator = new VilePostfixGenerator();
                else
                    fantasyNameGenerator.PostfixNameGenerator = new PostfixGenerator();
            }

            if (fantasyNameSettings.IncludeHomeland)
                fantasyNameGenerator.LandNameGenerator = new LandGenerator();

            return fantasyNameGenerator;
        }

        public FantasyName[] GetFantasyNames(int numNames)
        {
            if (numNames < 0)
                throw new ArgumentException(string.Format("Number of fantasy names cannot be negative. [{0}", numNames));

            FantasyName[] fantasyNames = new FantasyName[numNames];

            for (int i = 0; i < numNames; i++)
            {
                fantasyNames[i] = this.GetFantasyName();
            }

            return fantasyNames;
        }
    }
}
