using FredflixAndChell.Shared.Components.Weapons;
using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public enum MeleeAttackState
    {
        None, Forward, Backward
    }

    public class Melee : Weapon
    {
        private const float _swingTargetRadians = (float)Math.PI;
        private const float _epsilon = 0.01f;
        private const int _faceLeft = -1, _faceRight = 1;

        private MeleeRenderer _renderer;
        private MeleeParameters _parameters;
        private MeleeMetadata _metadata;
        private MeleeAttackState _attackState;

        private List<Player> _playersHitOnSwing;

        public override WeaponParameters Parameters => _parameters;
        public override CollectibleMetadata Metadata => _metadata;

        protected Collider _hitbox;
        private int _swingFacing = _faceRight;

        public float SwingRotation { get; set; }

        public Player Player { get; set; }

        public Melee(Player player, MeleeParameters meleeParameters, MeleeMetadata metadata = null) : base(player)
        {
            Player = player;
            _parameters = meleeParameters;
            _playersHitOnSwing = new List<Player>();
            _metadata = metadata ?? new MeleeMetadata();
            SetupParameters();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _renderer = addComponent(new MeleeRenderer(this, Player));
            _hitbox = addComponent(new BoxCollider(_parameters.HitboxSize.X, _parameters.HitboxSize.Y)); ;
            _hitbox.setLocalOffset(new Vector2(_parameters.HitboxOffset.X, _parameters.HitboxOffset.Y));

            _attackState = MeleeAttackState.None;

            Flags.setFlagExclusive(ref _hitbox.collidesWithLayers, Layers.Player);
        }

        private void SetupParameters()
        {
            Cooldown = new Cooldown(_parameters.FireRate);
            Cooldown.Start();
        }

        public override void Fire()
        {
            switch (_parameters.MeleeType)
            {
                case MeleeType.Hold:
                    if (Cooldown.IsReady())
                    {
                        Cooldown.Start();
                        _renderer?.Fire();
                    }
                    break;
                case MeleeType.Swing:
                    if (Cooldown.IsReady() && _attackState == MeleeAttackState.None)
                    {
                        Cooldown.Start();
                        _renderer?.Fire();
                        _swingFacing = Player.HorizontalFacing == (int)FacingCode.LEFT ? _faceLeft : _faceRight;
                        _attackState = MeleeAttackState.Forward;
                    }
                    break;
            }

        }

        public override void OnDespawn()
        {
        }

        public override void Update()
        {
            base.Update();
            Cooldown.Update();
            CheckCollision();
            switch (_attackState)
            {
                case MeleeAttackState.None:
                    break;
                case MeleeAttackState.Forward:
                    SwingRotation = Lerps.lerpTowards(SwingRotation, _swingFacing * _swingTargetRadians, 0.01f, Time.deltaTime * 10f);
                    if (Math.Abs(_swingFacing * SwingRotation) >= _swingTargetRadians - _epsilon)
                    {
                        _attackState = MeleeAttackState.Backward;
                    }
                    break;
                case MeleeAttackState.Backward:
                    SwingRotation = Lerps.lerpTowards(SwingRotation, 0, 0.01f, Time.deltaTime * 5f);
                    if (Math.Abs(_swingFacing * SwingRotation) <= _epsilon)
                    {
                        _attackState = MeleeAttackState.None;
                        _playersHitOnSwing.Clear();
                    }
                    break;
            }
        }

        private void CheckCollision()
        {
            if (_hitbox.collidesWithAny(out CollisionResult collision))
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
            if (_playersHitOnSwing.Contains(player)) return;

            _playersHitOnSwing.Add(player);
            OnImpact(player);
        }

        public virtual void OnImpact(Player player)
        {
            if (player == null) return;

            switch (_parameters.MeleeType)
            {
                case MeleeType.Hold:
                    if (player != Player && Cooldown.IsOnCooldown())
                    {
                        DamagePlayer(player);
                    }
                    break;
                case MeleeType.Swing:
                    if (player != Player & _attackState == MeleeAttackState.Forward)
                    {
                        DamagePlayer(player);
                    }
                    break;
            }

        }

        protected bool DamagePlayer(Player player)
        {
            player.Damage(this);
            return true;
        }

        public override void ToggleRunning(bool isRunning)
        {
            _renderer?.ToggleRunningDisplacement(isRunning);
        }
    }
}
