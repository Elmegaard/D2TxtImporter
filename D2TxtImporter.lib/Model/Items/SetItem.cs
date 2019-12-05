using Newtonsoft.Json;
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

        [JsonIgnore]
        public List<ItemProperty> SetProperties { get; set; }

        public List<string> SetPropertiesString { get; set; }

        [JsonIgnore]
        public static List<SetItem> SetItems { get; set; }

        [JsonIgnore]
        public int AddFunc { get; set; }

        public static void Import(string excelFolder)
        {
            SetItems = new List<SetItem>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/SetItems.txt");

            foreach (var row in table)
            {
                var addFunc = Utility.ToNullableInt(row["add func"]);
                var name = row["index"];

                var itemLevel = Utility.ToNullableInt(row["lvl"]);
                if (!itemLevel.HasValue)
                {
                    throw new Exception($"Could not find item level for '{name}' in SetItems.txt");
                }

                var requiredLevel = Utility.ToNullableInt(row["lvl req"]);
                if (!requiredLevel.HasValue)
                {
                    throw new Exception($"Could not find required level for '{name}' in SetItems.txt");
                }

                var setItem = new SetItem
                {
                    Name = name,
                    Set = row["set"],
                    Enabled = true,
                    ItemLevel = itemLevel.Value,
                    RequiredLevel = requiredLevel.Value,
                    Code = row["item"],
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
                setItem.Type = eq.Type.Name;

                // Add the properties
                var propArray = new string[] {
                    row["prop1"], row["par1"], row["min1"], row["max1"],
                    row["prop2"], row["par2"], row["min2"], row["max2"],
                    row["prop3"], row["par3"], row["min3"], row["max3"],
                    row["prop4"], row["par4"], row["min4"], row["max4"],
                    row["prop5"], row["par5"], row["min5"], row["max5"],
                    row["prop6"], row["par6"], row["min6"], row["max6"],
                    row["prop7"], row["par7"], row["min7"], row["max7"],
                    row["prop8"], row["par8"], row["min8"], row["max8"],
                    row["prop9"], row["par9"], row["min9"], row["max9"]
                };
                var properties = ItemProperty.GetProperties(propArray, setItem.ItemLevel).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                setItem.Properties = properties;

                var setPropArray = new string[] {
                    row["aprop1a"], row["apar1a"], row["amin1a"], row["amax1a"], row["aprop1b"], row["apar1b"], row["amin1b"], row["amax1b"],
                    row["aprop2a"], row["apar2a"], row["amin2a"], row["amax2a"], row["aprop2b"], row["apar2b"], row["amin2b"], row["amax2b"],
                    row["aprop3a"], row["apar3a"], row["amin3a"], row["amax3a"], row["aprop3b"], row["apar3b"], row["amin3b"], row["amax3b"],
                    row["aprop4a"], row["apar4a"], row["amin4a"], row["amax4a"], row["aprop4b"], row["apar4b"], row["amin4b"], row["amax4b"],
                    row["aprop5a"], row["apar5a"], row["amin5a"], row["amax5a"], row["aprop5b"], row["apar5b"], row["amin5b"], row["amax5b"]
                };
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
