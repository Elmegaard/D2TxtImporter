namespace D2TxtImporterLibrary.Model
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
                    default:
                        stat = Property.Code;
                        break;
                }
            }

            ItemStatCost = ItemStatCost.ItemStatCosts[stat];
            PropertyString = ItemStatCost.PropertyString(Min, Max, Parameter);
        }

        public override string ToString()
        {
            return Property.ToString();
        }
    }
}
