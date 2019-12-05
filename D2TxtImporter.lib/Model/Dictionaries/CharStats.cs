using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class CharStat
    {
        [JsonIgnore]
        public string Class { get; set; }
        [JsonIgnore]
        public string StrAllSkills { get; set; }
        [JsonIgnore]
        public string StrSkillTab1 { get; set; }
        [JsonIgnore]
        public string StrSkillTab2 { get; set; }
        [JsonIgnore]
        public string StrSkillTab3 { get; set; }

        [JsonIgnore]
        public static Dictionary<string, CharStat> CharStats;

        [JsonIgnore]
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

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/CharStats.txt");

            foreach (var row in table)
            {
                var charStat = new CharStat
                {
                    Class = row["class"].ToString(),
                    StrAllSkills = row["StrAllSkills"],
                    StrSkillTab1 = row["StrSkillTab1"],
                    StrSkillTab2 = row["StrSkillTab2"],
                    StrSkillTab3 = row["StrSkillTab3"]
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
