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
using static FredflixAndChell.Shared.GameObjects.Weapons.Sprites.GunSprite;

namespace FredflixAndChell.Shared.Components.Weapons
{
    public class GunRenderer : WeaponRenderer
    {
        private Gun _gun;
        private float _renderOffset;
        Sprite<GunAnimations> _animation;

        public GunRenderer(Gun gun, Player player) : base(player)
        {
            _gun = gun;
            _renderOffset = _gun.Parameters.RenderOffset;
        }

        private Sprite<GunAnimations> SetupAnimations(WeaponSprite sprite)
        {
            var animations = new Sprite<GunAnimations>();

            var gunSprite = sprite as GunSprite;
            animations.addAnimation(GunAnimations.Held_Fired, gunSprite.Fire.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Reload, gunSprite.Reload.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Held_Idle, gunSprite.Idle.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Held_Empty, gunSprite.Empty.ToSpriteAnimation(sprite.Source));
            return animations;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            entity.setScale(_gun.Parameters.Scale);
            setUpdateOrder(1);

            var handColorizer = AssetLoader.GetEffect("weapon_hand_color");
            handColorizer.Parameters["hand_color"].SetValue(PlayerSkinColor.ToVector4());
            handColorizer.Parameters["hand_border_color"].SetValue(PlayerSkinColor.subtract(new Color(0.1f, 0.1f, 0.1f, 0.0f)).ToVector4());

            _animation = entity.addComponent(SetupAnimations(_gun.Parameters.Sprite));
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

            if (_gun.Ammo + _gun.MagazineAmmo > 0)
            {
                _animation.play(GunAnimations.Held_Idle);
            }
            else
            {
                _animation.play(GunAnimations.Held_Empty);
            }
        }

        public override void Fire()
        {
            if (_gun.Ammo + _gun.MagazineAmmo == 0)
            {
                _animation?.play(GunAnimations.Held_Empty);
            }
            else
            {
                _animation?.play(GunAnimations.Held_Fired);
            }
        }

        public override void update()
        {
            _animation.layerDepth =
                _gun.Parameters.AlwaysAbovePlayer ? (Player.VerticalFacing == (int)FacingCode.UP ? 1 : 0) :
                1 - (entity.position.Y + (Player.VerticalFacing == (int)FacingCode.UP ? -10 : 10)) * Constants.RenderLayerDepthFactor;

            if (_gun.Parameters.FlipYWithPlayer)
            {
                _animation.flipY = Player.FlipGun;

            }
            if (_gun.Parameters.FlipXWithPlayer)
            {
                _animation.flipX = Player.FlipGun;
            }
            entity.position = new Vector2(Player.position.X + (float)Math.Cos(entity.localRotation) * _renderOffset,
                Player.position.Y + (float)Math.Sin(entity.localRotation) * _renderOffset / 2);
            if (_gun.Parameters.RotatesWithPlayer)
            {
                entity.localRotation = (float)Math.Atan2(Player.FacingAngle.Y, Player.FacingAngle.X);
            }

            if (IsPlayerRunning)
            {
                _animation.setLocalOffset(new Vector2(_animation.localOffset.X, (float)Math.Sin(Time.time * 25f) * 0.5f));
            }
            else
            {
                _animation.setLocalOffset(new Vector2(0, 0));
            }

            if (!_animation.isPlaying)
            {
                if (_gun.Ammo + _gun.MagazineAmmo > 0)
                {
                    _animation.play(GunAnimations.Held_Idle);
                }
                else
                {
                    _animation.play(GunAnimations.Held_Empty);
                }
            }
        }

        public void Empty()
        {
            _animation?.play(GunAnimations.Held_Empty);
        }

        public void Reload()
        {
            _animation?.play(GunAnimations.Reload);
        }
    }
}
