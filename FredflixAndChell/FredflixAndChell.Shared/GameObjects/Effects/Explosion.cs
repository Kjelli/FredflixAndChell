using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Effects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using System;
using static FredflixAndChell.Shared.Assets.Constants;
using Random = Nez.Random;

namespace FredflixAndChell.Shared.GameObjects.Effects
{
    public class Explosion : Entity
    {
        private CircleCollider _collider;
        private bool _shouldBeDestroyed;

        public Player ExplosionPlayerSource { get; set; }

        private enum ExplosionAnimation
        {
            Explode
        }

        public Explosion(Vector2 position, Player playerSource = null)
        {
            this.position = position;
            ExplosionPlayerSource = playerSource;

            _collider = addComponent(new CircleCollider(Values.ExplosionRadius));
            _collider.isTrigger = true;

            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Bullet);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Explosion);
            setTag(Tags.Explosion);
        }

        public override void onAddedToScene()
        {
            var spriteAnimator = SetupExplosionAnimation();
            spriteAnimator.renderLayer = Layers.PlayerFront;
            spriteAnimator.layerDepth = 0.5f + Random.range(0.01f, 0.02f);
            addComponent(spriteAnimator);

            var lightSource = addComponent(new LightSource(Color.LightYellow, this));
            lightSource.LightSprite.tweenColorTo(Color.Black, 1f)
                .setEaseType(EaseType.ExpoIn)
                .start();

            for (int i = 0; i < 30; i++)
            {
                var randomOffsetX = Mathf.cos(Random.range((float)-Math.PI, (float)Math.PI)) * 30f;
                var randomOffsetY = Mathf.sin(Random.range((float)-Math.PI, (float)Math.PI)) * 30f;
                var position = this.position + new Vector2(randomOffsetX, randomOffsetY);
                scene.addEntity(new Smoke(position, false));
            }

            var maxFireballCount = 20;
            for (float i = 0; i < maxFireballCount; i++)
            {
                if (Random.nextFloat() < 0.5f) continue;
                var direction = new Vector2(Mathf.cos(2 * (float)Math.PI * (i / maxFireballCount)), Mathf.sin(2 * (float)Math.PI * (i / maxFireballCount)));
                scene.addEntity(new Fireball(position, direction));
            }

            spriteAnimator.play(ExplosionAnimation.Explode);
            Core.schedule(1.0f, _ => _shouldBeDestroyed = true);
        }

        public override void update()
        {
            base.update();
            if (_shouldBeDestroyed) destroy();
        }

        private Sprite<ExplosionAnimation> SetupExplosionAnimation()
        {
            var animator = new Sprite<ExplosionAnimation>();
            var explodeAnimation = new SpriteAnimationDescriptor
            {
                FPS = 12f,
                Loop = false,
                Frames = new int[]
                {
                    0,1,2,3,4,5,6,7,8,9,10,11
                },
                Origin = new Vector2(32, 32),
                TileHeight = 64,
                TileWidth = 64
            }.ToSpriteAnimation("effects/explosion");
            animator.addAnimation(ExplosionAnimation.Explode, explodeAnimation);

            return animator;
        }
    }


}