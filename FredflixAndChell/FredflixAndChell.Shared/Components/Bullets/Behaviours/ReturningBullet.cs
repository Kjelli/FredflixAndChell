using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;
using Microsoft.Xna.Framework;
using Nez;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class ReturningBullet : BulletBehaviour
    {
        private const float _returnSpeed = 200f;
        private const float _accelerationFactor = 0.25f;

        private Player _owner;

        private float _acceleration = 1.0f;

        public ReturningBullet(Bullet bullet) : base(bullet) {
            _owner = bullet.Owner;
        }

        public override void OnImpact(Player player)
        {
            if (player != _owner)
            {
                DamagePlayer(player);
            }
            else
            {
                var damage = _bullet.ToDirectionalDamage();
                damage.CanHitSelf = true;
                damage.Damage /= 4;
                player.Damage(damage);
                _bullet.destroy();
            }
        }

        public override void OnNonPlayerImpact(CollisionResult collision)
        {
            Bounce(collision);
        }

        public override void update()
        {
            base.update();
            if (_owner.PlayerState == PlayerState.Dead) return;
            var directionToPlayer = Vector2.Normalize(_owner.position - _bullet.position);
            _bullet.Velocity = _bullet.Velocity * _acceleration + directionToPlayer * _returnSpeed * (1 - _acceleration);
            _acceleration = (float)Math.Max(_acceleration - _accelerationFactor * Time.deltaTime, 0);
        }
    }
}
