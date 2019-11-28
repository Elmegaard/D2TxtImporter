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

        public static Dictionary<int, string> SkillTabs;

        public static void Import(string excelFolder)
        {
            CharStats = new Dictionary<string, CharStat>();

            // Because the skill tabs doesn't match the .lst file..
            SkillTabs = new Dictionary<int, string>
            {
                {0, "StrSklTabItem3" },
                {1, "StrSklTabItem2" },
                {2, "StrSklTabItem1" },
                {3, "StrSklTabItem15" },
                {4, "StrSklTabItem14" },
                {5, "StrSklTabItem13" },
                {6, "StrSklTabItem9" },
                {7, "StrSklTabItem8" },
                {8, "StrSklTabItem7" },
                {9, "StrSklTabItem6" },
                {10, "StrSklTabItem5" },
                {11, "StrSklTabItem4" },
                {12, "StrSklTabItem12" },
                {13, "StrSklTabItem11" },
                {14, "StrSklTabItem10" },
                {15, "StrSklTabItem16" },
                {16, "StrSklTabItem17" },
                {17, "StrSklTabItem18" },
                {18, "StrSklTabItem19" },
                {19, "StrSklTabItem20" },
                {20, "StrSklTabItem21" }
            };

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
