using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class CharStat
    {
        public string Class { get; set; }
        public string StrAllSkills { get; set; }
        public string StrSkillTab1 { get; set; }
        public string StrSkillTab2 { get; set; }
        public string StrSkillTab3 { get; set; }

        public static Dictionary<string, CharStat> CharStats;

        public static void Import(string excelFolder)
        {
            CharStats = new Dictionary<string, CharStat>();

            var lines = Importer.ReadCsvFile(excelFolder + "/CharStats.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var charStat = new CharStat
                {
                    Class = values[0],
                    StrAllSkills = values[43],
                    StrSkillTab1 = values[44],
                    StrSkillTab2 = values[45],
                    StrSkillTab3 = values[46],
                };

                CharStats[charStat.Class.ToLower().Substring(0,3)] = charStat;
            }
        }

        public override string ToString()
        {
            return Class;
        }
    }
}
