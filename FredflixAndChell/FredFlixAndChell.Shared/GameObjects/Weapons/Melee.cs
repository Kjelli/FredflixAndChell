using FredflixAndChell.Shared.Components.Weapons;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using System;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public enum MeleeState
    {
        Idle, Attacking
    }

    public enum MeleeAttackState
    {
        None, Forward, Backward
    }

    public class Melee : Weapon
    {
        private const float _swingTargetRadians = (float)Math.PI;

        private MeleeRenderer _renderer;
        protected Collider _collider;

        public float SwingRotation { get; set; }

        public MeleeParameters Parameters { get; }
        public Player Player { get; set; }

        public Melee(Player player, MeleeParameters meleeParameters) : base(player)
        {
            Player = player;
            Parameters = meleeParameters;
            SetupParameters();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _renderer = addComponent(new MeleeRenderer(this, Player));

            var collider = addComponent<CircleCollider>();
            collider.radius = 5f;
            _collider = collider;
            _collider.setLocalOffset(new Vector2(10, 0));

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.Player);
        }

        private void SetupParameters()
        {
            Cooldown = new Cooldown(Parameters.FireRate);
        }

        public override void Fire()
        {
            if (Cooldown.IsReady())
            {
                //Functionality
                Cooldown.Start();
                //var dir = (float)Math.Atan2(_player.FacingAngle.Y, _player.FacingAngle.X);
                //var x = (float)(position.X
                //    + Math.Cos(localRotation) * _barrelOffset.X
                //    + Math.Cos(localRotation) * _barrelOffset.Y);
                //var y = (float)(position.Y
                //    + Math.Sin(localRotation) * _barrelOffset.Y
                //    + Math.Sin(localRotation) * _barrelOffset.X);
                //for (float i = 0; i < _bulletCount; i++)
                //{
                //    var direction = dir + (1 - _accuracy) * Nez.Random.minusOneToOne() / 2
                //    + (1 - _accuracy) * _player.Velocity.Length() * Nez.Random.minusOneToOne()
                //    + ((i * 2 - _bulletCount) * _bulletSpread / _bulletCount);

                //    Bullet.Create(_player, x, y, direction, Parameters.BulletParameters);
                //}

                //Animation
                _renderer?.Fire();
            }
        }

        public override void OnDespawn()
        {
        }

        public override void Update()
        {
            base.Update();
            CheckCollision();
        }

        private void CheckCollision()
        {
            if (_collider.collidesWithAny(out CollisionResult collision))
            {
                var collidedWithEntity = collision.collider.entity;
                if (collidedWithEntity.tag == Tags.Player)
                {
                    HitPlayer(collision.collider.entity);
                }

            }
        }

        private void HitPlayer(Entity playerEntity)
        {
            var player = playerEntity as Player;
            if (player.PlayerMobilityState == PlayerMobilityState.Rolling) return;

            OnImpact(player);
        }

        public virtual void OnImpact(Player player)
        {
            if (player != null && player != Player)
            {
                DamagePlayer(player);
            }
        }

        protected bool DamagePlayer(Player player, bool destroyBullet = true)
        {
            //if (player.CanBeDamagedBy(this))
            //{
            player.Damage(this);
            return true;
            //}
            //return false;
        }

        public override void ToggleRunning(bool isRunning)
        {
            _renderer?.ToggleRunningDisplacement(isRunning);
        }
    }
}
