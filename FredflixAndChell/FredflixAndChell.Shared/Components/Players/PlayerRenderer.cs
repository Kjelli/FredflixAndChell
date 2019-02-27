using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Effects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerHeadSprite;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerLegsSprite;
using static FredflixAndChell.Shared.GameObjects.Players.Sprites.PlayerTorsoSprite;

namespace FredflixAndChell.Shared.Components.Players
{
    public class PlayerRenderer : Component, IUpdatable
    {
        private Player _player;
        private Weapon _weapon;
        private PlayerSprite _playerSprite;
        private float _facingDepthOffset;
        private Sprite<TorsoAnimation> _torso;
        private Sprite<LegsAnimation> _legs;
        private LightSource _light;

        private SpriteMime _headShadow;
        private SpriteMime _torsoShadow;
        private SpriteMime _legsShadow;
        private SpriteMime _headSilhouette;
        private SpriteMime _torsoSilhouette;
        private SpriteMime _legsSilhouette;

        public Sprite<HeadAnimation> Head { get; set; }

        public PlayerRenderer(PlayerSprite playerSprite, Weapon weapon)
        {
            _playerSprite = playerSprite;
            _weapon = weapon;
        }

        public override void onAddedToEntity()
        {
            _player = entity as Player;

            SetupPlayerSprites();
            SetupShadow();
            SetupSilhouette();
            SetupLightsource();

            UpdateRenderLayerDepth();
        }

        private void SetupLightsource()
        {
            _light = _player.addComponent(new LightSource(Color.White));
        }

        private void SetupSilhouette()
        {
            // Assign silhouette component when player is visually blocked

            _headSilhouette = entity.addComponent(new SpriteMime(Head));
            _headSilhouette.color = new Color(0, 0, 0, 80);
            _headSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            _headSilhouette.renderLayer = Layers.Foreground;
            _headSilhouette.localOffset = new Vector2(0, 0);

            _torsoSilhouette = entity.addComponent(new SpriteMime(_torso));
            _torsoSilhouette.color = new Color(0, 0, 0, 80);
            _torsoSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            _torsoSilhouette.renderLayer = Layers.Foreground;
            _torsoSilhouette.localOffset = new Vector2(0, 0);

            _legsSilhouette = entity.addComponent(new SpriteMime(_legs));
            _legsSilhouette.color = new Color(0, 0, 0, 80);
            _legsSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            _legsSilhouette.renderLayer = Layers.Foreground;
            _legsSilhouette.localOffset = new Vector2(0, 0);
        }

        private void SetupShadow()
        {
            // Assign renderable shadow component
            _headShadow = entity.addComponent(new SpriteMime(Head));
            _headShadow.color = new Color(0, 0, 0, 80);
            _headShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            _headShadow.renderLayer = Layers.Shadow;
            _headShadow.localOffset = new Vector2(1, 2);

            _torsoShadow = entity.addComponent(new SpriteMime(_torso));
            _torsoShadow.color = new Color(0, 0, 0, 80);
            _torsoShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            _torsoShadow.renderLayer = Layers.Shadow;
            _torsoShadow.localOffset = new Vector2(1, 2);

            _legsShadow = entity.addComponent(new SpriteMime(_legs));
            _legsShadow.color = new Color(0, 0, 0, 80);
            _legsShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            _legsShadow.renderLayer = Layers.Shadow;
            _legsShadow.localOffset = new Vector2(1, 2);
        }

        private void SetupPlayerSprites()
        {
            // Assign renderable (animation) component
            var headAnimations = SetupHeadAnimations(_playerSprite.Head);
            Head = entity.addComponent(headAnimations);
            Head.renderLayer = Layers.Player;

            var torsoAnimations = SetupTorsoAnimations(_playerSprite.Torso);
            _torso = entity.addComponent(torsoAnimations);
            _torso.renderLayer = Layers.Player;

            var legsAnimations = SetupLegsAnimations(_playerSprite.Legs);
            _legs = entity.addComponent(legsAnimations);
            _legs.renderLayer = Layers.Player;

            Head.play(HeadAnimation.FrontFacing);
            _torso.play(TorsoAnimation.Front);
            _legs.play(LegsAnimation.Idle);
        }

        private Sprite<HeadAnimation> SetupHeadAnimations(PlayerHeadSprite headSprite)
        {
            var animations = new Sprite<HeadAnimation>();
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
            Head.tweenColorTo(c, durationSeconds)
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
            if (float.IsNaN(_player.position.X) || float.IsNaN(_player.position.Y)
                || float.IsNaN(_player.FacingAngle.X) || float.IsNaN(_player.FacingAngle.Y)) return;
            var hit = Physics.linecast(_player.position, _player.position + _player.FacingAngle * 1000f);
            Debug.drawLine(_player.position, _player.position + _player.FacingAngle * 1000f, Color.Gray);
        }

        public void UpdateRenderLayerDepth()
        {
            if (Head != null) Head.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y + _facingDepthOffset) * Constants.RenderLayerDepthFactor;
            if (_torso != null) _torso.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y - _facingDepthOffset) * Constants.RenderLayerDepthFactor;
            if (_legs != null) _legs.layerDepth = 1 - (entity.position.Y + _player.FacingAngle.Y - _facingDepthOffset) * Constants.RenderLayerDepthFactor;
        }

        private void UpdateAnimation()
        {
            //Todo: Fix check of unmarmed. A gun type called unarmed?
            bool armed = _player.IsArmed();

            // Select Animations (Idle initially)
            HeadAnimation headAnimation = HeadAnimation.FrontFacing;
            TorsoAnimation torsoAnimation = TorsoAnimation.Front;
            LegsAnimation legsAnimation = LegsAnimation.Idle;

            // Head
            if (_player.PlayerState == PlayerState.Dead)
            {
                headAnimation = Head.currentAnimation;
                Head.pause();
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
                Head.pause();
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

            if (!Head.isAnimationPlaying(headAnimation))
            {
                Head.play(headAnimation);
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

        public override void onEnabled()
        {
            _light.setEnabled(true);

            Head.setEnabled(true);
            _headShadow.setEnabled(true);
            _headSilhouette.setEnabled(true);

            _torso.setEnabled(true);
            _torsoShadow.setEnabled(true);
            _torsoSilhouette.setEnabled(true);

            _legs.setEnabled(true);
            _legsShadow.setEnabled(true);
            _legsSilhouette.setEnabled(true);
        }

        public override void onDisabled()
        {
            _light.setEnabled(false);

            Head.setEnabled(false);
            _headShadow.setEnabled(false);
            _headSilhouette.setEnabled(false);

            _torso.setEnabled(false);
            _torsoShadow.setEnabled(false);
            _torsoSilhouette.setEnabled(false);

            _legs.setEnabled(false);
            _legsShadow.setEnabled(false);
            _legsSilhouette.setEnabled(false);
        }

        public override void onRemovedFromEntity()
        {
            Head.removeComponent();
            _headShadow.removeComponent();
            _headSilhouette.removeComponent();

            _torso.removeComponent();
            _torsoShadow.removeComponent();
            _torsoSilhouette.removeComponent();

            _legs.removeComponent();
            _legsShadow.removeComponent();
            _legsSilhouette.removeComponent();

            _light.removeComponent();
        }

        public void FlipX(bool isFlipped)
        {
            if (Head != null) Head.flipX = isFlipped;
            if (_torso != null) _torso.flipX = isFlipped;
            if (_legs != null) _legs.flipX = isFlipped;
        }
    }
}
