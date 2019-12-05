using Newtonsoft.Json;
using System.Collections.Generic;

namespace D2TxtImporter.lib.Model
{
    public class EffectProperty
    {
        [JsonIgnore]
        public string Code { get; set; }

        [JsonIgnore]
        public string Stat { get; set; }

        [JsonIgnore]
        public static Dictionary<string, EffectProperty> EffectProperties;

        public static void Import(string excelFolder)
        {
            EffectProperties = new Dictionary<string, EffectProperty>();

            var table = Importer.ReadTxtFileToDictionaryList(excelFolder + "/Properties.txt");

            foreach (var row in table)
            {
                var effect = new EffectProperty
                {
                    Code = row["code"],
                    Stat = row["stat1"]
                };

                EffectProperties[effect.Code] = effect;
            }

            EffectProperties["eledam"] = new EffectProperty {Code = "eledam", Stat = "" };
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
