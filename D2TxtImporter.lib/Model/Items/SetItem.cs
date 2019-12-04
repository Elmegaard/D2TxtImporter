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
        public List<string> SetPropertiesString { get; set; }
        public static List<SetItem> SetItems { get; set; }
        public int AddFunc { get; set; }

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

                var addFunc = Utility.ToNullableInt(values[16]);
                var name = values[0];

                var itemLevel = Utility.ToNullableInt(values[5]);
                if (!itemLevel.HasValue)
                {
                    throw new Exception($"Could not find item level for '{name}' in SetItems.txt");
                }

                var requiredLevel = Utility.ToNullableInt(values[6]);
                if (!requiredLevel.HasValue)
                {
                    throw new Exception($"Could not find required level for '{name}' in SetItems.txt");
                }

                var setItem = new SetItem
                {
                    Name = name,
                    Set = values[1],
                    Enabled = true,
                    ItemLevel = itemLevel.Value,
                    RequiredLevel = requiredLevel.Value,
                    Code = values[2],
                    Type = values[3],
                    DamageArmorEnhanced = false,
                    AddFunc = addFunc.HasValue ? addFunc.Value : 0
                };

                Equipment eq = null;
                if (Armor.Armors.ContainsKey(setItem.Code))
                {
                    eq = Armor.Armors[setItem.Code];
                }
                else if (Weapon.Weapons.ContainsKey(setItem.Code))
                {
                    eq = Weapon.Weapons[setItem.Code];
                }
                else if (Misc.MiscItems.ContainsKey(setItem.Code))
                {
                    var misc = Misc.MiscItems[setItem.Code];

                    eq = new Equipment
                    {
                        Code = misc.Code,
                        EquipmentType = EquipmentType.Jewelery,
                        Name = misc.Name,
                        Type = misc.Type
                    };
                }
                else
                {
                    throw new Exception($"Could not find code '{setItem.Code}' in Weapons.txt, Armor.txt, or Misc.txt for set item '{setItem.Name}' in SetItems.txt");
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

            foreach (var setItem in SetItems)
            {
                SetPartialPropertyString(setItem);
            }
        }

        private static void SetPartialPropertyString(SetItem setItem)
        {
            setItem.SetPropertiesString = new List<string>();

            foreach (var prop in setItem.SetProperties)
            {
                switch (setItem.AddFunc)
                {
                    case 0:
                        setItem.Properties.Add(prop);
                        break;
                    case 1:
                        var setItems = SetItem.SetItems.Where(x => x.Set == setItem.Set && x.Name != setItem.Name).ToList();
                        var index = (int)Math.Floor(prop.Index / 2f);

                        setItem.SetPropertiesString.Add($"{prop.PropertyString} ({setItems[index].Name})");
                        break;
                    case 2:
                        setItem.SetPropertiesString.Add($"{prop.PropertyString} ({Math.Floor(prop.Index / 2f) + 2} Items)");
                        break;
                }
            }


        }
    }
}
