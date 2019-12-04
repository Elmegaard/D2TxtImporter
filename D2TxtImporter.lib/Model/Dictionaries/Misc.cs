using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            var lines = Importer.ReadCsvFile(excelFolder + "/Misc.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[5]))
                {
                    continue;
                }

                var name = values[1];

                if (!ItemType.ItemTypes.ContainsKey(values[32]))
                {
                    throw new Exception($"Could not find code '{values[32]}' in ItemTypes.txt for Misc.txt item {name}");
                }

                var itemLevel = Utility.ToNullableInt(values[5]);
                if (!itemLevel.HasValue)
                {
                    throw new Exception($"Could not find item level for '{name}' in Misc.txt");
                }

                var requiredLevel = Utility.ToNullableInt(values[6]);
                if (!requiredLevel.HasValue)
                {
                    throw new Exception($"Could not find required level for '{name}' in Misc.txt");
                }

                var misc = new Misc
                {
                    Name = values[1],
                    ItemLevel = itemLevel.Value,
                    RequiredLevel = requiredLevel.Value,
                    Code = values[13],
                    Type = ItemType.ItemTypes[values[32]],
                    Type2 = values[33]
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
