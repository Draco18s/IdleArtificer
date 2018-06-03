using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.upgrades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.init {
	public static class SkillList {
		private static List<Skill> allSkills = new List<Skill>();

		public static SkillInteger GuildmasterRating = (SkillInteger)new SkillInteger("GuildmasterRating", 5, 250, 5).register();
		public static Skill Income = new Skill("Income", 0.1, 1, 1.5).register();
		public static Skill VendorEffectiveness = new Skill("VendorEffectiveness", 0.25, 10, 2).register();
		public static Skill ResearchRate = new Skill("ResearchRate", 0.025, 5, 1.05).register();
		public static Skill ClickRate = new SkillClicks("ClickRate", 0.25, 2, 1.1).register();
		public static Skill RenownMulti = new Skill("RenownMulti", 0.05, 25, 1.1).register();
		public static SkillIndustryRank RawType = (SkillIndustryRank)new SkillIndustryRank("rank_raw", 0.25, 1, 1.25, Scalar._0_RAW).register();
		public static SkillIndustryRank RefinedType = (SkillIndustryRank)new SkillIndustryRank("rank_refined", 0.25, 1, 1.25, Scalar._1_REFINED).register();
		public static SkillIndustryRank SimpleType = (SkillIndustryRank)new SkillIndustryRank("rank_simple", 0.25, 1, 1.25, Scalar._2_SIMPLE).register();
		public static SkillIndustryRank ComplexType = (SkillIndustryRank)new SkillIndustryRank("rank_complex", 0.25, 1, 1.25, Scalar._3_COMPLEX).register();
		public static SkillIndustryRank RareType = (SkillIndustryRank)new SkillIndustryRank("rank_rare", 0.25, 1, 1.25, Scalar._4_RARE).register();
		public static Skill QuestTime = new SkillInteger("QuestTime", 60, 10, 1.2).register();

		public static void register(Skill s) {
			allSkills.Add(s);
		}

		public static IEnumerator<Skill> getSkillList() {
			return allSkills.GetEnumerator();
		}

		public static BigRational getScalarTypeMulti(Scalar scalar) {
			if(scalar == Scalar._0_RAW) return 1 + RawType.getMultiplier(scalar);
			if(scalar == Scalar._1_REFINED) return 1 + RefinedType.getMultiplier(scalar);
			if(scalar == Scalar._2_SIMPLE) return 1 + SimpleType.getMultiplier(scalar);
			if(scalar == Scalar._3_COMPLEX) return 1 + ComplexType.getMultiplier(scalar);
			if(scalar == Scalar._4_RARE) return 1 + RareType.getMultiplier(scalar);
			return 1;
			//return 1 + RawType.getMultiplier(scalar) + RefinedType.getMultiplier(scalar) + SimpleType.getMultiplier(scalar) + ComplexType.getMultiplier(scalar) + RareType.getMultiplier(scalar);
		}

		public static void writeSaveData(ref SerializationInfo info, ref StreamingContext context) {
			foreach(Skill s in allSkills) {
				info.AddValue(s.name, s.getRanks());
			}
		}

		public static void readSaveData(ref SerializationInfo info, ref StreamingContext context) {
			SerializationInfoEnumerator infoEnum = info.GetEnumerator();
			Hashtable values = new Hashtable();
			while(infoEnum.MoveNext()) {
				SerializationEntry val = infoEnum.Current;
				values.Add(val.Name, val.Value);
			}
			foreach(Skill s in allSkills) {
				if(values.Contains(s.name)) {
					s.setRanks((int)values[s.name]);
				}
			}
		}
	}
}
