using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter_lib.Model
{
    public class ItemType
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Class { get; set; }

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
