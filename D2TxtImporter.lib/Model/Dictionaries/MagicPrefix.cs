using D2TxtImporter.lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class MagicPrefix
    {
        public string Name { get; set; }

        [JsonIgnore]
        public int Index { get; set; }

        [JsonIgnore]
        public static Dictionary<int, MagicPrefix> MagicPrefixes;

        public static void Import(string excelFolder)
        {
            MagicPrefixes = new Dictionary<int, MagicPrefix>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/MagicPrefix.txt");

            var index = 0;
            foreach (var row in table)
            {
                index++;
                var magicPrefix = new MagicPrefix
                {
                    Name = row["Name"],
                    Index = index - 1
                };

                MagicPrefixes[index -1] = magicPrefix;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
