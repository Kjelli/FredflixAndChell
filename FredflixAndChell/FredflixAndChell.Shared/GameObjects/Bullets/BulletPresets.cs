using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{
    public static class BulletPresets
    {
        public static readonly BulletParameters Standard = new BulletParameters
        {
            Sprite = BulletSprite.Standard,
            Scale = 0.15f,
            Damage = 5f,
            Speed = 200f,
            LifeSpanSeconds = -1,
            RotateWithGun = false,
        };

        public static readonly BulletParameters Fido = new BulletParameters
        {
            Sprite = BulletSprite.Fido,
            Scale = 0.25f,
            Damage = 5f,
            Speed = 50f,
            LifeSpanSeconds = -1,
            RotateWithGun = true,
        };
    }
}
