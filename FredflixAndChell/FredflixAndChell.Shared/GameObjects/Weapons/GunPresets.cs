using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;
using System;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public static class GunPresets
    {
        public static readonly GunParameters M4 = new GunParameters
        {
            Name = "M4",
            Sprite = GunSpritePresets.M4,
            BulletParameters = BulletPresets.Standard,
            FireRate = 0.05f,
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
            Name = "Fido",
            Sprite = GunSpritePresets.Fido,
            BulletParameters = BulletPresets.Bark,
            FireRate = 0.3f,
            BarrelOffset = new Vector2(7, 0),
            Ammo = 300,
            MaxAmmo = 300,
            MagazineAmmo = 30,
            MagazineSize = 30,
            ReloadTime = 0.8f,
            RenderOffset = 5,
            BulletCount = 5,
            BulletSpread = (float)Math.PI / 8
        };

        public static readonly GunParameters PewPew = new GunParameters
        {
            Name = "PewPew",
            Sprite = GunSpritePresets.PewPew,
            BulletParameters = BulletPresets.Standard,
            FireRate = 0.05f,
            BarrelOffset = new Vector2(10, 0),
            Ammo = 300,
            MaxAmmo = 300,
            MagazineAmmo = 30,
            MagazineSize = 30,
            ReloadTime = 2f,
            RenderOffset = 7f
        };
    }
}
