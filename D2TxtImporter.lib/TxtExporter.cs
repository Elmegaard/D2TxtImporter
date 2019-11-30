using D2TxtImporter.lib.Model;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2TxtImporter.lib
{
    public class TxtExporter
    {
        public static void ExportTxt(string outputPath, List<Unique> uniques, List<Runeword> runewords, List<CubeRecipe> cubeRecipes)
        {
            if (!Directory.Exists(outputPath))
            {
                throw new System.Exception("Could not find output directory");
            }

            var txtOutputDirectory = outputPath + "/txt";

            if (!Directory.Exists(txtOutputDirectory))
            {
                Directory.CreateDirectory(txtOutputDirectory);
            }

            Uniques(txtOutputDirectory + "/uniques.txt", uniques);
            Runewords(txtOutputDirectory + "/runewords.txt", runewords);
            CubeRecipes(txtOutputDirectory + "/cube_recipes.txt", cubeRecipes);
        }

        private static void Uniques(string destination, List<Unique> uniques)
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
                                    sb.AppendLine("Damage: " + wep.DamageString);
                                    break;
                                case DamageTypeEnum.OneHanded:
                                    sb.AppendLine("One-Hand Damage: " + wep.DamageString);
                                    break;
                                case DamageTypeEnum.TwoHanded:
                                    sb.AppendLine("Two-Hand Damage: " + wep.DamageString);
                                    break;
                                case DamageTypeEnum.Thrown:
                                    sb.AppendLine("Thrown Damage: " + wep.DamageString);
                                    break;
                            }
                        }
                        break;
                    case EquipmentType.Armor:
                        var armor = (Armor)unique.Equipment;
                        sb.AppendLine("Defense: " + armor.ArmorString);
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

                if (unique.Equipment.Durability != 0)
                {
                    sb.AppendLine("Durability: " + unique.Equipment.Durability);
                }

                if (unique.Equipment.EquipmentType == EquipmentType.Armor)
                {
                    var armor = unique.Equipment as Armor;

                    if (!string.IsNullOrEmpty(armor.DamageString))
                    {
                        sb.AppendLine(armor.DamageStringPrefix + ": " + armor.DamageString);
                    }
                }

                foreach (var prop in unique.Properties)
                {
                    sb.AppendLine(prop.PropertyString);
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            File.WriteAllText(destination, sb.ToString());
        }

        private static void Runewords(string destination, List<Runeword> runewords)
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
                    sb.AppendLine(prop.PropertyString);
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            File.WriteAllText(destination, sb.ToString());
        }

        private static void CubeRecipes(string destination, List<CubeRecipe> cubeRecipes)
        {
            StringBuilder sb = new StringBuilder();


            foreach (var recipe in cubeRecipes)
            {
                sb.AppendLine(recipe.CubeRecipeDescription);
            }

            File.WriteAllText(destination, sb.ToString());
        }
    }
}
