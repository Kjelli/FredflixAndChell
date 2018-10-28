using FredflixAndChell.Shared.Components.Guns;
using FredflixAndChell.Shared.GameObjects.Bullets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class GunParameters
    {
        public GunSprite Sprite { get; set; }
        public BulletParameters BulletParameters { get; set; }
        public float FireRate { get; set; }
        public float RenderOffset { get; set; }
        public Vector2 BarrelOffset { get; set; }
        public int Ammo { get; set; }
        public int MaxAmmo { get; set; }
        public int MagazineSize { get; set; }
        public int MagazineAmmo { get; set; }
        public float ReloadTime { get; set; }
    }

    public static class GunPresets
    {
        public static readonly GunParameters M4 = new GunParameters
        {
            Sprite = GunSprite.M4,
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
            Sprite = GunSprite.Fido,
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
