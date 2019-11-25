using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class Unique : Item
    {
        public string Type { get; set; }
        public Equipment Equipment { get; set; }


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

                // Add the properties (there are 12)
                var propArray = values.Skip(21).ToArray();
                propArray = propArray.Take(propArray.Count() - 1).ToArray();

                var properties = new List<ItemProperty>();

                for (int i = 0; i < propArray.Count(); i += 4)
                {
                    if (!string.IsNullOrEmpty(propArray[i]) && !propArray[i].StartsWith("*"))
                    {
                        var prop = new ItemProperty(propArray[i], propArray[i + 1], ToNullableInt(propArray[i + 2]), ToNullableInt(propArray[i + 3]));
                        properties.Add(prop);
                    }
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
                    Properties = properties,
                    Equipment = eq
                };

                result.Add(unique);
            }

            return result;
        }
    }
}
