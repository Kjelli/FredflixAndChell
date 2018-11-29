using FredflixAndChell.Shared.Components.Guns;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class GunParameters
    {
        public string Name { get; set; }
        public GunSprite Sprite { get; set; }
        public BulletParameters BulletParameters { get; set; }

        public float FireRate { get; set; } = 0.1f;
        public float ReloadTime { get; set; } = 1f;

        public float RenderOffset { get; set; }
        public Vector2 BarrelOffset { get; set; }
        public bool RotatesWithPlayer { get; set; } = true;
        public bool AlwaysAbovePlayer { get; set; } = false;
        public bool FlipXWithPlayer { get; set; } = false;
        public bool FlipYWithPlayer { get; set; } = true;
        public float Scale { get; set; } = 0.6f;

        public int Ammo { get; set; }
        public int MaxAmmo { get; set; }
        public int MagazineSize { get; set; }
        public int MagazineAmmo { get; set; }

        public int BulletCount { get; set; } = 1;
        public float BulletSpread { get; set; } = 0;

        public float DropChance { get; set; }
        public Rarity Rarity { get; set; }
        public float Accuracy { get; set; } = 0.8f;
    }
}
