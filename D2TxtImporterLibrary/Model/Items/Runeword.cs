using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class Runeword : Item
    {
        public List<Misc> Runes { get; set; }
        public List<ItemType> Types { get; set; }

        public static List<Runeword> Import(string excelFolder)
        {
            var result = new List<Runeword>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Runes.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[14]))
                {
                    continue;
                }

                // Add the properties
                var propArray = values.Skip(20).ToArray();
                propArray = propArray.Take(propArray.Count() - 1).ToArray();

                var properties = ItemProperty.GetProperties(propArray);

                // Add the runes
                var runeArray = values.Skip(14).ToArray();
                runeArray = runeArray.Take(6).ToArray(); // Max 6 runes

                var runes = new List<Misc>();

                for (int i = 0; i < runeArray.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(runeArray[i]) && !runeArray[i].StartsWith("*"))
                    {
                        runes.Add(Misc.MiscItems[runeArray[i]]);
                    }
                }

                // Add the types
                var typeArray = values.Skip(4).ToArray();
                typeArray = typeArray.Take(6).ToArray(); // Max 6 runes

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
                    Name = values[1],
                    Enabled = true,
                    ItemLevel = runes.Max(x => x.ItemLevel),
                    RequiredLevel = runes.Max(x => x.RequiredLevel),
                    Code = values[0],
                    Types = types,
                    Runes = runes,
                    Properties = properties
                };

                if (properties.Count > 0)
                {
                    result.Add(runeword);
                }
            }

            return result;
        }
    }
}
