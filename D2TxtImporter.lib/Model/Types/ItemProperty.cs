﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D2TxtImporter.lib.Model
{
    public class ItemProperty
    {
        [JsonIgnore]
        public EffectProperty Property { get; set; }
        [JsonIgnore]
        public string Parameter { get; set; }
        [JsonIgnore]
        public int? Min { get; set; }
        [JsonIgnore]
        public int? Max { get; set; }
        [JsonIgnore]
        public ItemStatCost ItemStatCost { get; set; }
        public string PropertyString { get; set; }
        public int Index { get; set; }

        public ItemProperty(string property, string parameter, int? min, int? max, int index, int itemLevel = 0)
        {
            if (!EffectProperty.EffectProperties.ContainsKey(property.ToLower()))
            {
                throw new Exception($"Could not find property '{property.ToLower()}' parameter '{parameter}' min '{min}' max '{max}' index '{index}' itemlvl '{itemLevel}' in Properties.txt");
            }

            Property = EffectProperty.EffectProperties[property.ToLower()];
            Parameter = parameter;
            Min = min;
            Max = max;
            Index = index;

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

            if (!ItemStatCost.ItemStatCosts.ContainsKey(stat))
            {
                throw new System.Exception($"Could not find stat '{stat}' in ItemStatCost.txt");
            }

            ItemStatCost = ItemStatCost.ItemStatCosts[stat];
            try
            {
                PropertyString = ItemStatCost.PropertyString(Min, Max, Parameter, itemLevel);
            }
            catch (Exception e)
            {
                throw new Exception($"Could not generate properties for property '{property}' with parameter '{parameter}' min '{min}' max '{max}' index '{index}' itemlvl '{itemLevel}'", e);
            }
        }

        public static List<ItemProperty> GetProperties(List<PropertyInfo> properties, int itemLevel = 0)
        {
            var result = new List<ItemProperty>();

            foreach (var property in properties)
            {
                if (!string.IsNullOrEmpty(property.Property) && !property.Property.StartsWith("*"))
                {
                    var prop = new ItemProperty(property.Property, property.Parameter, property.Min, property.Max, properties.IndexOf(property), itemLevel);
                    if (prop.Property.Code.ToLower() != "state") // Don't add states (auras)
                    {
                        result.Add(prop);
                    }
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

                            var newProp = new ItemProperty("eledam", damagePropertyName, minDam.Min, maxDam.Max, minDam.Index);

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

            // Sometimes there is cold length with min/max damage
            lenDamage = result.Where(x => x.Property.Stat.Contains("length"));
            if (lenDamage.Count() > 0)
            {
                result.RemoveAll(x => lenDamage.Any(y => y == x && y.Property.Code == "cold-len"));
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

            return result;
        }

        public override string ToString()
        {
            return Property.ToString();
        }
    }
}
