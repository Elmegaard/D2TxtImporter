using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class Item
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int ItemLevel { get; set; }
        public int RequiredLevel { get; set; }
        public string Code { get; set; }
        public List<ItemProperty> Properties { get; set; }
        public bool DamageArmorEnhanced { get; set; }
        public Equipment Equipment { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
