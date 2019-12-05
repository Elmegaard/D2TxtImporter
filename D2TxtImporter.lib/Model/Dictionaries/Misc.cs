using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class Misc
    {
        public string Name { get; set; }
        public int ItemLevel { get; set; }
        public int RequiredLevel { get; set; }

        [JsonIgnore]
        public string Code { get; set; }

        public ItemType Type { get; set; }

        [JsonIgnore]
        public string Type2 { get; set; }

        [JsonIgnore]
        public static Dictionary<string, Misc> MiscItems;

        public static void Import(string excelFolder)
        {
            MiscItems = new Dictionary<string, Misc>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/Misc.txt");

            foreach (var row in table)
            {
                if (string.IsNullOrEmpty(row["code"]))
                {
                    continue;
                }

                var name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(row["name"].Replace(" Rune", ""));

                if (!ItemType.ItemTypes.ContainsKey(row["type"]))
                {
                    throw new Exception($"Could not find code '{row["type"]}' in ItemTypes.txt for type field in Misc.txt item {name}");
                }

                var itemLevel = Utility.ToNullableInt(row["level"]);
                if (!itemLevel.HasValue)
                {
                    throw new Exception($"Could not find item level for '{name}' in Misc.txt");
                }

                var requiredLevel = Utility.ToNullableInt(row["levelreq"]);
                if (!requiredLevel.HasValue)
                {
                    throw new Exception($"Could not find required level for '{name}' in Misc.txt");
                }

                var misc = new Misc
                {
                    Name = row["name"],
                    ItemLevel = itemLevel.Value,
                    RequiredLevel = requiredLevel.Value,
                    Code = row["code"],
                    Type = ItemType.ItemTypes[row["type"]],
                    Type2 = row["type2"]
                };

                MiscItems[misc.Code] = misc;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
