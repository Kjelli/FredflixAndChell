using FredflixAndChell.Shared.GameObjects.Collectibles;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class WeaponParameters
    {
        public string Name { get; set; }

        public float FireRate { get; set; } = 0.1f;

        public float RenderOffset { get; set; }
        public bool RotatesWithPlayer { get; set; } = true;
        public bool AlwaysAbovePlayer { get; set; } = false;
        public bool FlipXWithPlayer { get; set; } = false;
        public bool FlipYWithPlayer { get; set; } = true;
        public float Scale { get; set; } = 0.6f;

        public float DropChance { get; set; }
        public Rarity Rarity { get; set; }
    }
}
