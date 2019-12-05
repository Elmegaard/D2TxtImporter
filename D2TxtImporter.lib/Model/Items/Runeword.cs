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
                    Runes = runes
                };

                var propList = new List<PropertyInfo>();
                // Add the properties
                for (int i = 1; i <= 7; i++)
                {
                    propList.Add(new PropertyInfo(row[$"T1Code{i}"], row[$"T1Param{i}"], row[$"T1Min{i}"], row[$"T1Max{i}"]));
                }

                try
                {
                    var properties = ItemProperty.GetProperties(propList).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                    runeword.Properties = properties;
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not get properties for runeword '{runeword.Name}' in Runes.txt", e);
                }

                if (runeword.Properties.Count > 0)
                {
                    result.Add(runeword);
                }
            }

            return result.OrderBy(x => x.RequiredLevel).ToList();
        }
    }
}
