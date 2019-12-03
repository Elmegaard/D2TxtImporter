using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib
{
    public class WebExporter
    {
        public static void ExportWeb(string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                throw new Exception("Could not find output directory");
            }

            var exePath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}";
            var webPath = $"{exePath}/Exporters/Web";
            var jsonPath = $"{outputPath}/json";

            if (!Directory.Exists(webPath))
            {
                throw new Exception($"Could not find Web Files directory in '{webPath}'");
            }

            if (!Directory.Exists(jsonPath))
            {
                throw new Exception($"Could not find Json directory in '{jsonPath}'");
            }

            var webOutputDirectory = outputPath + "/web";

            if (!Directory.Exists(webOutputDirectory))
            {
                Directory.CreateDirectory(webOutputDirectory);
            }

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(webPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(webPath, webOutputDirectory));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(webPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(webPath, webOutputDirectory), true);
            }

            var uniqueJson = File.ReadAllText($"{jsonPath}/uniques.json", Encoding.UTF8);
            var runewordJson = File.ReadAllText($"{jsonPath}/runewords.json", Encoding.UTF8);
            var cubeRecipeJson = File.ReadAllText($"{jsonPath}/cube_recipes.json", Encoding.UTF8);
            var setsJson = File.ReadAllText($"{jsonPath}/sets.json", Encoding.UTF8);

            var jsFile = $"{webOutputDirectory}/d2export.js";
            var js = File.ReadAllText(jsFile, Encoding.UTF8);

            js = js.Replace("\"<UNIQUES_JSON>\"", JsonConvert.ToString(uniqueJson)).Replace("\"<RUNEWORDS_JSON>\"", JsonConvert.ToString(runewordJson)).Replace("\"<CUBE_RECIPES_JSON>\"", JsonConvert.ToString(cubeRecipeJson)).Replace("\"<SETS_JSON>\"", JsonConvert.ToString(setsJson));
            File.WriteAllText(jsFile, js, Encoding.UTF8);
        }

        static void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        Console.WriteLine(f);
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}
