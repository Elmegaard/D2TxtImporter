using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class CubeRecipe
    {
        public string Description { get; set; }

        public static List<CubeRecipe> Import(string excelFolder)
        {
            var result = new List<CubeRecipe>();

            var lines = Importer.ReadCsvFile(excelFolder + "/CubeMain.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]) || values[1] == "0")
                {
                    continue;
                }

                var descr = values[0].Replace("rune ", "r");
                var matches = Regex.Matches(descr, @"(r\d\d)");
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var rune = Misc.MiscItems[match.Groups[1].Value];

                        descr = descr.Replace(rune.Code, rune.Name);
                    }
                }

                var cupeRecipe = new CubeRecipe
                {
                    Description = descr
                };

                result.Add(cupeRecipe);
            }

            return result;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
