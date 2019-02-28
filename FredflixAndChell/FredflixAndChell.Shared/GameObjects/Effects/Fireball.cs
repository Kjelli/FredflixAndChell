using FredflixAndChell.Shared.Components.Effects;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using static FredflixAndChell.Shared.Assets.Constants;
using Random = Nez.Random;

namespace FredflixAndChell.Shared.GameObjects.Effects
{
    public class Fireball : Entity
    {
        private Mover _mover;
        private Vector2 _velocity;
        private Vector2 _initialDirection;
        private float _timeAlive;
        private float _lifespan = 1.1f;
        private float _speed = 30f;
        private bool _shouldBeDestroyed;

        private enum FireballAnimation
        {
            Fade
        }

        public Fireball(Vector2 position, Vector2 direction)
        {
            this.position = position;
            scale = new Vector2(Random.range(0.25f, 1.0f));
            _speed *= Random.range(0.75f, 1.5f);
            _initialDirection = new Vector2(Random.range(-1.5f, 1.5f), -0.5f) * _speed;
            _velocity = direction * _speed;
            _mover = addComponent(new Mover());
        }

        public override void onAddedToScene()
        {
            SetupVisuals();
            Core.schedule(_lifespan, _ => _shouldBeDestroyed = true);
        }

        public override void update()
        {
            base.update();

            if (_shouldBeDestroyed)
            {
                destroy();
                return;
            }

            var progress = _timeAlive / (_lifespan * 0.5f);

            var targetVelocity = (1 - progress) * _initialDirection + progress * _velocity;
            _mover.move(targetVelocity * Time.deltaTime, out _);
            _timeAlive += Time.deltaTime;
        }

        private void SetupVisuals()
        {
            var spriteAnimator = SetupExplosionAnimation();
            spriteAnimator.renderLayer = Layers.PlayerFront;
            spriteAnimator.layerDepth = Random.nextFloat();
            addComponent(spriteAnimator);

            var lightSource = addComponent(new LightSource(Color.Red, this));
            lightSource.LightSprite.tweenColorTo(Color.Black, _lifespan)
                .setEaseType(EaseType.ExpoIn)
                .start();

            spriteAnimator.play(FireballAnimation.Fade);
        }

        private Sprite<FireballAnimation> SetupExplosionAnimation()
        {
            var animator = new Sprite<FireballAnimation>();
            var explodeAnimation = new SpriteAnimationDescriptor
            {
                FPS = 9 / _lifespan,
                Loop = false,
                Frames = new int[]
                {
                    0,0,0,0,1,2,3,4,5
                },
                Origin = new Vector2(8, 8),
                TileHeight = 16,
                TileWidth = 16
            }.ToSpriteAnimation("effects/fireball");
            animator.addAnimation(FireballAnimation.Fade, explodeAnimation);

            return animator;
        }
    }


}