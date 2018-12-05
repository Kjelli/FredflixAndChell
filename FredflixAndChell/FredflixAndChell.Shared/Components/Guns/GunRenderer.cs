﻿using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.GameObjects.Weapons.Sprites.GunSprite;

namespace FredflixAndChell.Shared.Components.Guns
{
    public class GunRenderer : Component, IUpdatable
    {
        private Player _player;
        private Gun _gun;
        private bool _isPlayerRunning;

        private float _renderOffset;
        Sprite<GunAnimations> _animation;

        public GunRenderer(Gun gun, Player player)
        {
            _gun = gun;
            _player = player;
            _renderOffset = _gun.Parameters.RenderOffset;
        }

        private Sprite<GunAnimations> SetupAnimations(GunSprite sprite)
        {
            var animations = new Sprite<GunAnimations>();

            animations.addAnimation(GunAnimations.Held_Fired, sprite.Fire.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Reload, sprite.Reload.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Held_Idle, sprite.Idle.ToSpriteAnimation(sprite.Source));

            return animations;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            entity.setScale(_gun.Parameters.Scale);
            setUpdateOrder(1);

            _animation = entity.addComponent(SetupAnimations(_gun.Parameters.Sprite));
            _animation.renderLayer = Layers.Player;

            var shadow = entity.addComponent(new SpriteMime(_animation));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when gun is visually blocked
            var silhouette = entity.addComponent(new SpriteMime(_animation));
            silhouette.color = new Color(0, 0, 0, 80);
            silhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            silhouette.renderLayer = Layers.Foreground;
            silhouette.localOffset = new Vector2(0, 0);

            _animation.play(GunAnimations.Held_Idle);
        }

        public void ToggleRunningDisplacement(bool isRunning)
        {
            _isPlayerRunning = isRunning;
        }

        public void Fire()
        {
            _animation?.play(GunAnimations.Held_Fired);
        }

        public void update()
        {
            _animation.layerDepth =
                _gun.Parameters.AlwaysAbovePlayer ? (_player.VerticalFacing == (int)FacingCode.UP ? 1 : 0 ) :
                1 - (entity.position.Y +(_player.VerticalFacing == (int)FacingCode.UP ? -10 : 10)) * Constants.RenderLayerDepthFactor;

            if (_gun.Parameters.FlipYWithPlayer)
            {
                _animation.flipY = _player.FlipGun;

            }
            if (_gun.Parameters.FlipXWithPlayer)
            {
                _animation.flipX = _player.FlipGun;
            }
            entity.position = new Vector2(_player.position.X + (float)Math.Cos(entity.localRotation) * _renderOffset,
                _player.position.Y + (float)Math.Sin(entity.localRotation) * _renderOffset / 2);
            if (_gun.Parameters.RotatesWithPlayer)
            {
                entity.localRotation = (float)Math.Atan2(_player.FacingAngle.Y, _player.FacingAngle.X);
            }

            if (_isPlayerRunning)
            {
                _animation.setLocalOffset(new Vector2(_animation.localOffset.X, (float)Math.Sin(Time.time * 25f) * 0.5f));
            }
            else
            {
                _animation.setLocalOffset(new Vector2(0, 0));
            }

            if (!_animation.isPlaying)
            {
                _animation.play(GunAnimations.Held_Idle);
            }
        }

        public void Reload()
        {
            _animation?.play(GunAnimations.Reload);
        }
    }
}
