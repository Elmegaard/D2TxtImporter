using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class Set
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public List<SetItem> SetItems { get; set; }
        public List<ItemProperty> PartialProperties { get; set; }
        public List<ItemProperty> FullProperties { get; set; }
        public int Level { get; set; }

        public static List<Set> Import(string excelFolder)
        {
            var result = new List<Set>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Sets.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var set = new Set
                {
                    Index = values[0],
                    Name = values[1]
                };

                var level = Utility.ToNullableInt(values[3]);
                if (!level.HasValue)
                {
                    throw new Exception($"Could not get level of set {set.Name} from Sets.txt");
                }

                set.Level = level.Value;

                var propArray = values.Skip(4).ToArray();
                propArray = propArray.Take(36).ToArray();
                var properties = ItemProperty.GetProperties(propArray, set.Level).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                set.PartialProperties = properties.OrderBy(x => x.Index).ToList();

                propArray = values.Skip(40).ToArray();
                propArray = propArray.Take(propArray.Length - 1).ToArray();
                var propertiesFull = ItemProperty.GetProperties(propArray, set.Level).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                set.FullProperties = propertiesFull;

                set.SetItems = SetItem.SetItems.Where(x => x.Set == set.Index).ToList();

                result.Add(set);
            }

            return result.OrderBy(x => x.Level).ToList();
        }
    }
}
