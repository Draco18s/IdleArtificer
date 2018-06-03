using Assets.draco18s.artificer.init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.artificer.items;

namespace Assets.draco18s.artificer.quests.challenge {
	[Serializable]
	public class QuestChallenge : ISerializable {
		public ObstacleType type;
		public int questBonus;
		public int monsterHealth;

		public QuestChallenge(ObstacleType questType, int bonus) {
			type = questType;
			questBonus = bonus;
			monsterHealth = -1;
		}

		public EnumResult MakeAttempt(Quest quest, int fails, int partials) {
			if(type is IMonsterChallenge && monsterHealth < 0) {
				monsterHealth = ((IMonsterChallenge)type).getMonsterTotalHealth();
			}
			return type.MakeAttempt(quest, fails, partials, questBonus);
		}
		public void OnAttempt(EnumResult result, Quest theQuest) {
			if(type is IMonsterChallenge) {
				ItemStack rangedStack = theQuest.getHeroItemWith(AidType.RANGED_WEAPON);
				int mod = 0;
				if(rangedStack != null) {
					mod = rangedStack.doesStackHave(RequirementType.PERFECT_AIM) && result < EnumResult.CRIT_SUCCESS ? 1 : 0;
					//if(mod + result > EnumResult.CRIT_SUCCESS) mod = 0;
				}
				int dmg = ((IMonsterChallenge)type).getRangedDamage(result+mod, theQuest, ref questBonus, rangedStack);
				if(rangedStack != null) {
					if(rangedStack.doesStackHave(Enchantments.ENHANCEMENT)) {
						foreach(Enchantment en in rangedStack.enchants) {
							if(en == Enchantments.ENHANCEMENT)
								dmg += 5;
						}
					}
					if(rangedStack.item.isConsumable && rangedStack.stackSize > 0) {
						rangedStack.stackSize--;
					}
					dmg = (int)Math.Round(rangedStack.getEffectiveness(RequirementType.RANGED) * dmg);
					rangedStack.onUsedDuringQuest(theQuest);
				}
				monsterHealth -= dmg;
			}
			int hpBefore = theQuest.heroCurHealth;
			type.OnAttempt(result, theQuest, ref questBonus);
			if(type is IMonsterChallenge) {
				bool tookDamage = hpBefore != theQuest.heroCurHealth;
				ItemStack meleeStack = theQuest.getHeroItemWith(AidType.WEAPON);
				int dmg = ((IMonsterChallenge)type).getDamageDealtToMonster(result, theQuest, ref questBonus, meleeStack);
				if(meleeStack != null) {
					if(meleeStack.doesStackHave(Enchantments.ENHANCEMENT)) {
						foreach(Enchantment en in meleeStack.enchants) {
							if(en == Enchantments.ENHANCEMENT)
								dmg += 5;
						}
					}
					if(meleeStack.doesStackHave(Enchantments.KEEN)) {
						if(theQuest.testLuck(5) == 0) {
							dmg *= 2;
						}
					}
					if(meleeStack.item.isConsumable && meleeStack.stackSize > 0) {
						meleeStack.stackSize--;
					}
					dmg = (int)Math.Round(meleeStack.getEffectiveness(RequirementType.WEAPON) * dmg);
					meleeStack.onUsedDuringQuest(theQuest);
				}
				ItemStack thornStack = theQuest.getHeroItemWith(Enchantments.THORNS);
				if(tookDamage && thornStack != null) {
					int c = 0;
					foreach(Enchantment en in thornStack.enchants) {
						if(en == Enchantments.THORNS)
							c++;
					}
					dmg += (c * 5) / 2;
					thornStack.onUsedDuringQuest(theQuest);
				}
				monsterHealth -= dmg;
				if(monsterHealth > 0) {
					theQuest.repeatTask();
				}
				else {
					((IMonsterChallenge)type).getLootDrops(result, theQuest, ref questBonus);
				}
				theQuest.addTime(-50); //combats should be fast
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("quest_id", type.name);
			info.AddValue("bonus", questBonus);
			info.AddValue("monsterHealth", monsterHealth);
		}

		public QuestChallenge(SerializationInfo info, StreamingContext context) {
			string ID = info.GetString("quest_id");
			type = GameRegistry.GetObstacleByID(ID);
			//UnityEngine.Debug.Log("ID: " + ID + ", " + type);
			questBonus = info.GetInt32("bonus");
			monsterHealth = info.GetInt32("monsterHealth");
		}

		public RequirementType getReq(int v) {
			//UnityEngine.Debug.Log(type);
			if(type == null) return (RequirementType)0;
			if(v >= type.requirements.Length) return (RequirementType)0;
			return type.requirements[v].req;
		}

		public RequirementType getAltReq(int v) {
			//UnityEngine.Debug.Log(type);
			if(type == null) return (RequirementType)0;
			if(v >= type.requirements.Length) return (RequirementType)0;
			return type.requirements[v].alt;
		}
	}
}
