using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{
    public class BulletParameters
    {

        public BulletSprite Sprite { get; set; }
        public float Scale { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float LifeSpanSeconds { get; set; }
        public bool RotateWithGun { get; set; }

        // Construction limited to namespace
        internal BulletParameters() { }
    }
}