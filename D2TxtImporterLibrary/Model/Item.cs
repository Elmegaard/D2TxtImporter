using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporterLibrary.Model
{
    public class Item
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int ItemLevel { get; set; }
        public int RequiredLevel { get; set; }
        public string Code { get; set; }
        public List<ItemProperty> Properties { get; set; }

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
