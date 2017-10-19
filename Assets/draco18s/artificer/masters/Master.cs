using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.draco18s.artificer.init;
using UnityEngine;
using Assets.draco18s.artificer.game;
using System.Runtime.Serialization;

namespace Assets.draco18s.artificer.masters {
	[Serializable]
	public class Master {
		private static System.Random rand = new System.Random();
		private static int[] weights = { 5, 3, 1, -2 };
		public static Master createRandomMaster(int points) {
			Master newMaster = new Master();
			int a = rand.Next(12);
			int b = rand.Next(12);
			int c = rand.Next(12);
			int d = rand.Next(12);

			adjustStat(newMaster, a, weights[0], points);
			adjustStat(newMaster, b, weights[1], points);
			adjustStat(newMaster, c, weights[2], points);
			adjustStat(newMaster, d, weights[3], points);
			//newMaster.displayString = getString(newMaster, a) + "\n" + getString(newMaster, b) + "\n" + getString(newMaster, c) + "\n" + getString(newMaster, d);
			newMaster.displayString = getString(newMaster, a);
			if(a != b) newMaster.displayString += "\n" + getString(newMaster, b);
			if(a != c && b != c) newMaster.displayString += "\n" + getString(newMaster, c);
			if(a != d && b != d && d != c) newMaster.displayString += "\n" + getString(newMaster, d);
			newMaster.finalize();
			return newMaster;
		}

		private static void adjustStat(Master newMaster, int stat, int weight, int totalPoints) {
			float amt = (weight / 7f * totalPoints);
			switch(stat) {
				case 0:
					newMaster.cash += (float)Math.Round(amt) / 100f;
					break;
				case 1:
					newMaster.renown += (float)Math.Round(amt) / 100f * 0.1f;
					break;
				case 2:
					newMaster.ingredient += (float)Math.Round(amt * 21) / 100f;
					break;
				case 3:
					newMaster.relic += (float)Math.Round(amt * 2.1) / 100f * 0.1f;
					break;
				case 4:
					int ty = rand.Next(7) + 1;
					newMaster.industry[ty] += (float)Math.Round(amt * 10.5) / 100f;
					break;
				case 5:
					newMaster.industryRate += (float)Math.Round(amt) / 100f;
					break;
				case 6:
					newMaster.quest += (float)Math.Round(amt) / 100f;
					break;
				case 7:
					newMaster.vendors += (float)Math.Round(amt * 7 / 3) / 100f;
					break;
				case 8:
					newMaster.apprentice += (float)Math.Round(amt) / 100f;
					break;
				case 9:
					newMaster.journeymen += (float)Math.Round(amt * 7 / 3) / 100f;
					break;
				case 10:
					newMaster.research += (float)Math.Round(amt * 3.15) / 100f;
					break;
				case 11:
					newMaster.click += (float)Math.Round(amt * 7) / 100f;
					break;
			}
		}

