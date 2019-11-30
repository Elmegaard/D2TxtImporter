namespace D2TxtImporter.lib.Model
{
    public class Equipment
    {
        public EquipmentType EquipmentType { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int RequiredStrength { get; set; }
        public int RequiredDexterity { get; set; }
        public int Durability { get; set; }
    }

    public enum EquipmentType
    {
        Armor,
        Weapon,
        Jewelery
    }
}
