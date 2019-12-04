using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class MonStat
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonIgnore]
        public string Hcldx { get; set; }
        [JsonIgnore]
        public string NameStr { get; set; }

        [JsonIgnore]
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
