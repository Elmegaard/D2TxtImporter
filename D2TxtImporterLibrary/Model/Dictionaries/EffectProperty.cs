using System.Collections.Generic;

namespace D2TxtImporterLibrary.Model
{
    public class EffectProperty
    {
        public string Code { get; set; }
        public string Stat { get; set; }

        public static Dictionary<string, EffectProperty> EffectProperties;

        public static void Import(string excelFolder)
        {
            EffectProperties = new Dictionary<string, EffectProperty>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Properties.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');

                var effect = new EffectProperty
                {
                    Code = values[0],
                    Stat = values[5]
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
