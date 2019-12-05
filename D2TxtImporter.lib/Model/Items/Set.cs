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

                // Add the properties
                var propArray = new string[] {
                    row["PCode2a"], row["PParam2a"], row["PMin2a"], row["PMax2a"], row["PCode2b"], row["PParam2b"], row["PMin2b"], row["PMax2b"],
                    row["PCode3a"], row["PParam3a"], row["PMin3a"], row["PMax3a"], row["PCode3b"], row["PParam3b"], row["PMin3b"], row["PMax3b"],
                    row["PCode4a"], row["PParam4a"], row["PMin4a"], row["PMax4a"], row["PCode4b"], row["PParam4b"], row["PMin4b"], row["PMax4b"],
                    row["PCode5a"], row["PParam5a"], row["PMin5a"], row["PMax5a"], row["PCode5b"], row["PParam5b"], row["PMin5b"], row["PMax5b"]
                };
                var properties = ItemProperty.GetProperties(propArray, set.Level).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                set.PartialProperties = properties.OrderBy(x => x.Index).ToList();

                propArray = new string[] {
                    row["FCode1"], row["FParam1"], row["FMin1"], row["FMax1"],
                    row["FCode2"], row["FParam2"], row["FMin2"], row["FMax2"],
                    row["FCode3"], row["FParam3"], row["FMin3"], row["FMax3"],
                    row["FCode4"], row["FParam4"], row["FMin4"], row["FMax4"],
                    row["FCode5"], row["FParam5"], row["FMin5"], row["FMax5"],
                    row["FCode6"], row["FParam6"], row["FMin6"], row["FMax6"],
                    row["FCode7"], row["FParam7"], row["FMin7"], row["FMax7"],
                    row["FCode8"], row["FParam8"], row["FMin8"], row["FMax8"]
                };

                var propertiesFull = ItemProperty.GetProperties(propArray, set.Level).OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();
                set.FullProperties = propertiesFull;

                set.SetItems = SetItem.SetItems.Where(x => x.Set == set.Index).ToList();

                result.Add(set);
            }

            return result.OrderBy(x => x.Level).ToList();
        }
    }
}
