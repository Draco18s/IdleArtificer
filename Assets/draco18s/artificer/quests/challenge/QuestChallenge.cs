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
				int dmg = ((IMonsterChallenge)type).getRangedDamage(result, theQuest, ref questBonus, rangedStack);
				if(rangedStack != null) {
					if(rangedStack.doesStackHave(Enchantments.ENHANCEMENT)) {
						dmg += 5;
					}
					rangedStack.stackSize--;
				}
				dmg = (int)Math.Round(rangedStack.getEffectiveness(RequirementType.RANGED) * dmg);
				monsterHealth -= dmg;
			}
			type.OnAttempt(result, theQuest, ref questBonus);
			if(type is IMonsterChallenge) {
				ItemStack meleeStack = theQuest.getHeroItemWith(AidType.WEAPON);
				int dmg = ((IMonsterChallenge)type).getDamageDealtToMonster(result, theQuest, ref questBonus, meleeStack);
				if(meleeStack.doesStackHave(Enchantments.ENHANCEMENT)) {
					dmg += 5;
				}
				if(meleeStack.doesStackHave(Enchantments.KEEN)) {
					if(theQuest.testLuck(5) == 0) {
						dmg *= 2;
					}
				}
				dmg = (int)Math.Round(meleeStack.getEffectiveness(RequirementType.WEAPON) * dmg);
				monsterHealth -= dmg;
				if(monsterHealth > 0) {
					theQuest.repeatTask();
				}
				else {
					((IMonsterChallenge)type).getLootDrops(result, theQuest, ref questBonus);
				}
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
	}
}
