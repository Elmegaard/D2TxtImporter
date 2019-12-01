using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace D2TxtImporter.lib
{
    public class Importer
    {
        private string _outputPath;
        private string _excelPath;
        private string _tablePath;

        public List<Model.Unique> Uniques { get; set; }
        public List<Model.Runeword> Runewords { get; set; }
        public List<Model.CubeRecipe> CubeRecipes { get; set; }

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
        }

        public void LoadData()
        {
            Model.Table.ImportFromTbl(_tablePath);
            Model.MagicPrefix.Import(_excelPath);
            Model.MagicSuffix.Import(_excelPath);
            Model.ItemStatCost.Import(_excelPath);
            Model.EffectProperty.Import(_excelPath);
            Model.ItemType.Import(_excelPath);
            Model.Armor.Import(_excelPath);
            Model.Weapon.Import(_excelPath);
            Model.Skill.Import(_excelPath);
            Model.CharStat.Import(_excelPath);
            Model.MonStat.Import(_excelPath);
            Model.Misc.Import(_excelPath);
        }

        public void ImportModel()
        {
            Uniques = Model.Unique.Import(_excelPath);
            Runewords = Model.Runeword.Import(_excelPath);
            CubeRecipes = Model.CubeRecipe.Import(_excelPath);
        }

        public void Export()
        {
            TxtExporter.ExportTxt(_outputPath, Uniques, Runewords, CubeRecipes);
            JsonExporter.ExportJson(_outputPath, Uniques, Runewords, CubeRecipes);
            WebExporter.ExportWeb(_outputPath);
        }

        public static List<string> ReadCsvFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).ToList();
        }
    }
}
