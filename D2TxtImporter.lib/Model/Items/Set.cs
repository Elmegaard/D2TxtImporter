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

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/Sets.txt");

            foreach (var row in table)
            {
                var set = new Set
                {
                    Index = row["index"],
                    Name = row["name"]
                };

                var level = Utility.ToNullableInt(row["level"]);
                if (!level.HasValue)
                {
                    throw new Exception($"Could not get level of set {set.Name} from Sets.txt");
                }

                set.Level = level.Value;

                var propList = new List<PropertyInfo>();
                // Add the properties
                for (int i = 2; i <= 5; i++)
                {
                    propList.Add(new PropertyInfo(row[$"PCode{i}a"], row[$"PParam{i}a"], row[$"PMin{i}a"], row[$"PMax{i}a"]));
                    propList.Add(new PropertyInfo(row[$"PCode{i}b"], row[$"PParam{i}b"], row[$"PMin{i}b"], row[$"PMax{i}b"]));
                }

                try
                {
                    var properties = ItemProperty.GetProperties(propList, set.Level).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                    set.PartialProperties = properties.OrderBy(x => x.Index).ToList();
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not get properties for set '{set.Name}' in Sets.txt", e);
                }

                propList = new List<PropertyInfo>();
                // Add the properties
                for (int i = 1; i <= 8; i++)
                {
                    propList.Add(new PropertyInfo(row[$"FCode{i}"], row[$"FParam{i}"], row[$"FMin{i}"], row[$"FMax{i}"]));
                }

                try
                {
                    var propertiesFull = ItemProperty.GetProperties(propList, set.Level).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                    set.FullProperties = propertiesFull;
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not get properties for set '{set.Name}' in Sets.txt", e);
                }

                set.SetItems = SetItem.SetItems.Where(x => x.Set == set.Index).ToList();

                result.Add(set);
            }

            return result.OrderBy(x => x.Level).ToList();
        }
    }
}
