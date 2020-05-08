using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/Armor.txt");

            foreach (var row in table)
            {
                var name = row["name"];

                if (!ItemType.ItemTypes.ContainsKey(row["type"]))
                {
                    ExceptionHandler.LogException(new Exception($"Could not find type '{row["type"]}' in ItemTypes.txt for armor '{name}' in Armor.txt"));

                }
                var type = ItemType.ItemTypes[row["type"]];

                var minAc = Utility.ToNullableInt(row["minac"]);
                if (!minAc.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find MinAC for armor '{name}' in Armor.txt"));
                }

                var maxAc = Utility.ToNullableInt(row["minac"]);
                if (!maxAc.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find MaxAC for armor '{name}' in Armor.txt"));
                }

                var requiredStrength = Utility.ToNullableInt(row["reqstr"]);
                if (!requiredStrength.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find Required Strength for armor '{name}' in Armor.txt"));
                }

                var durability = Utility.ToNullableInt(row["durability"]);
                if (!durability.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find Durability for armor '{name}' in Armor.txt"));
                }


                var itemLevel = Utility.ToNullableInt(row["level"]);
                if (!durability.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find Item Level for armor '{name}' in Armor.txt"));
                }

                var armor = new Armor
                {
                    Code = row["code"],
                    MinAc = minAc.Value,
                    MaxAc = maxAc.Value,
                    RequiredStrength = requiredStrength.Value,
                    Type = type,
                    EquipmentType = EquipmentType.Armor,
                    Durability = durability.Value,
                    MinDamage = Utility.ToNullableInt(row["mindam"]),
                    MaxDamage = Utility.ToNullableInt(row["maxdam"]),
                    ItemLevel = itemLevel.Value
                };

                Armors[armor.Code] = armor;
            }
        }

        public new object Clone()
        {
            return new Armor
            {
                EquipmentType = this.EquipmentType,
                Code = this.Code,
                RequiredStrength = this.RequiredStrength,
                RequiredDexterity = this.RequiredDexterity,
                Durability = this.Durability,
                ItemLevel = this.ItemLevel,
                Type = this.Type,
                MinAc = this.MinAc,
                MaxAc = this.MaxAc,
                MinDamage = this.MinDamage,
                MaxDamage = this.MaxDamage,
                DamageString = this.DamageString,
                DamageStringPrefix = this.DamageStringPrefix,
                ArmorString = this.ArmorString
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
