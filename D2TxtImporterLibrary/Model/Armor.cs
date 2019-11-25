using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    class Armor : Equipment
    {
        public int MinAc { get; set; }
        public int MaxAc { get; set; }
        public string Type { get; set; }

        public static Dictionary<string, Armor> Armors;

        public static void Import(string excelFolder)
        {
            Armors = new Dictionary<string, Armor>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Armor.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var armor = new Armor
                {
                    Name = values[0],
                    Code = values[17],
                    MinAc = int.Parse(values[5]),
                    MaxAc = int.Parse(values[6]),
                    RequiredStrength = int.Parse(values[9]),
                    Type = values[48],
                    EquipmentType = EquipmentType.Armor
                };

                Armors[armor.Code] = armor;
            }
        }

        public override string ToString()
        {
            return Name;
        }


    }
}
