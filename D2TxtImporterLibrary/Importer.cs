using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace D2TxtImporterLibrary
{
    public class Importer
    {
        private string _outputPath;
        private string _excelPath;
        private string _tablePath;

        public Importer(string excelPath, string tablePath, string outputDir)
        {
            
            if (!Directory.Exists(outputDir))
            {
                throw new Exception($"Could not find output directory at '{outputDir}'");
            }

            if (!Directory.Exists(excelPath))
            {
                throw new Exception($"Could not find excel directory at '{_excelPath}'");
            }

            if (!Directory.Exists(tablePath))
            {
                throw new Exception($"Could not find table directory at '{_tablePath}'");
            }

            _outputPath = outputDir.Trim('/', '\\');
            _excelPath = excelPath.Trim('/', '\\');
            _tablePath = tablePath.Trim('/', '\\');
           
            Model.Table.ImportFromTbl(_tablePath);
            Model.ItemStatCost.Import(_excelPath);
            Model.EffectProperty.Import(_excelPath);
            Model.ItemType.Import(_excelPath);
            Model.Armor.Import(_excelPath);
            Model.Weapon.Import(_excelPath);
            Model.Skill.Import(_excelPath);
            Model.CharStat.Import(_excelPath);
            Model.MonStat.Import(_excelPath);
            Model.Misc.Import(_excelPath);

            var uniques = Model.Unique.Import(_excelPath);
            var runewords = Model.Runeword.Import(_excelPath);
            var cubeRecipes = Model.CubeRecipe.Import(_excelPath);

            TxtExporter.Uniques(_outputPath + "/uniques.txt", uniques);
            TxtExporter.Runewords(_outputPath + "/runewords.txt", runewords);
            TxtExporter.CubeRecipes(_outputPath + "/cube_recipes.txt", cubeRecipes);
        }

        public static List<string> ReadCsvFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).ToList();
        }
    }
}
