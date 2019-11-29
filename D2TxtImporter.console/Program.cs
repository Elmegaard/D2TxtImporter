using System;

namespace D2TxtImporter_console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var excelPath = args[0];
                var tablePath = args[1];
                var outputDir = args[2];

                var importer = new D2TxtImporter_lib.Importer(excelPath, tablePath, outputDir);

                importer.LoadData();
                importer.ImportModel();
                importer.ExportTxt();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
