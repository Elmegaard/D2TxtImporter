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
            var importer = new D2TxtImporterLibrary.Importer(args[0]);
        }
    }
}
