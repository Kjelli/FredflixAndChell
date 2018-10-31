using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{
    public class BulletParameters
    {

        public BulletSprite Sprite { get; set; }
        public float Scale { get; set; } = 0.25f;
        public float Speed { get; set; } = 50f;
        public float Damage { get; set; } = 10f;
        public float Knockback { get; set; } = 3.0f;
        public float LifeSpanSeconds { get; set; } = -1f;
        public bool RotateWithGun { get; set; } = true;
        public bool Bounce { get; set; } = false;
    }
}