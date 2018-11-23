using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;
using Nez;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class MultiplyingBullet : BulletBehaviour
    {
        private const float MultiplyTime = 0.5f;
        private float _multiplyTimerSeconds = 0f;
        public MultiplyingBullet(Bullet bullet) : base(bullet) {}

        public override void update()
        {
            base.update();
            _multiplyTimerSeconds += Time.deltaTime;
            if(_multiplyTimerSeconds >= MultiplyTime)
            {
                Multiply();
            }
        }

        private void Multiply()
        {
            if (!entity.enabled) return;
            var direction = (float)Math.Atan2(_bullet.Velocity.Y, _bullet.Velocity.X);
            Bullet.Create(_bullet.Owner, entity.position.X, entity.position.Y, direction + 0.1f, _bullet.Parameters);
            Bullet.Create(_bullet.Owner, entity.position.X, entity.position.Y, direction - 0.1f, _bullet.Parameters);
            entity.destroy();
        }
    }
}
