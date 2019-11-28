using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dataDir = args[0];
                var outputDir = args[1];

                var importer = new D2TxtImporterLibrary.Importer(dataDir, outputDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
