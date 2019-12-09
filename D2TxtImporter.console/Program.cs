using CommandLine;
using System;

namespace D2TxtImporter_console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parsedArgs = Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
                {
                    var importer = new D2TxtImporter.lib.Importer(o.ExcelPath, o.TablePath, o.OutputPath);

                    D2TxtImporter.lib.Model.CubeRecipe.UseDescription = o.CubeRecipeDescription;
                    D2TxtImporter.lib.Importer.ContinueOnException = o.ContinueOnException;

                    importer.LoadData();
                    importer.ImportModel();
                    importer.Export();
                });
            }
            catch (Exception e)
            {
                var errorMessage = "";
                do
                {
                    errorMessage += $"Message:\n{e.Message}\n\n";
                    e = e.InnerException;
                }
                while (e != null);

                Console.WriteLine(errorMessage);
                Console.ReadLine();
            }
        }
    }

    public class Options
    {
        [Option('e', "excelPath", Required = true, HelpText = "Path to the .txt files")]
        public string ExcelPath { get; set; }

        [Option('t', "tablePath", Required = true, HelpText = "Path to the .tbl files")]
        public string TablePath { get; set; }

        [Option('o', "outputPath", Required = true, HelpText = "Path the output is genereted at (must exist)")]
        public string OutputPath { get; set; }

        [Option("cubeRecipeDescription", Required = false, HelpText = "Use the description from CubeRecipes.txt instead of generating it")]
        public bool CubeRecipeDescription { get; set; }

        [Option("continueOnException", Required = false, HelpText = "If an exception occures, log it and continue. Check debuglog.txt and errorlog.txt for info")]
        public bool ContinueOnException { get; set; }
    }
}
