using System.Collections.Generic;
using System.Linq;

namespace D2TxtImporter.lib.Model
{
    public class ItemProperty
    {
        public EffectProperty Property { get; set; }
        public string Parameter { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public ItemStatCost ItemStatCost { get; set; }
        public string PropertyString { get; set; }

        public ItemProperty(string property, string parameter, int? min, int? max)
        {
            Property = EffectProperty.EffectProperties[property];
            Parameter = parameter;
            Min = min;
            Max = max;

            var stat = Property.Stat;


            if (string.IsNullOrEmpty(stat))
            {
                // Fix wrongly spelled and hardcoded stats
                switch (Property.Code)
                {
                    case "str":
                        stat = "strength";
                        break;
                    case "dmg-min":
                        stat = "mindamage";
                        break;
                    case "dmg-max":
                        stat = "maxdamage";
                        break;
                    case "indestruct":
                        stat = "item_indesctructible";
                        break;
                    case "poisonlength":
                        Min = null;
                        Max = null;
                        break;
                    default:
                        stat = Property.Code;
                        break;
                }
            }

            switch (stat)
            {
                case "item_addskill_tab": // Fetch skill tab from .lst files
                    break;
            }

            // Fix all class skills displaying as amazon
            if (stat == "item_addclassskills")
            {
                Parameter = Property.Code;
            }

            ItemStatCost = ItemStatCost.ItemStatCosts[stat];
            PropertyString = ItemStatCost.PropertyString(Min, Max, Parameter);
        }

        public static List<ItemProperty> GetProperties(string[] properties)
        {
            var result = new List<ItemProperty>();

            for (int i = 0; i < properties.Length; i += 4)
            {
                if (!string.IsNullOrEmpty(properties[i]) && !properties[i].StartsWith("*"))
                {
                    var prop = new ItemProperty(properties[i], properties[i + 1], Utility.ToNullableInt(properties[i + 2]), Utility.ToNullableInt(properties[i + 3]));
                    result.Add(prop);
                }
            }

            // Cleanup elemental damage as they are added as 2-3 different parameters
            var minDamage = result.Where(x => x.Property.Stat.Contains("mindam"));
            var maxDamage = result.Where(x => x.Property.Stat.Contains("maxdam"));
            var lenDamage = result.Where(x => x.Property.Stat.Contains("length"));

            if (minDamage.Count() > 0 && maxDamage.Count() > 0)
            {
                var props = new List<ItemProperty>();
                var toRemove = new List<ItemProperty>();
                var toAdd = new List<ItemProperty>();

                foreach (var minDam in minDamage)
                {
                    foreach (var maxDam in maxDamage)
                    {
                        var minDamProperty = minDam.Property.Stat.Replace("mindam", "");
                        var maxDamProperty = maxDam.Property.Stat.Replace("maxdam", "");

                        if (minDamProperty == maxDamProperty)
                        {
                            var damagePropertyName = minDamProperty;
                            if (damagePropertyName == "light")
                            {
                                damagePropertyName = "lightning";
                            }

                            damagePropertyName = damagePropertyName.First().ToString().ToUpper() + damagePropertyName.Substring(1);

                            var newProp = new ItemProperty("eledam", damagePropertyName, minDam.Min, maxDam.Max);

                            toRemove.Add(minDam);
                            toRemove.Add(maxDam);

                            foreach (var lenDam in lenDamage)
                            {
                                var lenDamProperty = lenDam.Property.Stat.Replace("length", "");
                                if (minDamProperty == lenDamProperty)
                                {
                                    toRemove.Add(lenDam);
                                }
                            }

                            toAdd.Add(newProp);
                        }
                    }
                }

                toRemove.ForEach(x => result.Remove(x));
                result.AddRange(toAdd);
            }

            // Min damage sometimes contain both elements, weird.
            if (minDamage.Count() > 0 && maxDamage.Count() == 0)
            {
                foreach (var minDam in minDamage)
                {
                    if (minDam.Min != minDam.Max)
                    {
                        minDam.PropertyString = minDam.PropertyString.Replace("+", "Adds ").Replace("Minimum ", "");
                    }
                }
            }

            result = result.OrderByDescending(x => x.ItemStatCost == null ? 0 : x.ItemStatCost.DescriptionPriority).ToList();

            return result;
        }

        public override string ToString()
        {
            return Property.ToString();
        }
    }
}
