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
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerTorsoSprite;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerHeadSprite;
using System;
using Nez.Tweens;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerLegsSprite;
using FredflixAndChell.Shared.Components.Players;

namespace FredflixAndChell.Shared.Components.PlayerComponents
{
    public class PlayerRenderer : Component, IUpdatable
    {
        private Player _player;
        private Gun _gun;
        private PlayerSprite _playerSprite;
        private float _facingDepthOffset;

        Sprite<HeadAnimation> _head;
        Sprite<TorsoAnimation> _torso;
        Sprite<LegsAnimation> _legs;

        public PlayerRenderer(PlayerSprite playerSprite, Gun gun)
        {
            _playerSprite = playerSprite;
            _gun = gun;
        }

        public override void onAddedToEntity()
        {
            _player = entity as Player;

            SetupPlayerSprites();

            // Assign faint glow to player
            var light = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
            light.material = Material.blendLinearDodge();
            light.color = Color.White;
            light.renderLayer = Layers.Lights;

            // Assign renderable shadow component
            var torsoShadow = entity.addComponent(new SpriteMime(_torso));
            torsoShadow.color = new Color(0, 0, 0, 80);
            torsoShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            torsoShadow.renderLayer = Layers.Shadow;
            torsoShadow.localOffset = new Vector2(1, 2);

            var legsShadow = entity.addComponent(new SpriteMime(_legs));
            legsShadow.color = new Color(0, 0, 0, 80);
            legsShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            legsShadow.renderLayer = Layers.Shadow;
            legsShadow.localOffset = new Vector2(1, 2);


            var headShadow = entity.addComponent(new SpriteMime(_head));
            headShadow.color = new Color(0, 0, 0, 80);
            headShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            headShadow.renderLayer = Layers.Shadow;
            headShadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when player is visually blocked
            var torsoSilhouette = entity.addComponent(new SpriteMime(_torso));
            torsoSilhouette.color = new Color(0, 0, 0, 80);
            torsoSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            torsoSilhouette.renderLayer = Layers.Foreground;
            torsoSilhouette.localOffset = new Vector2(0, 0);

            var legsSilhouette = entity.addComponent(new SpriteMime(_legs));
            legsSilhouette.color = new Color(0, 0, 0, 80);
            legsSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            legsSilhouette.renderLayer = Layers.Foreground;
            legsSilhouette.localOffset = new Vector2(0, 0);

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
            var headAnimations = SetupHeadAnimations(_playerSprite.Head);
            _head = entity.addComponent(headAnimations);
            _head.renderLayer = Layers.Player;

            var torsoAnimations = SetupTorsoAnimations(_playerSprite.Torso);
            _torso = entity.addComponent(torsoAnimations);
            _torso.renderLayer = Layers.Player;

            var legsAnimations = SetupLegsAnimations(_playerSprite.Legs);
            _legs = entity.addComponent(legsAnimations);
            _legs.renderLayer = Layers.Player;

            _head.play(HeadAnimation.FrontFacing);
            _torso.play(TorsoAnimation.Front);
            _legs.play(LegsAnimation.Idle);
        }

        private Sprite<HeadAnimation> SetupHeadAnimations(PlayerHeadSprite headSprite)
        {
            var animations = new Foo<HeadAnimation>();

            animations.addAnimation(HeadAnimation.FrontFacing,
                headSprite.Front.ToSpriteAnimation(_playerSprite.Source + "/head"));
            animations.addAnimation(HeadAnimation.BackFacing,
                headSprite.Back.ToSpriteAnimation(_playerSprite.Source + "/head"));

            return animations;
        }

