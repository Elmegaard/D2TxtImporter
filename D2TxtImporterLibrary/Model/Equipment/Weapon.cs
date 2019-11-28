using System.Collections.Generic;

namespace D2TxtImporterLibrary.Model
{
    public class Weapon : Equipment
    {
        public List<DamageType> DamageTypes { get; set; }

        public static Dictionary<string, Weapon> Weapons;


        public static void Import(string excelFolder)
        {
            Weapons = new Dictionary<string, Weapon>();

            var lines = Importer.ReadCsvFile(excelFolder + "/Weapons.txt");

            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var damageTypes = new List<DamageType>();

                var isOneOrTwoHanded = values[12] == "1";
                var isTwoHanded = values[13] == "1";
                var isThrown = !string.IsNullOrEmpty(values[16]);

                if (!isTwoHanded)
                {
                    damageTypes.Add(new DamageType { Type = DamageTypeEnum.Normal, MinDamage = int.Parse(values[10]), MaxDamage = int.Parse(values[11]) });
                }
                else if (isOneOrTwoHanded)
                {
                    damageTypes.Add(new DamageType { Type = DamageTypeEnum.OneHanded, MinDamage = int.Parse(values[10]), MaxDamage = int.Parse(values[11]) });
                }

                if (isTwoHanded)
                {
                    damageTypes.Add(new DamageType { Type = DamageTypeEnum.TwoHanded, MinDamage = int.Parse(values[14]), MaxDamage = int.Parse(values[15]) });
                }

                if (isThrown)
                {
                    damageTypes.Add(new DamageType { Type = DamageTypeEnum.Thrown, MinDamage = int.Parse(values[16]), MaxDamage = int.Parse(values[17]) });
                }

                var weapon = new Weapon
                {
                    Name = values[0],
                    DamageTypes = damageTypes,
                    Code = values[3],
                    EquipmentType = EquipmentType.Weapon,
                    RequiredStrength = !string.IsNullOrEmpty(values[23]) ? int.Parse(values[23]) : 0,
                    RequiredDexterity = !string.IsNullOrEmpty(values[24]) ? int.Parse(values[24]) : 0,
                    Durability = values[26] == "1" ? 0 : int.Parse(values[25])
                };

                Weapons[weapon.Code] = weapon;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
