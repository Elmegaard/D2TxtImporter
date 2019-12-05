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

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/ItemTypes.txt");

            foreach (var row in table)
            {
                var itemType = new ItemType
                {
                    Name = row["ItemType"],
                    Code = row["Code"],
                    Equiv1 = row["Equiv1"],
                    Equiv2 = row["Equiv2"],
                    Class = row["Class"]
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
