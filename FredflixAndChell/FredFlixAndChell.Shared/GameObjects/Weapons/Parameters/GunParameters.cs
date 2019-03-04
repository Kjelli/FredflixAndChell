using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using FredflixAndChell.Shared.Weapons.Parameters;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Parameters
{
    public class GunParameters : WeaponParameters
    {
        public GunSprite Sprite { get; set; }
        public string Bullet { get; set; }
        public float ReloadTime { get; set; } = 1f;

        public Vector2 BarrelOffset { get; set; }

        public int Ammo { get; set; }
        public int MaxAmmo { get; set; }
        public int MagazineSize { get; set; }
        public int MagazineAmmo { get; set; }

        public int BulletCount { get; set; } = 1;
        public float BulletSpread { get; set; } = 0;

        public float Accuracy { get; set; } = 0.8f;
        public bool InheritsPlayerSpeed { get; set; } = false;
    }
}