        private Sprite<TorsoAnimation> SetupTorsoAnimations(PlayerTorsoSprite torsoSprite)
        {
            var animations = new Sprite<TorsoAnimation>();

            animations.addAnimation(TorsoAnimation.Front,
                torsoSprite.Front.ToSpriteAnimation(_playerSprite.Source + "/torso"));
            animations.addAnimation(TorsoAnimation.Back,
                torsoSprite.Back.ToSpriteAnimation(_playerSprite.Source + "/torso"));
            animations.addAnimation(TorsoAnimation.FrontUnarmed,
                torsoSprite.FrontUnarmed.ToSpriteAnimation(_playerSprite.Source + "/torso"));
            animations.addAnimation(TorsoAnimation.BackUnarmed,
                torsoSprite.BackUnarmed.ToSpriteAnimation(_playerSprite.Source + "/torso"));

            return animations;
        }

        private Sprite<LegsAnimation> SetupLegsAnimations(PlayerLegsSprite legsSprite)
        {
            var animations = new Sprite<LegsAnimation>();

            animations.addAnimation(LegsAnimation.Idle,
                legsSprite.Idle.ToSpriteAnimation(_playerSprite.Source + "/legs"));
            animations.addAnimation(LegsAnimation.Walking,
                legsSprite.Walking.ToSpriteAnimation(_playerSprite.Source + "/legs"));

            return animations;
        }

        public void TweenColor(Color c, float durationSeconds, EaseType easeType = EaseType.CubicOut)
        {
            _head.tweenColorTo(c, durationSeconds)
                .setEaseType(easeType)
                .start();
            _torso.tweenColorTo(c, durationSeconds)
                .setEaseType(easeType)
                .start();
            _legs.tweenColorTo(c, durationSeconds)
                .setEaseType(easeType)
                .start();
        }


        public void update()
        {
            UpdateAnimation();
        }

        public void UpdateRenderLayerDepth()
        {
            _head.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y + _facingDepthOffset) * Constants.RenderLayerDepthFactor;
            _torso.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y - _facingDepthOffset) * Constants.RenderLayerDepthFactor;
            _legs.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y - _facingDepthOffset) * Constants.RenderLayerDepthFactor;
        }

        private void UpdateAnimation()
        {
            //Todo: Fix check of unmarmed. A gun type called unarmed?
            bool armed = _player.IsArmed;

            // Select Animations (Idle initially)
            HeadAnimation headAnimation = HeadAnimation.FrontFacing;
            TorsoAnimation torsoAnimation = TorsoAnimation.Front;
            LegsAnimation legsAnimation = LegsAnimation.Idle;

            // Head
            if (_player.PlayerState == PlayerState.Dead)
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

            // Torso
            if (_player.PlayerState == PlayerState.Dead)
            {
                torsoAnimation = _torso.currentAnimation;
                _head.pause();
            }
            else if (_player.VerticalFacing == (int)FacingCode.UP)
            {
                _facingDepthOffset = -20 * Constants.RenderLayerDepthFactor;
                torsoAnimation = armed ? TorsoAnimation.Back : TorsoAnimation.BackUnarmed;
            }
            else if (_player.VerticalFacing == (int)FacingCode.DOWN)
            {
                _facingDepthOffset = 20 * Constants.RenderLayerDepthFactor;
                torsoAnimation = armed ? TorsoAnimation.Front : TorsoAnimation.FrontUnarmed;
            }

            // Legs
            if (_player.PlayerState == PlayerState.Dead || _player.PlayerState == PlayerState.Dying)
            {
                legsAnimation = LegsAnimation.Idle;
                _legs.pause();
            }
            else if (_player.Acceleration.Length() > 0)
            {
                legsAnimation = LegsAnimation.Walking;
            }


            // Play Animations

            if (!_head.isAnimationPlaying(headAnimation))
            {
                _head.play(headAnimation);
            }

            if (!_torso.isAnimationPlaying(torsoAnimation))
            {
                _torso.play(torsoAnimation);
            }

            if (!_legs.isAnimationPlaying(legsAnimation))
            {
                _legs.play(legsAnimation);
            }
        }

        public void FlipX(bool isFlipped)
        {
            if (_head != null) _head.flipX = isFlipped;
            if (_torso != null) _torso.flipX = isFlipped;
            if (_legs != null) _legs.flipX = isFlipped;
        }
    }
}
