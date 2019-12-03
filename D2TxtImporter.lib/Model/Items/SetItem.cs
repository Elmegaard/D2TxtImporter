using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class SetItem : Item
    {
        public string Type { get; set; }
        public string Set { get; set; }
        public List<ItemProperty> SetProperties { get; set; }
        public static List<SetItem> SetItems { get; set; }

        public static void Import(string excelFolder)
        {
            SetItems = new List<SetItem>();

            var lines = Importer.ReadCsvFile(excelFolder + "/SetItems.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var setItem = new SetItem
                {
                    Name = values[0],
                    Set = values[1],
                    Enabled = true,
                    ItemLevel = int.Parse(values[5]),
                    RequiredLevel = int.Parse(values[6]),
                    Code = values[2],
                    Type = values[3],
                    DamageArmorEnhanced = false
                };

                Equipment eq;
                if (Armor.Armors.ContainsKey(setItem.Code))
                {
                    eq = Armor.Armors[setItem.Code];
                }
                else if (Weapon.Weapons.ContainsKey(setItem.Code))
                {
                    eq = Weapon.Weapons[setItem.Code];
                }
                else
                {
                    if (!Misc.MiscItems.ContainsKey(setItem.Code))
                    {
                        throw new Exception($"Could not find code '{setItem.Code}' in Misc.txt for set item '{setItem.Name}'");
                    }

                    var misc = Misc.MiscItems[setItem.Code];

                    eq = new Equipment
                    {
                        Code = misc.Code,
                        EquipmentType = EquipmentType.Jewelery,
                        Name = misc.Name,
                        Type = misc.Type
                    };
                }

                setItem.Equipment = eq;

                var propArray = values.Skip(17).ToArray();
                propArray = propArray.Take(36).ToArray();
                var properties = ItemProperty.GetProperties(propArray, setItem.ItemLevel).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                setItem.Properties = properties;

                var setPropArray = values.Skip(53).ToArray();
                setPropArray = setPropArray.Take(setPropArray.Length - 1).ToArray();
                var setProperties = ItemProperty.GetProperties(setPropArray, setItem.ItemLevel).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                setItem.SetProperties = setProperties;

                Unique.AddDamageArmorString(setItem);

                SetItems.Add(setItem);
            }
        }
    }
}
