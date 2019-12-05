using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace D2TxtImporter.lib.Model
{
    public class Weapon : Equipment
    {
        public List<DamageType> DamageTypes { get; set; }

        [JsonIgnore]
        public static Dictionary<string, Weapon> Weapons;

        public static void Import(string excelFolder)
        {
            Weapons = new Dictionary<string, Weapon>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/Weapons.txt");

            foreach (var row in table)
            {
                var damageTypes = new List<DamageType>();

                var isOneOrTwoHanded = row["1or2handed"] == "1";
                var isTwoHanded = row["2handed"] == "1";
                var isThrown = !string.IsNullOrEmpty(row["minmisdam"]);
                var name = row["name"];

                if (!isTwoHanded)
                {
                    try
                    {
                        damageTypes.Add(new DamageType { Type = DamageTypeEnum.Normal, MinDamage = int.Parse(row["mindam"]), MaxDamage = int.Parse(row["mindam"]) });
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Could not get min or max damage for weapon: '{name}' in Weapons.txt");
                    }
                }
                else if (isOneOrTwoHanded)
                {
                    try
                    {
                        damageTypes.Add(new DamageType { Type = DamageTypeEnum.OneHanded, MinDamage = int.Parse(row["mindam"]), MaxDamage = int.Parse(row["mindam"]) });
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Could not get min or max one handed damage for weapon: '{name}' in Weapons.txt");
                    }
                }

                if (isTwoHanded)
                {
                    try
                    {
                        damageTypes.Add(new DamageType { Type = DamageTypeEnum.TwoHanded, MinDamage = int.Parse(row["2handmindam"]), MaxDamage = int.Parse(row["2handmaxdam"]) });
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Could not get min or max two handed damage for weapon: '{name}' in Weapons.txt");
                    }
                }

                if (isThrown)
                {
                    try
                    {
                        damageTypes.Add(new DamageType { Type = DamageTypeEnum.Thrown, MinDamage = int.Parse(row["minmisdam"]), MaxDamage = int.Parse(row["maxmisdam"]) });
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Could not get min or max thrown damage for weapon: '{name}' in Weapons.txt");
                    }
                }

                var itemLevel = Utility.ToNullableInt(row["level"]);
                if (!itemLevel.HasValue)
                {
                    throw new Exception($"Could not find item level for weapon '{name}' in Weapons.txt");
                }

                if (!ItemType.ItemTypes.ContainsKey(row["type"]))
                {
                    throw new Exception($"Could not find type '{row["type"]}' in ItemTypes.txt for weapon '{name}' in Weapons.txt");
                }

                var weapon = new Weapon
                {
                    Name = name,
                    DamageTypes = damageTypes,
                    Code = row["code"],
                    EquipmentType = EquipmentType.Weapon,
                    RequiredStrength = !string.IsNullOrEmpty(row["reqstr"]) ? int.Parse(row["reqstr"]) : 0,
                    RequiredDexterity = !string.IsNullOrEmpty(row["reqdex"]) ? int.Parse(row["reqdex"]) : 0,
                    Durability = row["nodurability"] == "1" ? 0 : int.Parse(row["durability"]),
                    ItemLevel = itemLevel.Value,
                    Type = ItemType.ItemTypes[row["type"]]
                };

                Weapons[weapon.Code] = weapon;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
