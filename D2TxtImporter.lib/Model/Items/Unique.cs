using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class Unique : Item
    {
        public string Type { get; set; }
        public Equipment Equipment { get; set; }
        public bool DamageArmorEnhanced { get; set; }

        public static List<Unique> Import(string excelFolder)
        {
            var result = new List<Unique>();

            var lines = Importer.ReadCsvFile(excelFolder + "/UniqueItems.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[6]))
                {
                    continue;
                }


                Equipment eq;
                var code = values[8];

                if (Armor.Armors.ContainsKey(code))
                {
                    eq = Armor.Armors[code];
                }
                else if (Weapon.Weapons.ContainsKey(code))
                {
                    eq = Weapon.Weapons[code];
                }
                else
                {
                    var misc = Misc.MiscItems[code];

                    eq = new Equipment
                    {
                        Code = misc.Code,
                        EquipmentType = EquipmentType.Jewelery,
                        Name = misc.Name
                    };
                }

                var unique = new Unique
                {
                    Name = values[0],
                    Enabled = values[2] == "1",
                    ItemLevel = int.Parse(values[6]),
                    RequiredLevel = int.Parse(values[7]),
                    Code = values[8],
                    Type = values[9],
                    Equipment = eq,
                    DamageArmorEnhanced = false
                };

                // Add the properties (there are 12)
                var propArray = values.Skip(21).ToArray();
                propArray = propArray.Take(propArray.Count() - 1).ToArray();

                var properties = ItemProperty.GetProperties(propArray, unique.Equipment.ItemLevel);

                unique.Properties = properties;


                AddDamageArmorString(unique);

                result.Add(unique);
            }

            return result;
        }

        private static void AddDamageArmorString(Unique unique)
        {
            if (unique.Equipment.EquipmentType == EquipmentType.Weapon)
            {
                var weapon = unique.Equipment as Weapon;

                foreach (var damageType in weapon.DamageTypes)
                {
                    int minDam1 = damageType.MinDamage;
                    int minDam2 = damageType.MinDamage;
                    int maxDam1 = damageType.MaxDamage;
                    int maxDam2 = damageType.MaxDamage;

                    foreach (var property in unique.Properties.OrderBy(x => x.Property.Code))
                    {
                        switch (property.Property.Code)
                        {
                            case "dmg%":
                                minDam1 = (int)(property.Min / 100f * damageType.MinDamage + damageType.MinDamage);
                                minDam2 = (int)(property.Max / 100f * damageType.MinDamage + damageType.MinDamage);

                                maxDam1 = (int)(property.Min / 100f * damageType.MaxDamage + damageType.MaxDamage);
                                maxDam2 = (int)(property.Max / 100f * damageType.MaxDamage + damageType.MaxDamage);

                                unique.DamageArmorEnhanced = true;
                                break;
                            case "dmg-norm":
                                minDam1 += property.Min.Value;
                                minDam2 += property.Min.Value;

                                maxDam1 += property.Max.Value;
                                maxDam2 += property.Max.Value;
                                unique.DamageArmorEnhanced = true;
                                break;
                            case "dmg-min":
                                minDam1 += property.Min.Value;
                                minDam2 += property.Max.Value;
                                unique.DamageArmorEnhanced = true;
                                break;
                            case "dmg-max":
                                maxDam1 += property.Min.Value;
                                maxDam2 += property.Max.Value;
                                unique.DamageArmorEnhanced = true;
                                break;
                        }
                    }

                    if (minDam1 == minDam2)
                    {
                        damageType.DamageString = $"{minDam1} to {maxDam1}";
                    }
                    else
                    {
                        damageType.DamageString = $"({minDam1}-{minDam2}) to ({maxDam1}-{maxDam2})";
                    }
                }
            }
            else if (unique.Equipment.EquipmentType == EquipmentType.Armor)
            {
                // Calculate armor
                var armor = unique.Equipment as Armor;

                int minAc = armor.MaxAc;
                int maxAc = armor.MaxAc;

                foreach (var property in unique.Properties.OrderByDescending(x => x.Property.Code))
                {
                    switch (property.Property.Code)
                    {
                        case "ac%":
                            minAc = (int)Math.Floor(((minAc + 1) * (100f + property.Min) / 100f).Value);
                            maxAc = (int)Math.Floor(((maxAc + 1) * (100f + property.Max) / 100f).Value);
                            unique.DamageArmorEnhanced = true;
                            break;
                        case "ac":
                            minAc += property.Min.Value;
                            maxAc += property.Max.Value;
                            unique.DamageArmorEnhanced = true;
                            break;
                    }
                }

                if (minAc == maxAc)
                {
                    armor.ArmorString = $"{maxAc}";
                }
                else
                {
                    armor.ArmorString = $"{minAc}-{maxAc}";
                }

                // Handle smite damage
                if (armor.MinDamage.HasValue && armor.MinDamage.Value > 0)
                {
                    switch (armor.Type)
                    {
                        case "shie":
                        case "ashd": // pala shield
                            armor.DamageStringPrefix = "Smite Damage";
                            break;
                        case "boot":
                            armor.DamageStringPrefix = "Kick Damage";
                            break;
                        default:
                            armor.DamageStringPrefix = "Unhandled Damage Prefix";
                            break;
                    }

                    if (armor.MinDamage.Value == armor.MaxDamage.Value)
                    {
                        armor.DamageString = $"{armor.MinDamage.Value}";
                    }
                    else
                    {
                        armor.DamageString = $"{armor.MinDamage.Value} to {armor.MaxDamage.Value}";
                    }
                }
            }

            // Handle max durability
            var dur = unique.Properties.FirstOrDefault(x => x.Property.Code == "dur");
            if (dur != null)
            {
                unique.Equipment.Durability += dur.Min.Value;
                unique.Properties.Remove(dur);
            }

            // Handle indestructible items durability
            if (unique.Properties.Any(x => x.Property.Code == "indestruct"))
            {
                unique.Equipment.Durability = 0;
            }
        }
    }
}