		private static string getString(Master newMaster, int stat) {
			string s = "";
			switch(stat) {
				case 0:
					s = (newMaster.cash > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.cash - 1) * 100) + "% cash from industries";
					break;
				case 1:
					s = (newMaster.renown > 1 ? "+" : "") + (Mathf.RoundToInt((newMaster.renown - 1) * 1000) / 10f) + "% renown from quests";
					break;
				case 2:
					s = (newMaster.ingredient > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.ingredient - 1) * 100) + "% more enchanting ingredients";
					break;
				case 3:
					s = (newMaster.relic > 1 ? "+" : "") + (Mathf.RoundToInt((newMaster.relic - 1) * 1000) / 10f) + "% relic sell value";
					break;
				case 4:
					foreach(Industries.IndustryTypesEnum ty in Enum.GetValues(typeof(Industries.IndustryTypesEnum))) {
						if(newMaster.industry[(int)ty] != 1) {
							string indust = Main.ToTitleCase(ty.ToString());
							s += (newMaster.industry[(int)ty] > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.industry[(int)ty] - 1) * 100) + "% all " + indust + " industry values" + "\n";
						}
					}
					s = s.Substring(0, s.Length - 1);
					break;
				case 5:
					s = (newMaster.industryRate > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.industryRate - 1) * 100) + "% all industry production speed";
					break;
				case 6:
					s = (newMaster.quest > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.quest - 1) * 100) + "% faster quests";
					break;
				case 7:
					s = (newMaster.vendors > 0 ? "+" : "") + (Mathf.RoundToInt((newMaster.vendors) * 100)) + "% vendor effectiveness";
					break;
				case 8:
					s = (newMaster.apprentice > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.apprentice - 1) * 100) + "% apprentice effectivenss";
					break;
				case 9:
					s = (newMaster.journeymen > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.journeymen - 1) * 100) + "% journeymen speed";
					break;
				case 10:
					s = (newMaster.research > 1 ? "+" : "") + Mathf.RoundToInt((newMaster.research - 1) * 100) + "% research speed";
					break;
				case 11:
					s = "Click speed" + (newMaster.click > 0 ? " increased " : " reduced ") + "by " + Math.Abs(newMaster.click) + "s";
					break;
			}
			return s;
		}

		protected float cash = 1;//
		protected float renown = 1;//
		protected float ingredient = 1;//
		protected float relic = 1;//
		protected float[] industry = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };//
		protected float industryRate = 1;//
		protected float quest = 1;//
		protected float vendors = 0;//
		protected float apprentice = 1;//
		protected float journeymen = 1;//
		protected float research = 1;//
		protected float click;

		protected string displayString;

		public Master() {
			displayString = "";
		}

		//allows for -100% and larger values, where -100% is "half"
		//   0.7 -> -30% less
		//   0.7 - 1 = -0.3
		//   Abs(-0.3) + 1 = 1.3
		//   1 / 1.3 = 0.77
		protected void finalize() {
			if(cash < 1) cash = 1 / (Math.Abs(cash - 1) + 1);
			if(renown < 1) renown = 1 / (Math.Abs(renown - 1) + 1);
			if(ingredient < 1) ingredient = 1 / (Math.Abs(ingredient - 1) + 1);
			if(relic < 1) relic = 1 / (Math.Abs(relic - 1) + 1);
			if(industryRate < 1) industryRate = 1 / (Math.Abs(industryRate - 1) + 1);
			if(quest < 1) quest = 1 / (Math.Abs(quest - 1) + 1);
			//if(vendors < 1) vendors = 1 / (Math.Abs(vendors - 1) + 1);
			if(apprentice < 1) apprentice = 1 / (Math.Abs(apprentice - 1) + 1);
			if(journeymen < 1) journeymen = 1 / (Math.Abs(journeymen - 1) + 1);
			if(research < 1) research = 1 / (Math.Abs(research - 1) + 1);
			for(int i = 0; i < industry.Length; i++) {
				if(industry[i] < 1) industry[i] = 1 / (Math.Abs(industry[i] - 1) + 1);
			}
			//if(click < 1) click = 1 / (Math.Abs(click - 1) + 1);
		}
		public float cashIncomeMultiplier() {
			return cash;
		}
		public float renownIncomeMultiplier() {
			return renown;
		}
		public float ingredientIncomeMultiplier() {
			return ingredient;
		}
		public float relicSellMultiplier() {
			return relic;
		}
		public float researchMultiplier() {
			return relic;
		}
		public float industryTypeMultiplier(Industries.IndustryTypesEnum forType) {
			return industry[(int)forType];
		}
		public float industryRateMultiplier() {
			return industryRate;
		}
		public float newQuestRateMultiplier() {
			return quest;
		}
		public float vendorSellMultiplier() {
			return vendors;
		}
		public float apprenticeRateMultiplier() {
			return apprentice;
		}
		public float journeymenRateMultiplier() {
			return journeymen;
		}
		public float clickRateMultiplier() {
			return click;
		}

		public string getDisplay() {
			return displayString;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("cash", cash);
			info.AddValue("renown", renown);
			info.AddValue("ingredient", ingredient);
			info.AddValue("relic", relic);
			info.AddValue("industryRate", industryRate);
			info.AddValue("quest", quest);
			info.AddValue("vendors", vendors);
			info.AddValue("apprentice", apprentice);
			info.AddValue("journeymen", journeymen);
			info.AddValue("research", research);
			for(int i = 0; i < industry.Length; i++) {
				info.AddValue("industry_"+i, industry[i]);
			}
			info.AddValue("click", click);
		}

		public Master(SerializationInfo info, StreamingContext context) {
			cash = (float)info.GetDouble("cash");
			renown = (float)info.GetDouble("renown");
			ingredient = (float)info.GetDouble("ingredient");
			relic = (float)info.GetDouble("relic");
			industryRate = (float)info.GetDouble("industryRate");
			quest = (float)info.GetDouble("quest");
			vendors = (float)info.GetDouble("vendors");
			apprentice = (float)info.GetDouble("apprentice");
			journeymen = (float)info.GetDouble("journeymen");
			research = (float)info.GetDouble("research");
			for(int i = 0; i < industry.Length; i++) {
				industry[i] = (float)info.GetDouble("industry_" + i);
			}
			click = (float)info.GetDouble("click");
		}
	}
}
