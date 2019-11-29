using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter_lib.Model
{
    public class MonStat
    {
        public string Id { get; set; }
        public string Hcldx { get; set; }
        public string NameStr { get; set; }

        public static Dictionary<string, MonStat> MonStats;

        public static void Import(string excelFolder)
        {
            MonStats = new Dictionary<string, MonStat>();

            var lines = Importer.ReadCsvFile(excelFolder + "/MonStats.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var monStat = new MonStat
                {
                    Id = values[0],
                    Hcldx = values[1],
                    NameStr = values[5]
                };

                MonStats[monStat.Hcldx] = monStat;
            }
        }

        public override string ToString()
        {
            return NameStr;
        }
    }
}
