using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2TxtImporter.lib.Model
{
    public class PropertyInfo
    {
        public string Property { get; set; }
        public string Parameter { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }

        public PropertyInfo(string property, string parameter, int? min, int? max)
        {
            Property = property;
            Parameter = parameter;
            Min = min;
            Max = max;
        }

        public PropertyInfo(string property, string parameter, string min, string max)
        {
            Property = property;
            Parameter = parameter;
            Min = Utility.ToNullableInt(min);
            Max = Utility.ToNullableInt(max);
        }
    }
}
