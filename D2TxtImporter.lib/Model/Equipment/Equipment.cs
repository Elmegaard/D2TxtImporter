using System.Collections.Generic;

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
        public int ItemLevel { get; set; }
        public ItemType Type { get; set; }
        public string RequiredClass => string.IsNullOrEmpty(Type.Equiv2) ? "" : ItemType.ItemTypes[Type.Equiv2].Name.Replace(" Item", "");
    }

    public enum EquipmentType
    {
        Armor,
        Weapon,
        Jewelery
    }
}
