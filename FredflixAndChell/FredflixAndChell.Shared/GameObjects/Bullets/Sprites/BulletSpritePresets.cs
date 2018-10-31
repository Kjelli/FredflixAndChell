using FredflixAndChell.Shared.Utilities.Graphics.Animations;

namespace FredflixAndChell.Shared.GameObjects.Bullets.Sprites
{
    public static class BulletSpritePresets
    {
        public static readonly BulletSprite Standard = new BulletSprite()
        {
            Source = "bullets/standard",
            Bullet = new SpriteAnimationDescriptor()
            {
                Frames = new int[] { 0 },
                FPS = 1,
                Loop = false,
            }
        };

        public static readonly BulletSprite Fido = new BulletSprite()
        {
            Source = "bullets/fido",
            Bullet = new SpriteAnimationDescriptor()
            {
                Frames = new int[] { 0 },
                FPS = 1,
                Loop = false,
            }
        };

        public static readonly BulletSprite Shockwave = new BulletSprite()
        {
            Source = "bullets/shockwave_sm",
            Bullet = new SpriteAnimationDescriptor()
            {
                Frames = new int[] { 0 },
                FPS = 1,
                Loop = false,
            }
        };
    }
}
