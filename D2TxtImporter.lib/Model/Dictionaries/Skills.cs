using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class Skill
    {
        public string Name { get; set; }
        public int? Id { get; set; }
        public string CharClass { get; set; }
        public string SkillDesc { get; set; }

        private static Dictionary<int?, Skill> IdSkillDictionary;
        private static Dictionary<string, Skill> NameSkillDictionary;
        private static Dictionary<string, Skill> DescSkillDictionary;

        public static void Import(string excelFolder)
        {
            IdSkillDictionary = new Dictionary<int?, Skill>();
            NameSkillDictionary = new Dictionary<string, Skill>();
            DescSkillDictionary = new Dictionary<string, Skill>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Skills.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var skill = new Skill
                {
                    Name = values[0],
                    Id = Utility.ToNullableInt(values[1]),
                    CharClass = values[2],
                    SkillDesc = values[3]
                };

                IdSkillDictionary[skill.Id] = skill;
                NameSkillDictionary[skill.Name] = skill;
                DescSkillDictionary[skill.SkillDesc] = skill;
            }
        }

        public static Skill GetSkill(string skill)
        {
            if (Utility.ToNullableInt(skill).HasValue)
            {
                return IdSkillDictionary[Utility.ToNullableInt(skill)];
            }
            else if (NameSkillDictionary.ContainsKey(skill))
            {
                return NameSkillDictionary[skill];
            }
            else
            {
                return DescSkillDictionary[skill];
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
