using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{
    public static class BulletPresets
    {
        public static readonly BulletParameters Standard = new BulletParameters
        {
            Sprite = BulletSpritePresets.Standard,
            Scale = 0.15f,
            Damage = 5f,
            Speed = 200f,
            LifeSpanSeconds = -1,
            RotateWithGun = false,
        };

        public static readonly BulletParameters Fido = new BulletParameters
        {
            Sprite = BulletSpritePresets.Fido,
            Scale = 0.25f,
            Damage = 5f,
            Speed = 50f,
            LifeSpanSeconds = -1,
            RotateWithGun = true,
            Bounce = true,
        };

        public static readonly BulletParameters Bark = new BulletParameters
        {
            Sprite = BulletSpritePresets.Shockwave,
            Scale = 0.25f,
            Damage = 0f,
            Speed = 60f,
            LifeSpanSeconds = 0.5f,
            RotateWithGun = true,
        };
    }
}
