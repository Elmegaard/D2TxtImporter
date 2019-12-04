using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    class Armor : Equipment
    {
        [JsonIgnore]
        public int MinAc { get; set; }
        [JsonIgnore]
        public int MaxAc { get; set; }
        [JsonIgnore]
        public int? MinDamage { get; set; }
        [JsonIgnore]
        public int? MaxDamage { get; set; }
        public string DamageString { get; set; }
        public string DamageStringPrefix { get; set; }

        [JsonIgnore]
        public static Dictionary<string, Armor> Armors;
        public string ArmorString { get; set; }

        public static void Import(string excelFolder)
        {
            Armors = new Dictionary<string, Armor>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Armor.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var name = values[0];

                if (!ItemType.ItemTypes.ContainsKey(values[48]))
                {
                    throw new Exception($"Could not find type '{values[48]}' in ItemTypes.txt for armor '{name}' in Armor.txt");

                }
                var type = ItemType.ItemTypes[values[48]];

                var minAc = Utility.ToNullableInt(values[5]);
                if (!minAc.HasValue)
                {
                    throw new Exception($"Could not find MinAC for armor '{name}' in Armor.txt");
                }

                var maxAc = Utility.ToNullableInt(values[6]);
                if (!maxAc.HasValue)
                {
                    throw new Exception($"Could not find MaxAC for armor '{name}' in Armor.txt");
                }

                var requiredStrength = Utility.ToNullableInt(values[9]);
                if (!requiredStrength.HasValue)
                {
                    throw new Exception($"Could not find Required Strength for armor '{name}' in Armor.txt");
                }

                var durability = Utility.ToNullableInt(values[11]);
                if (!durability.HasValue)
                {
                    throw new Exception($"Could not find Durability for armor '{name}' in Armor.txt");
                }


                var itemLevel = Utility.ToNullableInt(values[13]);
                if (!durability.HasValue)
                {
                    throw new Exception($"Could not find Item Level for armor '{name}' in Armor.txt");
                }

                var armor = new Armor
                {
                    Name = name,
                    Code = values[17],
                    MinAc = minAc.Value,
                    MaxAc = maxAc.Value,
                    RequiredStrength = requiredStrength.Value,
                    Type = type,
                    EquipmentType = EquipmentType.Armor,
                    Durability = durability.Value,
                    MinDamage = Utility.ToNullableInt(values[63]),
                    MaxDamage = Utility.ToNullableInt(values[64]),
                    ItemLevel = itemLevel.Value
                };

                Armors[armor.Code] = armor;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
