using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class Runeword : Item
    {
        public List<Misc> Runes { get; set; }
        public List<ItemType> Types { get; set; }

        public static List<Runeword> Import(string excelFolder)
        {
            var result = new List<Runeword>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/Runes.txt");

            foreach (var row in table)
            {
                if (string.IsNullOrEmpty(row["Rune1"]))
                {
                    continue;
                }

                // Add the properties
                var propArray = new string[] {
                    row["T1Code1"], row["T1Param1"], row["T1Min1"], row["T1Max1"],
                    row["T1Code2"], row["T1Param2"], row["T1Min2"], row["T1Max2"],
                    row["T1Code3"], row["T1Param3"], row["T1Min3"], row["T1Max3"],
                    row["T1Code4"], row["T1Param4"], row["T1Min4"], row["T1Max4"],
                    row["T1Code5"], row["T1Param5"], row["T1Min5"], row["T1Max5"],
                    row["T1Code6"], row["T1Param6"], row["T1Min6"], row["T1Max6"],
                    row["T1Code7"], row["T1Param7"], row["T1Min7"], row["T1Max7"]
                };

                var properties = ItemProperty.GetProperties(propArray).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();

                // Add the runes
                var runeArray = new string[] { row["Rune1"], row["Rune2"], row["Rune3"], row["Rune4"], row["Rune5"], row["Rune6"] };
                var runes = new List<Misc>();

                for (int i = 0; i < runeArray.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(runeArray[i]) && !runeArray[i].StartsWith("*"))
                    {
                        runes.Add(Misc.MiscItems[runeArray[i]]);
                    }
                }

                // Add the types
                var typeArray = new string[] { row["itype1"], row["itype2"], row["itype3"], row["itype4"], row["itype5"], row["itype6"] };
                var types = new List<ItemType>();

                for (int i = 0; i < typeArray.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(typeArray[i]) && !typeArray[i].StartsWith("*"))
                    {
                        types.Add(ItemType.ItemTypes[typeArray[i]]);
                    }
                }

                var runeword = new Runeword
                {
                    Name = row["Rune Name"],
                    Enabled = true,
                    ItemLevel = runes.Max(x => x.ItemLevel),
                    RequiredLevel = runes.Max(x => x.RequiredLevel),
                    Code = row["Name"],
                    Types = types,
                    Runes = runes,
                    Properties = properties
                };

                if (properties.Count > 0)
                {
                    result.Add(runeword);
                }
            }

            return result.OrderBy(x => x.RequiredLevel).ToList();
        }
    }
}
