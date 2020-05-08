using Newtonsoft.Json;
using System;

namespace D2TxtImporter.lib.Model
{
    public class Equipment : ICloneable
    {
        public EquipmentType EquipmentType { get; set; }
        public string Name { get { return Table.GetValue(Code); } }
        [JsonIgnore]
        public string Code { get; set; }
        public int RequiredStrength { get; set; }
        public int RequiredDexterity { get; set; }
        public int Durability { get; set; }
        public int ItemLevel { get; set; }
        public ItemType Type { get; set; }
        public string RequiredClass => string.IsNullOrEmpty(Type.Equiv2) ? "" : ItemType.ItemTypes[Type.Equiv2].Name.Replace(" Item", "");

        public object Clone()
        {
            return new Equipment
            {
                EquipmentType = this.EquipmentType,
                Code = this.Code,
                RequiredStrength = this.RequiredStrength,
                RequiredDexterity = this.RequiredDexterity,
                Durability = this.Durability,
                ItemLevel = this.ItemLevel,
                Type = this.Type
            };
        }
    }

    public enum EquipmentType
    {
        Armor,
        Weapon,
        Jewelery
    }
}
