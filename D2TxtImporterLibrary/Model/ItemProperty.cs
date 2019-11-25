namespace D2TxtImporterLibrary.Model
{
    public class ItemProperty
    {
        public EffectProperty Property { get; set; }
        public string Parameter { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }

        public ItemProperty(string property, string parameter, int? min, int? max)
        {
            Property = EffectProperty.EffectProperties[property];
            Parameter = parameter;
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return Property.ToString();
        }
    }
}
