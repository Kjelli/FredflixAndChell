using FredflixAndChell.Shared.GameObjects.Bullets;
using Nez;
using System;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class RotatingReturningBullet : ReturningBullet
    {
        public RotatingReturningBullet(Bullet bullet) : base(bullet)
        {
        }

        public override void update()
        {
            base.update();
            _bullet.rotation += 8 * (float)Math.PI * Time.deltaTime;
        }
    }
}
