using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public static class GunPresets
    {
        public static readonly GunParameters M4 = new GunParameters
        {
            Sprite = GunSpritePresets.M4,
            BulletParameters = BulletPresets.Standard,
            FireRate = 0.2f,
            BarrelOffset = new Vector2(10, 0),
            Ammo = 300,
            MaxAmmo = 300,
            MagazineAmmo = 30,
            MagazineSize = 30,
            ReloadTime = 2f,
            RenderOffset = 7f,
        };

        public static readonly GunParameters Fido = new GunParameters
        {
            Sprite = GunSpritePresets.Fido,
            BulletParameters = BulletPresets.Fido,
            FireRate = 0.3f,
            BarrelOffset = new Vector2(7, 0),
            Ammo = 300,
            MaxAmmo = 300,
            MagazineAmmo = 30,
            MagazineSize = 30,
            ReloadTime = 0.8f,
            RenderOffset = 5,
        };
    }
}
