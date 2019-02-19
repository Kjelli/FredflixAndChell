using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public enum CollectibleType
    {
        Weapon = 1
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public class CollectibleParameters
    {
        public string Name { get; set; }
        internal CollectibleParameters() { }
        public CollectibleType Type { get; set; }
        public GunParameters Gun { get; set; }
        public float DropChance { get; set; }
        public Rarity Rarity { get; set; }
    }
}
