using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.statistics;
using System.Globalization;

namespace Assets.draco18s.artificer.quests.challenge.goals.DeepGoals {
	public class WarPreparations : IDeepGoal {
		protected string _name;
		protected string _description;
		protected int sideAstrength = 3;
		protected int sideBstrength = 3;
		protected bool battleOver = false;
		protected bool combatStarts = false;
		protected string sideAname = "";
		protected string sideBname = "";
		protected bool hasInfo = false;
		protected int numGuildmastersAtWarEnd = -1;
		public string description {
			get {
				if(!sideAname.Equals("") && !sideBname.Equals("")) {
					//return _description + "The war is between the kingdoms of " + sideAname + " and " + sideBname;
					return String.Format(_description, "The two kingdoms of " + sideAname + " and " + sideBname + " are going to war");
				}
				//Two kingdoms are going to war
				return String.Format(_description, "Two kingdoms are going to war");
			}
		}

		public string name {
			get {
				return _name;
			}
		}

		public WarPreparations() {
			_name = "War Preparations";
			_description = "{0}, effecting local economies.\nArmor and Weapons sell for more.\nReduced speed for Glass and Gold industries.";

		}

		public IDeepGoal register() {
			DeepGoalsTypes.register(this);
			return this;
		}

		public float getSpeedModifier(Industry industry) {
			if(industry.industryType == Industries.IndustryTypesEnum.GLASS || industry.industryType == Industries.IndustryTypesEnum.GOLD) {
				return 0.5f;
			}
			return 1f;
		}

		public float getValuedModifier(Industry industry) {
			if(industry.industryItem.hasReqType(requirement.RequirementType.ARMOR) || (industry.industryItem.equipType & ItemEquipType.WEAPON) > 0 || (industry.industryItem.equipType & ItemEquipType.RANGED) > 0) {
				if(industry.industryType == Industries.IndustryTypesEnum.IRON) {
					return 4;
				}
				return 2;
			}
			return 1;
		}

		public bool isActive() {
			if(battleOver && numGuildmastersAtWarEnd + 1 < StatisticsTracker.guildmastersElected.value) {
				sideAstrength = 3;
				sideBstrength = 3;
				battleOver = false;
				combatStarts = false;
				sideAname = "";
				sideBname = "";
			}
			
			if(battleOver) return false;

			return true;
		}

		public int minQuestDifficulty() {
			return 10;
		}

		public ObstacleType getQuestType() {
			if(combatStarts) return ChallengeTypes.Goals.DeepGoalSpecial.COMBAT;
			return ChallengeTypes.Goals.DeepGoalSpecial.EQUIP_ARMY;
		}

		public void modifyQuest(Quest theQuest) {
			theQuest.miscData = new Dictionary<string, object>();
			theQuest.numQuestsBefore = Math.Min(theQuest.numQuestsBefore, 10);
			int r = theQuest.questRand.Next(2);
			UnityEngine.Debug.Log("R: " + r);
			if(sideAname.Equals("")) r = 0;
			else if(sideBname.Equals("")) r = 1;
			if(r == 0) {
				theQuest.miscData.Add("WarSide", "A");
				int i = theQuest.heroName.IndexOf("of ");
				if(sideAname.Equals("")) {
					sideAname = theQuest.heroName.Substring(i + 3);
				}
				else {
					theQuest.heroName = theQuest.heroName.Substring(0, i + 3) + sideAname;
				}
			}
			else {
				theQuest.miscData.Add("WarSide", "B");
				int i = theQuest.heroName.IndexOf("of ");
				if(sideBname.Equals("")) {
					sideBname = theQuest.heroName.Substring(i + 3);
				}
				else {
					theQuest.heroName = theQuest.heroName.Substring(0, i + 3) + sideBname;
				}
			}
		}

		public void finalizeQuest(ref Quest theQuest) {
			
		}

