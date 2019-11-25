using System.Collections.Generic;

namespace D2TxtImporterLibrary.Model
{
    public class EffectProperty
    {
        public string Code { get; set; }
        public string ItemString => Code;

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
                    Code = values[0]
                };

                EffectProperties[effect.Code] = effect;
            }
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
