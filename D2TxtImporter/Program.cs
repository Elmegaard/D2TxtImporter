using System;

namespace D2TxtImporter
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

                var importer = new D2TxtImporterLibrary.Importer(excelPath, tablePath, outputDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