		public void onSuccessfulQuest(Quest theQuest) {
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(theQuest.miscData != null) {
				if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.EQUIP_ARMY) {
					object sideObj;
					theQuest.miscData.TryGetValue("WarSide", out sideObj);
					string sideStr = (string)sideObj;
					if(sideStr.Equals("A")) {
						sideAstrength += 1;
					}
					else {
						sideBstrength += 1;
					}
				}
				else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.COMBAT) {
					object sideObj;
					theQuest.miscData.TryGetValue("WarSide", out sideObj);
					string sideStr = (string)sideObj;
					if(sideStr.Equals("A")) {
						//side A quest was a success, so are attacking
						//odds are wider
						doCombat(ref sideAstrength, ref sideBstrength, theQuest, true);
					}
					else {
						//side B quest was a success, so are attacking
						//odds are wider
						doCombat(ref sideBstrength, ref sideAstrength, theQuest, true);
					}
				}
				checkVictory();
				//else 
			}
			if(goal.type == ChallengeTypes.Goals.OBSERVE_ENEMY) {
				hasInfo = true;
			}
		}

		public void onFailedQuest(Quest theQuest) {
			QuestChallenge goal = QuestManager.getGoal(theQuest);
			if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.EQUIP_ARMY) {
				//sideBstrength += 1;
			}
			else if(goal.type == ChallengeTypes.Goals.DeepGoalSpecial.COMBAT) {
				object sideObj;
				theQuest.miscData.TryGetValue("WarSide", out sideObj);
				string sideStr = (string)sideObj;
				if(sideStr.Equals("A")) {
					//side A quest was a failure, so are defending
					//however, odds are narrower
					doCombat(ref sideBstrength, ref sideAstrength, theQuest, false);
				}
				else {
					//side B quest was a failure, so are defending
					//however, odds are narrower
					doCombat(ref sideAstrength, ref sideBstrength, theQuest, false);
				}
			}
			checkVictory();
		}

		private void doCombat(ref int attacker, ref int defender, Quest theQuest, bool wasSuccessful) {
			int[] atk = new int[3];
			int[] def = new int[3];
			for(int i = 0; i < 3; i++) { //attacker gets 3 dice
				if(i <= attacker) atk[i] = theQuest.testLuck(6 + (wasSuccessful ? (hasInfo ? 2 : 1) : 0)) + 1;
				else atk[i] = 0;
			}
			for(int i = 0; i < 3; i++) { //defender gets 2 dice
				if(i <= sideBstrength && i < 2) def[i] = theQuest.testLuck(6 + (!wasSuccessful && hasInfo ? 1 : 0)) + 1;
				else def[i] = 0;
			}
			Array.Sort(atk);
			Array.Sort(def);
			Array.Reverse(atk);
			Array.Reverse(def);
			for(int i = 0; i < 3; i++) { //for each die
				if(atk[i] > def[i]) { //if attacker die is bigger than defender, kill a defender
					defender--;
				}
				else if(def[i] > 0) { //else as long as there *is* a defender, kill attacker (defender wins ties)
					attacker--;
				}
			}
			hasInfo = false;
		}

		private void checkVictory() {
			if(!combatStarts) {
				if(sideAstrength > 25 || sideBstrength > 25) {
					combatStarts = true;
				}
			}
			else {
				if(sideAstrength < 0 || sideBstrength < 0) {
					battleOver = true;
					numGuildmastersAtWarEnd = StatisticsTracker.guildmastersElected.value;
				}
			}
		}

		public void serialize(ref SerializationInfo info, ref StreamingContext context) {
			info.AddValue(name + "_sideAstrength", sideAstrength);
			info.AddValue(name + "_sideBstrength", sideBstrength);
			info.AddValue(name + "_sideAname", sideAname);
			info.AddValue(name + "_sideBname", sideBname);
			info.AddValue(name + "_battleOver", battleOver);
			info.AddValue(name + "_combatStarts", combatStarts);
			info.AddValue(name + "_hasInfo", hasInfo);
			info.AddValue(name + "_numGuildmastersAtWarEnd", numGuildmastersAtWarEnd);
		}

		public void deserialize(Hashtable info) {
			if(info.ContainsKey(name + "_sideAstrength")) {
				sideAstrength = (int)info[name + "_sideAstrength"];
				sideBstrength = (int)info[name + "_sideBstrength"];
				sideAname = (string)info[name + "_sideAname"];
				sideBname = (string)info[name + "_sideBname"];
				battleOver = (bool)info[name + "_battleOver"];
				combatStarts = (bool)info[name + "_combatStarts"];
				if(info.ContainsKey(name + "_hasInfo"))
					hasInfo = (bool)info[name + "_hasInfo"];
				numGuildmastersAtWarEnd = (int)info[name + "_numGuildmastersAtWarEnd"];
			}
		}
	}
}
