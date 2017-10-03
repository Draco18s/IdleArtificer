using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.upgrades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.draco18s.artificer.init {
	public static class SkillList {
		private static List<Skill> allSkills = new List<Skill>();

		public static SkillIndustryRank RawType = (SkillIndustryRank)new SkillIndustryRank("rank_raw", 0.25, 1, 1.25, Scalar._0_RAW).register();
		public static SkillIndustryRank RefinedType = (SkillIndustryRank)new SkillIndustryRank("rank_refined", 0.25, 1, 1.25, Scalar._1_REFINED).register();
		public static SkillIndustryRank SimpleType = (SkillIndustryRank)new SkillIndustryRank("rank_simple", 0.25, 1, 1.25, Scalar._2_SIMPLE).register();
		public static SkillIndustryRank ComplexType = (SkillIndustryRank)new SkillIndustryRank("rank_complex", 0.25, 1, 1.25, Scalar._3_COMPLEX).register();
		public static SkillIndustryRank RareType = (SkillIndustryRank)new SkillIndustryRank("rank_rare", 0.25, 1, 1.25, Scalar._4_RARE).register();
		public static Skill Income = new Skill("income", 0.1, 1, 1.5).register();
		public static SkillInteger GuildmasterRating = (SkillInteger)new SkillInteger("guildmaster_rating", 5, 250, 5).register();

		public static void register(Skill s) {
			allSkills.Add(s);
		}

		public static IEnumerator<Skill> getSkillList() {
			return allSkills.GetEnumerator();
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
