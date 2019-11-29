using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter_lib.Model
{
    public class DamageType
    {
        public DamageTypeEnum Type { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public string DamageString { get; set; }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    public enum DamageTypeEnum
    {
        OneHanded,
        TwoHanded,
        Thrown,
        Normal
    }
}
