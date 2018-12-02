using FredflixAndChell.Shared.GameObjects.Bullets;
using Nez;
using System;
using FredflixAndChell.Shared.GameObjects.Players;
using static FredflixAndChell.Shared.Assets.Constants;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public abstract class BulletBehaviour : Component, IUpdatable
    {
        protected readonly Bullet _bullet;
        protected CircleCollider _collider;
        protected ProjectileMover _mover;

        public BulletBehaviour(Bullet bullet)
        {
            _bullet = bullet;
        }

        public override void onAddedToEntity()
        {
            _collider = entity.addComponent<CircleCollider>();
            _collider.radius = 2f;

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Bullet);

            _mover = entity.addComponent(new ProjectileMover());

            // Hack to avoid moving without collider and causing nullreference
            _collider.onAddedToEntity();
            _mover.onAddedToEntity();

        }

        public virtual void OnFired()
        {
            // TO BE IMPLEMENTED
        }

        public virtual void OnNonPlayerImpact(CollisionResult collision)
        {
            entity.destroy();
        }
        public virtual void OnImpact(Player player)
        {
            if (player != null && player != _bullet.Owner)
            {
                DamagePlayer(player);
                _bullet.destroy();
            }
        }

        public virtual void Move()
        {
            if (_bullet.Velocity.Length() == 0) return;

            var isColliding = _mover.move(_bullet.Velocity * Time.deltaTime);
            if (isColliding) {
                HandleCollisions();
            }
        }

        protected void HandleCollisions()
        {
            if (_collider.collidesWithAny(out CollisionResult collision))
            {
                var collidedWithEntity = collision.collider.entity;
                if (Flags.isFlagSet(collidedWithEntity.tag, Tags.Player))
                {
                    HitPlayer(collision.collider.entity);
                }
                else
                {
                    OnNonPlayerImpact(collision);
                }
            }
        }

        protected void Bounce(CollisionResult collision)
        {
            var distance = (collision.point - _collider.absolutePosition);
            _mover.move(-collision.minimumTranslationVector);

            var tangentVector = new Vector2(-distance.Y, distance.X);
            Vector2.Normalize(ref tangentVector, out tangentVector);
            var length = Vector2.Dot(_bullet.Velocity, tangentVector);
            Vector2 velocityComponentOnTangent;
            Vector2.Multiply(ref tangentVector, length, out velocityComponentOnTangent);
            Vector2 velocityComponentPerpendicularToTangent = _bullet.Velocity - velocityComponentOnTangent;
            _bullet.Velocity = new Vector2(_bullet.Velocity.X - 2 * velocityComponentPerpendicularToTangent.X,
                _bullet.Velocity.Y - 2 * velocityComponentPerpendicularToTangent.Y);
        }

        private void HitPlayer(Entity playerEntity)
        {
            var player = playerEntity as Player;
            OnImpact(player);
        }

        protected void DamagePlayer(Player player)
        {
            player.Damage((int)_bullet.Parameters.Damage, _bullet.Velocity);
            player.Velocity += _bullet.Velocity * _bullet.Parameters.Knockback * Time.deltaTime;
        }

        public virtual void update()
        {
            Move();
        }
    }
}
