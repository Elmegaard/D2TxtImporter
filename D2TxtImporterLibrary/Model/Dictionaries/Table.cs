using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class Table
    {
        public static Dictionary<string, string> Tables;

        public static void Import(string tableFolder)
        {
            Tables = new Dictionary<string, string>();

            var files = Directory.GetFiles(tableFolder, "*.txt");
            foreach (var file in files)
            {
                var lines = Importer.ReadCsvFile(file);

                foreach (var line in lines)
                {
                    var values = line.Split('\t');

                    var key = values[0].Trim('"');
                    var value = values[1].Trim('"');

                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    Tables[key] = value;
                }
            }
        }

        public static string GetValue(string key)
        {
            if (Tables.ContainsKey(key))
            {
                return Tables[key];
            }
            return null;
        }
    }
}
