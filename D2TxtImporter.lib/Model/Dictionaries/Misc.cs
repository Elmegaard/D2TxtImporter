using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter_lib.Model
{
    public class Misc
    {
        public string Name { get; set; }
        public int ItemLevel { get; set; }
        public int RequiredLevel { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }

        public static Dictionary<string, Misc> MiscItems;

        public static void Import(string excelFolder)
        {
            MiscItems = new Dictionary<string, Misc>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Misc.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[5]))
                {
                    continue;
                }

                var misc = new Misc
                {
                    Name = values[1],
                    ItemLevel = int.Parse(values[5]),
                    RequiredLevel = int.Parse(values[6]),
                    Code = values[13],
                    Type = values[32]
                };

                MiscItems[misc.Code] = misc;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
