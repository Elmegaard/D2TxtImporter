using System;
using System.Collections.Generic;
using System.Linq;

namespace D2TxtImporter.lib.Model
{
    public class Unique : Item
    {
        public string Type { get; set; }

        public static List<Unique> Import(string excelFolder)
        {
            var result = new List<Unique>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/UniqueItems.txt");

            foreach (var row in table)
            {
                if (string.IsNullOrEmpty(row["lvl"]))
                {
                    continue;
                }

                var name = row["index"];

                var itemLevel = Utility.ToNullableInt(row["lvl"]);
                if (!itemLevel.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find item level for '{name}' in UniqueItems.txt"));
                }

                var requiredLevel = Utility.ToNullableInt(row["lvl req"]);
                if (!requiredLevel.HasValue)
                {
                    ExceptionHandler.LogException(new Exception($"Could not find required level for '{name}' in UniqueItems.txt"));
                }

                var code = row["code"];
                var unique = new Unique
                {
                    Index = name,
                    Enabled = row["enabled"] == "1",
                    ItemLevel = itemLevel.Value,
                    RequiredLevel = requiredLevel.Value,
                    Code = code,
                    DamageArmorEnhanced = false
                };

                Equipment eq = null;

                if (Armor.Armors.ContainsKey(code))
                {
                    eq = (Armor)Armor.Armors[code].Clone();
                }
                else if (Weapon.Weapons.ContainsKey(code))
                {
                    eq = (Weapon)Weapon.Weapons[code].Clone();
                }
                else if (Misc.MiscItems.ContainsKey(code))
                {
                    var misc = Misc.MiscItems[code];

                    eq = new Equipment
                    {
                        Code = misc.Code,
                        EquipmentType = EquipmentType.Jewelery,
                        Type = misc.Type
                    };
                }
                else
                {
                    ExceptionHandler.LogException(new Exception($"Could not find code '{code}' in Weapons.txt, Armor.txt, or Misc.txt for set item '{unique.Name}' in UniqueItems.txt"));
                }

                unique.Equipment = eq;
                unique.Type = eq.Type.Index;

                var propList = new List<PropertyInfo>();
                // Add the properties
                for (int i = 1; i <= 12; i++)
                {
                    propList.Add(new PropertyInfo(row[$"prop{i}"], row[$"par{i}"], row[$"min{i}"], row[$"max{i}"]));
                }

                try
                {
                    var properties = ItemProperty.GetProperties(propList, unique.ItemLevel).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                    unique.Properties = properties;
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(new Exception($"Could not get properties for unique '{unique.Name}' in UniqueItems.txt", e));
                }

                AddDamageArmorString(unique);

                result.Add(unique);
            }

            return result.OrderBy(x => x.RequiredLevel).ToList();
        }

        public static void AddDamageArmorString(Item unique)
        {
            if (unique == null)
            {
                throw new Exception("The unique item does not exist, something is wrong with one of your unique items.");
            }

            if (unique.Equipment == null)
            {
                throw new Exception($"Could not find equipment for item '{unique.Name}'");
            }

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
                    switch (armor.Type.Equiv1)
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
