﻿using FredflixAndChell.Shared.GameObjects.Bullets;
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
        protected Collider _collider;
        protected ProjectileMover _mover;

        public BulletBehaviour(Bullet bullet)
        {
            _bullet = bullet;
        }

        public override void onAddedToEntity()
        {

            if (_bullet.Parameters.BulletType == BulletType.Entity)
            {
                SetupEntityBullet();
            }

            if (_bullet.Parameters.BulletType == BulletType.Line)
            {
                SetupLineBullet();
            }
            OnFired();
        }

        private void SetupLineBullet()
        {
            // TODO
        }

        private void SetupEntityBullet()
        {
            var collider = entity.addComponent<CircleCollider>();
            collider.radius = 2f;
            _collider = collider;

            _mover = _bullet.addComponent(new ProjectileMover());

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Bullet);

            // Hack to avoid moving without collider and causing nullreference

            _collider.onAddedToEntity();
            _mover.onAddedToEntity();
        }

        public virtual void OnFired()
        {

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
            }
        }

        public virtual void Move()
        {
            if (_bullet.Velocity.Length() == 0) return;

            var isColliding = _mover.move(_bullet.Velocity * Time.deltaTime);
            if (isColliding)
            {
                HandleCollisions();
            }
        }

        protected void HandleCollisions()
        {
            if (_collider.collidesWithAny(out CollisionResult collision))
            {
                var collidedWithEntity = collision.collider.entity;
                if (collidedWithEntity.tag == Tags.Player)
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
            if (player.PlayerMobilityState == PlayerMobilityState.Rolling) return;

            OnImpact(player);
        }
        /// <summary>
		/// Returns true and damages player if it can be damaged, returns false if player cannot be damaged by the bullet.
		/// </summary>
        protected bool DamagePlayer(Player player, bool destroyBullet = true)
        {
            if (player.CanBeDamagedBy(_bullet))
            {
                player.Damage(_bullet);
                if (destroyBullet)
                {
                    _bullet.destroy();
                }
                return true;
            }
            return false;
        }

        public virtual void update()
        {
            Move();
        }
    }
}
