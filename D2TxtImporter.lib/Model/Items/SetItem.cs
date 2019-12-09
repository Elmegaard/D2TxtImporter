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
                    Importer.LogException(new Exception($"Could not find item level for '{name}' in SetItems.txt"));
                }

                var requiredLevel = Utility.ToNullableInt(row["lvl req"]);
                if (!requiredLevel.HasValue)
                {
                    Importer.LogException(new Exception($"Could not find required level for '{name}' in SetItems.txt"));
                }

                var setItem = new SetItem
                {
                    Index = name,
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
                    Importer.LogException(new Exception($"Could not find code '{setItem.Code}' in Weapons.txt, Armor.txt, or Misc.txt for set item '{setItem.Name}' in SetItems.txt"));
                }

                setItem.Equipment = eq;
                setItem.Type = eq.Type.Index;

                var propList = new List<PropertyInfo>();
                // Add the properties
                for (int i = 1; i <= 9; i++)
                {
                    propList.Add(new PropertyInfo(row[$"prop{i}"], row[$"par{i}"], row[$"min{i}"], row[$"max{i}"]));
                }

                try
                {
                    var properties = ItemProperty.GetProperties(propList, setItem.ItemLevel).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                    setItem.Properties = properties;
                }
                catch (Exception e)
                {
                    Importer.LogException(new Exception($"Could not get properties for item '{setItem.Name}' in SetItems.txt", e));
                }

                propList = new List<PropertyInfo>();
                // Add the properties
                for (int i = 1; i <= 5; i++)
                {
                    propList.Add(new PropertyInfo(row[$"aprop{i}a"], row[$"apar{i}a"], row[$"amin{i}a"], row[$"amax{i}a"]));
                    propList.Add(new PropertyInfo(row[$"aprop{i}b"], row[$"apar{i}b"], row[$"amin{i}b"], row[$"amax{i}b"]));
                }

                try
                {
                    var setProperties = ItemProperty.GetProperties(propList, setItem.ItemLevel).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                    setItem.SetProperties = setProperties;
                }
                catch (Exception e)
                {
                    Importer.LogException(new Exception($"Could not get set properties for item '{setItem.Name}' in SetItems.txt", e));
                }

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
