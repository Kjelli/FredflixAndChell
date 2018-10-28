using FredflixAndChell.Shared.Components.Guns;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class GunParameters
    {
        // Construction limited to namespace
        internal GunParameters(){}

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
}
