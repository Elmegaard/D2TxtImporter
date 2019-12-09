using System;
using System.Collections.Generic;
using System.Data;
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
        public List<Model.Set> Sets { get; set; }
        public static bool ContinueOnException { get; set; }

        private readonly static string _exceptionFile = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}/errorlog.txt";
        private readonly static string _debugFile = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}/debuglog.txt";

        private static List<Exception> _exceptionsWritten { get; set; }

        public Importer(string excelPath, string tablePath, string outputDir)
        {
            _exceptionsWritten = new List<Exception>();

            // Empty output files
            if (File.Exists(_exceptionFile))
            {
                File.Delete(_exceptionFile);
            }
            if (File.Exists(_debugFile))
            {
                File.Delete(_debugFile);
            }

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

        private static void WriteException(Exception e)
        {
            if (_exceptionsWritten.Contains(e))
            {
                if (!ContinueOnException)
                {
                    throw e;
                }

                return;
            }
            _exceptionsWritten.Add(e);

            var ex = e;

            var errorMessage = "";
            var debugMessage = "";
            do
            {
                errorMessage += $"Message:\n{ex.Message}\n\nStacktrace:\n{ex.StackTrace}\n\n";
                debugMessage += $"Message:\n{ex.Message}\n\n";
                ex = ex.InnerException;
            }
            while (ex != null);

            File.AppendAllText(_exceptionFile, errorMessage + "\n\n");
            File.AppendAllText(_debugFile, debugMessage + "\n\n");

            if (!ContinueOnException)
            {
                throw e;
            }
        }

        public static void LogException(Exception e)
        {
            WriteException(e);

            if (!ContinueOnException)
            {
                throw e;
            }
        }

        public void LoadData()
        {
            try
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
                Model.Gem.Import(_excelPath);
                Model.SetItem.Import(_excelPath);
            }
            catch (Exception e)
            {
                WriteException(e);
            }
        }

        public void ImportModel()
        {
            try
            {
                Uniques = Model.Unique.Import(_excelPath);
                Runewords = Model.Runeword.Import(_excelPath);
                CubeRecipes = Model.CubeRecipe.Import(_excelPath);
                Sets = Model.Set.Import(_excelPath);
            }
            catch (Exception e)
            {
                WriteException(e);
            }
        }

        public void Export()
        {
            try
            {
                //TxtExporter.ExportTxt(_outputPath, Uniques, Runewords, CubeRecipes, Sets); // Out of date
                JsonExporter.ExportJson(_outputPath, Uniques, Runewords, CubeRecipes, Sets);
                WebExporter.ExportWeb(_outputPath);
            }
            catch (Exception e)
            {
                WriteException(e);
            }
        }

        public static List<string> ReadTxtFileToList(string path)
        {
            return File.ReadAllLines(path).ToList();
        }

        public static List<Dictionary<string, string>> ReadTxtFileToDictionaryList(string path)
        {
            var table = new List<Dictionary<string, string>>();

            var fileArray = File.ReadAllLines(path);
            var headerArray = fileArray.Take(1).First().Split('\t');

            var header = new List<string>();

            foreach (var column in headerArray)
            {
                header.Add(column);
            }

            var dataArray = fileArray.Skip(1);
            foreach (var valueLine in dataArray)
            {
                var values = valueLine.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var row = new Dictionary<string, string>();

                for (var i = 0; i < values.Length; i++)
                {
                    row[headerArray[i]] = values[i];
                }

                table.Add(row);
            }

            return table;
        }
    }
}
