using D2TxtImporter.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class MagicSuffix
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public static Dictionary<int, MagicSuffix> MagicSuffixes;

        public static void Import(string excelFolder)
        {
            MagicSuffixes = new Dictionary<int, MagicSuffix>();

            var lines = Importer.ReadCsvFile(excelFolder + "/MagicSuffix.txt");

            var index = 0;
            foreach (var line in lines)
            {
                index++;
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[0]))
                {
                    continue;
                }

                var magicSuffix = new MagicSuffix
                {
                    Name = values[0],
                    Index = index - 1
                };

                MagicSuffixes[index - 1] = magicSuffix;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
