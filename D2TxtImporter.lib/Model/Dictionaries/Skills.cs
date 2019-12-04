using Newtonsoft.Json;
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

        [JsonIgnore]
        public int? Id { get; set; }
        public string CharClass { get; set; }

        [JsonIgnore]
        public string SkillDesc { get; set; }
        public int RequiredLevel { get; set; }

        [JsonIgnore]
        private static Dictionary<int?, Skill> IdSkillDictionary;
        [JsonIgnore]
        private static Dictionary<string, Skill> NameSkillDictionary;
        [JsonIgnore]
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

                var reqLevel = Utility.ToNullableInt(values[152]);
                if (!reqLevel.HasValue || reqLevel.Value < 1)
                {
                    throw new Exception($"Invalid required level for skill '{values[0]}' in Skills.txt, should be an integer value 1 or above");
                }

                var skill = new Skill
                {
                    Name = values[0],
                    Id = Utility.ToNullableInt(values[1]),
                    CharClass = values[2],
                    SkillDesc = values[3],
                    RequiredLevel = reqLevel.Value
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
            else if (DescSkillDictionary.ContainsKey(skill))
            {
                return DescSkillDictionary[skill];
            }
            else
            {
                throw new Exception($"Could not find skill with id, name, or description '{skill}'");
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
