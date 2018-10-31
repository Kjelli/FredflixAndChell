using FredflixAndChell.Shared.Utilities.Graphics.Animations;

namespace FredflixAndChell.Shared.GameObjects.Bullets.Sprites
{
    public class BulletSprite
    {
        public enum BulletAnimations
        {
            Bullet
        }

        public string Source { get; set; }

        public SpriteAnimationDescriptor Bullet { get; set; }
    }
}
