using FredflixAndChell.Shared.Components.Bullets;
using FredflixAndChell.Shared.Components.Bullets.Behaviours;
using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;
using System;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{
    public class BulletParameters
    {
        public BulletSprite Sprite { get; set; }
        public string BulletBehaviour { get; set; } = nameof(StandardBullet);
        public float Scale { get; set; } = 0.25f;
        public float Speed { get; set; } = 50f;
        public float Damage { get; set; } = 10f;
        public float Knockback { get; set; } = 3.0f;
        public float LifeSpanSeconds { get; set; } = -1f;
        public bool RotateWithGun { get; set; } = true;

    }
}