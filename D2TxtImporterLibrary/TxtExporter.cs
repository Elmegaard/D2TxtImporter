using D2TxtImporterLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary
{
    public class TxtExporter
    {
        public static void Uniques(string destination, List<Unique> uniques)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var unique in uniques)
            {
                sb.AppendLine(unique.Name);
                sb.AppendLine(unique.Equipment.Name);

                switch (unique.Equipment.EquipmentType)
                {
                    case EquipmentType.Weapon:
                        foreach (var wep in ((Weapon)unique.Equipment).DamageTypes)
                        {
                            switch (wep.Type)
                            {
                                case DamageTypeEnum.Normal:
                                    sb.AppendLine("Damage: " + wep.MinDamage + " to " + wep.MaxDamage);
                                    break;
                                case DamageTypeEnum.OneHanded:
                                    sb.AppendLine("One-Hand Damage: " + wep.MinDamage + " to " + wep.MaxDamage);
                                    break;
                                case DamageTypeEnum.TwoHanded:
                                    sb.AppendLine("Two-Hand Damage: " + wep.MinDamage + " to " + wep.MaxDamage);
                                    break;
                                case DamageTypeEnum.Thrown:
                                    sb.AppendLine("Thrown Damage: " + wep.MinDamage + " to " + wep.MaxDamage);
                                    break;
                            }
                        }
                        break;
                    case EquipmentType.Armor:
                        var armor = (Armor)unique.Equipment;

                        sb.AppendLine("Defense: " + armor.MinAc + "-" + armor.MaxAc);

                        break;
                    case EquipmentType.Jewelery:
                        break;
                }

                sb.AppendLine("Required Level: " + unique.RequiredLevel);

                if (unique.Equipment.RequiredStrength > 0)
                {
                    sb.AppendLine("Required Strength: " + unique.Equipment.RequiredStrength);
                }

                if (unique.Equipment.RequiredDexterity > 0)
                {
                    sb.AppendLine("Required Dexterity: " + unique.Equipment.RequiredDexterity);
                }

                foreach (var prop in unique.Properties)
                {
                    sb.AppendLine(prop.Property.ItemString + ": " + prop.Min + " - " + prop.Max);
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            File.WriteAllText(destination, sb.ToString());
        }

        public static void Runewords(string destination, List<Runeword> runewords)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var runeword in runewords)
            {
                sb.AppendLine(runeword.Name);

                var runes = "";
                foreach (var rune in runeword.Runes)
                {
                    runes += rune.Name + " ";
                }
                runes = runes.Trim(' ');
                sb.AppendLine(runes);

                var types = "";
                foreach (var type in runeword.Types)
                {
                    types += type.Name + "/";
                }
                types = types.Trim('/');

                sb.AppendLine(types);

                sb.AppendLine("Required Level: " + runeword.RequiredLevel);

                foreach (var prop in runeword.Properties)
                {
                    sb.AppendLine(prop.Property.ItemString + ": " + prop.Min + " - " + prop.Max);
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            File.WriteAllText(destination, sb.ToString());
        }

        public static void CubeRecipes(string destination, List<CubeRecipe> cubeRecipes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var recipe in cubeRecipes)
            {
                sb.AppendLine(recipe.Description);
            }

            File.WriteAllText(destination, sb.ToString());
        }
    }
}
