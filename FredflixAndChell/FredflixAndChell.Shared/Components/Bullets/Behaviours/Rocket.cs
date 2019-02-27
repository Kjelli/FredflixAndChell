using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Effects;
using FredflixAndChell.Shared.GameObjects.Players;
using Microsoft.Xna.Framework;
using Nez;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class Rocket : BulletBehaviour
    {
        public const float MaxRocketSpeed = 200f;

        public Rocket(Bullet bullet) : base(bullet)
        {

        }

        public override void update()
        {
            base.update();
            if (_bullet.Velocity.Length() < MaxRocketSpeed)
            {
                _bullet.Velocity *= 1.025f;
            }

            if (Time.checkEvery(0.08f))
            {
                entity.scene.addEntity(new Smoke(_bullet.position
                    - new Vector2(Mathf.cos(_bullet.Direction) * 4, Mathf.sin(_bullet.Direction) * 4)));
            }
        }

        public override void OnFired()
        {
            _bullet.Owner.Velocity += -_bullet.Owner.FacingAngle * 5f;
            for (var i = 0; i < 20; i++)
            {
                var randomOffsetX = Random.range(-2f, 2f);
                var randomOffsetY = Random.range(-2f, 2f);
                var position = _bullet.position + new Vector2(randomOffsetX, randomOffsetY);
                entity.scene.addEntity(new Smoke(position, false ));
            }
        }

        public override void OnImpact(Player player)
        {
            OnAnyImpact();
        }

        public override void OnNonPlayerImpact(CollisionResult collision)
        {
            OnAnyImpact();
        }

        private void OnAnyImpact()
        {
            entity.scene.addEntity(new Explosion(_bullet.position, _bullet.Owner));
            _bullet.destroy();
        }

    }
}
