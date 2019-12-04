using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class ItemType
    {
        public string Name { get; set; }

        [JsonIgnore]
        public string Code { get; set; }
        public string Class { get; set; }

        [JsonIgnore]
        public string Equiv2 { get; set; }

        [JsonIgnore]
        public string Equiv1 { get; set; }

        [JsonIgnore]
        public static Dictionary<string, ItemType> ItemTypes;

        public static void Import(string excelFolder)
        {
            ItemTypes = new Dictionary<string, ItemType>();

            var lines = Importer.ReadCsvFile(excelFolder + "/ItemTypes.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var itemType = new ItemType
                {
                    Name = values[0],
                    Code = values[1],
                    Equiv1 = values[2],
                    Equiv2 = values[3],
                    Class = values[27]
                };

                ItemTypes[itemType.Code] = itemType;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
