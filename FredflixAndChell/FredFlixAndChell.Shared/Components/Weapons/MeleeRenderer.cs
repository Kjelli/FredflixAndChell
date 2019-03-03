using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
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
    public class MeleeRenderer : WeaponRenderer
    {
        private Melee _melee;
        private float _renderOffset;
        Sprite<MeleeAnimations> _animation;

        public MeleeRenderer(Melee melee, Player player) : base(player)
        {
            _melee = melee;
            _renderOffset = _melee.Parameters.RenderOffset;
        }

        private Sprite<MeleeAnimations> SetupAnimations(WeaponSprite sprite)
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
            handColorizer.Parameters["hand_color"].SetValue(PlayerSkinColor.ToVector4());
            handColorizer.Parameters["hand_border_color"].SetValue(PlayerSkinColor.subtract(new Color(0.1f, 0.1f, 0.1f, 0.0f)).ToVector4());
            handColorizer.Parameters["replace_color"].SetValue(_melee.Metadata.Color.ToVector4());

            _animation = entity.addComponent(SetupAnimations((_melee.Parameters as MeleeParameters).Sprite));
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

        public override void Fire()
        {
            _animation?.play(MeleeAnimations.Held_Fired);
        }

        public override void update()
        {
            _animation.layerDepth =
                _melee.Parameters.AlwaysAbovePlayer ? (Player.VerticalFacing == (int)FacingCode.UP ? 1 : 0) :
                1 - (entity.position.Y + (Player.VerticalFacing == (int)FacingCode.UP ? -10 : 10)) * Constants.RenderLayerDepthFactor;

            if (_melee.Parameters.FlipYWithPlayer)
            {
                _animation.flipY = Player.FlipGun;

            }
            if (_melee.Parameters.FlipXWithPlayer)
            {
                _animation.flipX = Player.FlipGun;
            }
            entity.position = new Vector2(Player.position.X + (float)Math.Cos(entity.localRotation) * _renderOffset,
                Player.position.Y + (float)Math.Sin(entity.localRotation) * _renderOffset / 2);
            if (_melee.Parameters.RotatesWithPlayer)
            {
                entity.localRotation = (float)Math.Atan2(Player.FacingAngle.Y, Player.FacingAngle.X) + (float)(Math.PI) + _melee.SwingRotation;
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
                _animation.play(MeleeAnimations.Held_Idle);
            }
        }
    }
}
