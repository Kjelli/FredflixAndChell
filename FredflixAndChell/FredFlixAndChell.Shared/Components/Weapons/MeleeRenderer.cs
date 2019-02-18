using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.GameObjects.Weapons.Sprites.MeleeSprite;

namespace FredflixAndChell.Shared.Components.Weapons
{
    public class MeleeRenderer : Component, IUpdatable
    {
        private Player _player;
        private Melee _melee;
        private bool _isPlayerRunning;

        private float _renderOffset;
        private Color _playerSkinColor;
        Sprite<MeleeAnimations> _animation;

        public MeleeRenderer(Melee melee, Player player)
        {
            _melee = melee;
            _player = player;
            _renderOffset = _melee.Parameters.RenderOffset;
            _playerSkinColor = player.Parameters.SkinColor;
        }

        private Sprite<MeleeAnimations> SetupAnimations(MeleeSprite sprite)
        {
            var animations = new Sprite<MeleeAnimations>();

            animations.addAnimation(MeleeAnimations.Held_Fired, sprite.Fire.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(MeleeAnimations.Held_Idle, sprite.Idle.ToSpriteAnimation(sprite.Source));
            return animations;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            entity.setScale(_melee.Parameters.Scale);
            setUpdateOrder(1);

            var handColorizer = AssetLoader.GetEffect("weapon_hand_color");
            handColorizer.Parameters["hand_color"].SetValue(_playerSkinColor.ToVector4());
            handColorizer.Parameters["hand_border_color"].SetValue(_playerSkinColor.subtract(new Color(0.1f, 0.1f, 0.1f, 0.0f)).ToVector4());

            _animation = entity.addComponent(SetupAnimations(_melee.Parameters.Sprite));
            _animation.renderLayer = Layers.Player;
            _animation.material = new Material(BlendState.NonPremultiplied, handColorizer);

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

            _animation.play(MeleeAnimations.Held_Idle);
        }

        public void ToggleRunningDisplacement(bool isRunning)
        {
            _isPlayerRunning = isRunning;
        }

        public void Fire()
        {
            _animation?.play(MeleeAnimations.Held_Fired);
        }

        public void update()
        {
            _animation.layerDepth =
                _melee.Parameters.AlwaysAbovePlayer ? (_player.VerticalFacing == (int)FacingCode.UP ? 1 : 0) :
                1 - (entity.position.Y + (_player.VerticalFacing == (int)FacingCode.UP ? -10 : 10)) * Constants.RenderLayerDepthFactor;

            if (_melee.Parameters.FlipYWithPlayer)
            {
                _animation.flipY = _player.FlipGun;

            }
            if (_melee.Parameters.FlipXWithPlayer)
            {
                _animation.flipX = _player.FlipGun;
            }
            entity.position = new Vector2(_player.position.X + (float)Math.Cos(entity.localRotation) * _renderOffset,
                _player.position.Y + (float)Math.Sin(entity.localRotation) * _renderOffset / 2);
            if (_melee.Parameters.RotatesWithPlayer)
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
                _animation.play(MeleeAnimations.Held_Idle);
            }
        }

        //public void Reload()
        //{
        //    _animation?.play(GunAnimations.Reload);
        //}
    }
}
