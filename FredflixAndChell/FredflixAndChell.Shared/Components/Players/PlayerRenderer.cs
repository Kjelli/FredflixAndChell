using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerBodySprite;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerHeadSprite;
using System;
using Nez.Tweens;

namespace FredflixAndChell.Shared.Components.PlayerComponents
{
    public class PlayerRenderer : Component, IUpdatable
    {
        private Player _player;
        private Gun _gun;
        private PlayerSprite _playerSprite;
        private float _facingDepthOffset;

        Sprite<BodyAnimation> _body;
        Sprite<HeadAnimation> _head;

        public PlayerRenderer(PlayerSprite playerSprite, Gun gun)
        {
            _playerSprite = playerSprite;
            _gun = gun;
        }

        public override void onAddedToEntity()
        {
            _player = entity.getComponent<Player>();

            SetupPlayerSprites();

            // Assign faint glow to player
            var light = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
            light.material = Material.blendLinearDodge();
            light.color = Color.White;
            light.renderLayer = Layers.Lights;

            // Assign renderable shadow component
            var bodyShadow = entity.addComponent(new SpriteMime(_body));
            bodyShadow.color = new Color(0, 0, 0, 80);
            bodyShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            bodyShadow.renderLayer = Layers.Shadow;
            bodyShadow.localOffset = new Vector2(1, 2);


            var headShadow = entity.addComponent(new SpriteMime(_head));
            headShadow.color = new Color(0, 0, 0, 80);
            headShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            headShadow.renderLayer = Layers.Shadow;
            headShadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when player is visually blocked
            var bodySilhouette = entity.addComponent(new SpriteMime(_body));
            bodySilhouette.color = new Color(0, 0, 0, 80);
            bodySilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            bodySilhouette.renderLayer = Layers.Foreground;
            bodySilhouette.localOffset = new Vector2(0, 0);

            var headSilhouette = entity.addComponent(new SpriteMime(_head));
            headSilhouette.color = new Color(0, 0, 0, 80);
            headSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            headSilhouette.renderLayer = Layers.Foreground;
            headSilhouette.localOffset = new Vector2(0, 0);

            UpdateRenderLayerDepth();
        }

        private void SetupPlayerSprites()
        {
            // Assign renderable (animation) component
            var bodyAnimations = SetupBodyAnimations(_playerSprite.Body);
            _body = entity.addComponent(bodyAnimations);
            _body.renderLayer = Layers.Player;

            var headAnimations = SetupHeadAnimations(_playerSprite.Head);
            _head = entity.addComponent(headAnimations);
            _head.renderLayer = Layers.Player;

            _body.play(BodyAnimation.Idle);
            _head.play(HeadAnimation.FrontFacing);
        }
        private Sprite<BodyAnimation> SetupBodyAnimations(PlayerBodySprite bodySprite)
        {
            var animations = new Sprite<BodyAnimation>();

            animations.addAnimation(BodyAnimation.Idle, 
                bodySprite.Idle.ToSpriteAnimation(_playerSprite.Source + "_body"));
            animations.addAnimation(BodyAnimation.IdleUnarmed, 
                bodySprite.IdleUnarmed.ToSpriteAnimation(_playerSprite.Source + "_body"));
            animations.addAnimation(BodyAnimation.Walking, 
                bodySprite.Walking.ToSpriteAnimation(_playerSprite.Source + "_body"));
            animations.addAnimation(BodyAnimation.WalkingUnarmed, 
                bodySprite.WalkingUnarmed.ToSpriteAnimation(_playerSprite.Source + "_body"));

            return animations;
        }

        public void TweenColor(Color c, float durationSeconds, EaseType easeType = EaseType.CubicOut)
        {
            _head.tweenColorTo(c, durationSeconds)
                .setEaseType(easeType)
                .start();
            _body.tweenColorTo(c, durationSeconds)
                .setEaseType(easeType)
                .start();
        }

        private Sprite<HeadAnimation> SetupHeadAnimations(PlayerHeadSprite headSprite)
        {
            var animations = new Sprite<HeadAnimation>();

            animations.addAnimation(HeadAnimation.FrontFacing,
                headSprite.Front.ToSpriteAnimation(_playerSprite.Source + "_head"));
            animations.addAnimation(HeadAnimation.BackFacing,
                headSprite.Back.ToSpriteAnimation(_playerSprite.Source + "_head"));

            return animations;
        }

        public void update()
        {
            UpdateAnimation();
        }

        public void UpdateRenderLayerDepth()
        {
            _head.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y + _facingDepthOffset) * Constants.RenderLayerDepthFactor;
            _body.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y - _facingDepthOffset) * Constants.RenderLayerDepthFactor;
        }

        private void UpdateAnimation()
        {
            //Todo: Fix check of unmarmed. A gun type called unarmed?
            bool armed = _player.IsArmed;

            // Select Animations (Idle initially)
            BodyAnimation bodyAnimation = armed ? BodyAnimation.Idle : BodyAnimation.IdleUnarmed;
            HeadAnimation headAnimation = HeadAnimation.FrontFacing;

            // Body
            if(_player.PlayerState == PlayerState.Dead || _player.PlayerState == PlayerState.Dying)
            {
                bodyAnimation = BodyAnimation.IdleUnarmed;
                _body.pause();
            }
            else if (_player.Acceleration.Length() > 0)
            {
                bodyAnimation = armed ? BodyAnimation.Walking : BodyAnimation.WalkingUnarmed;
            }

            // Head
            if(_player.PlayerState == PlayerState.Dead)
            {
                headAnimation = _head.currentAnimation;
                _head.pause();
            }
            else if (_player.VerticalFacing == (int)FacingCode.UP)
            {
                _facingDepthOffset = -20 * Constants.RenderLayerDepthFactor;
                headAnimation = HeadAnimation.BackFacing;
            }
            else if (_player.VerticalFacing == (int)FacingCode.DOWN)
            {
                _facingDepthOffset = 20 * Constants.RenderLayerDepthFactor;
                headAnimation = HeadAnimation.FrontFacing;
            }

            // Play Animations

            if (!_body.isAnimationPlaying(bodyAnimation))
            {
                _body.play(bodyAnimation);
            }

            if (!_head.isAnimationPlaying(headAnimation))
            {
                _head.play(headAnimation);
            }

        }

        public void FlipX(bool isFlipped)
        {
            if (_body != null) _body.flipX = isFlipped;
            if (_head != null) _head.flipX = isFlipped;
        }
    }
}
