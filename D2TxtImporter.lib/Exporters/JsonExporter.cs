using D2TxtImporter.lib.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace D2TxtImporter.lib
{
    public class JsonExporter
    {
        public static void ExportJson(string outputPath, List<Unique> uniques, List<Runeword> runewords, List<CubeRecipe> cubeRecipes)
        {
            if (!Directory.Exists(outputPath))
            {
                throw new System.Exception("Could not find output directory");
            }

            var txtOutputDirectory = outputPath + "/json";

            if (!Directory.Exists(txtOutputDirectory))
            {
                Directory.CreateDirectory(txtOutputDirectory);
            }

            Uniques(txtOutputDirectory + "/uniques.json", uniques);
            Runewords(txtOutputDirectory + "/runewords.json", runewords);
            CubeRecipes(txtOutputDirectory + "/cube_recipes.json", cubeRecipes);
        }

        private static void Uniques(string destination, List<Unique> uniques)
        {
            var json = JsonConvert.SerializeObject(uniques);
            File.WriteAllText(destination, json);
        }

        private static void Runewords(string destination, List<Runeword> runewords)
        {
            var json = JsonConvert.SerializeObject(runewords);
            File.WriteAllText(destination, json);
        }

        private static void CubeRecipes(string destination, List<CubeRecipe> cubeRecipes)
        {
            var json = JsonConvert.SerializeObject(cubeRecipes);
            File.WriteAllText(destination, json);
        }
    }
}
