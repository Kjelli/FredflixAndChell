using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using Nez.Tweens;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public class Collectible : GameObject
    {
        private Effect _flashEffect;

        private Mover _mover;
        private Sprite _sprite;
        private Vector2 _acceleration;
        private Collider _pickupHitbox;
        private Collider _collisionHitbox;

        private bool _dropped;
        private int _numberOfPlayersInProximity;
        private bool _isHighlighted;

        public CollectibleParameters Preset { get; set; }

        public Collectible(float x, float y, string name, bool dropped) : base(x, y)
        {
            Preset = Collectibles.Get(name);
            _dropped = dropped;
            _acceleration = new Vector2();
        }

        public override void OnDespawn()
        {
            Console.WriteLine($"Despawned entity {entity}");
        }

        public override void OnSpawn()
        {
            var gunSprite = Preset.Gun.Sprite;

            _sprite = entity.addComponent(new Sprite(gunSprite.Icon.ToSpriteAnimation(gunSprite.Source).frames[0]));
            _sprite.renderLayer = Layers.Items;
            _sprite.material = new Material();

            entity.scale = new Vector2(0.025f, 0.025f);
            entity.tweenLocalScaleTo(0.5f, 0.5f)
                .setEaseType(EaseType.ExpoOut)
                .setCompletionHandler(_ => Hover(2f))
                .start();
            _mover = entity.addComponent(new Mover());

            // Delay pickup (debug)
            Core.schedule(0.5f, _ => SetupPickupHitbox());

            //Collision
            _collisionHitbox = entity.addComponent(new CircleCollider(4));
            Flags.setFlagExclusive(ref _collisionHitbox.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collisionHitbox.collidesWithLayers, Layers.Items);
        }

        private void SetupPickupHitbox()
        {
            entity.setTag(Tags.Collectible);
            _pickupHitbox = entity.addComponent(new BoxCollider());
            _pickupHitbox.isTrigger = true;
            Flags.setFlagExclusive(ref _pickupHitbox.physicsLayer, Layers.Items);
        }

        private void FallIntoPit(Entity pitEntity)
        {
            Velocity = Vector2.Zero;
            _mover.setEnabled(false);
            _collisionHitbox.setEnabled(false);
            _pickupHitbox.setEnabled(false);

            var easeType = EaseType.CubicOut;
            var durationSeconds = 2f;
            var targetScale = 0.2f;
            var targetRotationDegrees = 180;
            var targetColor = new Color(0, 0, 0, 0.25f);
            var destination = pitEntity.localPosition;

            entity.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .start();
            entity.tweenScaleTo(targetScale, durationSeconds)
                .setEaseType(easeType)
                .start();
            entity.tweenPositionTo(destination, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => entity.setEnabled(false))
                .start();
            _sprite.tweenColorTo(targetColor, durationSeconds)
                .setEaseType(easeType)
                .start();
        }

        private void Hover(float yOffset)
        {
            if (entity != null)
            {
                entity.tweenLocalPositionTo(new Vector2(entity.transform.position.X, entity.transform.position.Y + yOffset), 1f)
               .setEaseType(EaseType.SineInOut)
               .setCompletionHandler(_ => Hover(-yOffset))
               .start();
            }
        }

        public void Highlight()
        {
            _numberOfPlayersInProximity++;
            UpdateHighlightRendering();
        }

        public void Unhighlight()
        {
            _numberOfPlayersInProximity--;
            UpdateHighlightRendering();
        }

        private void UpdateHighlightRendering()
        {
            if (_numberOfPlayersInProximity > 0 && !_isHighlighted)
            {
                Console.WriteLine($"Highlighting entity {entity.name}");

                _isHighlighted = true;

                var flashEffect = Assets.AssetLoader.GetEffect("shader_flash");
                var flashTexture = Assets.AssetLoader.GetTexture("effects/lava1");

                flashEffect.Parameters["flash_texture"].SetValue(flashTexture);
                flashEffect.Parameters["flashRate"].SetValue(0f);
                flashEffect.Parameters["flashOffset"].SetValue(1f);
                flashEffect.Parameters["scrollSpeed"].SetValue(new Vector2(0.45f, 0.45f));

                _flashEffect = flashEffect;
                _sprite.material.effect = _flashEffect;
            }
            else if (_numberOfPlayersInProximity == 0 && _isHighlighted)
            {
                Console.WriteLine($"Unhighlighting entity {entity.name}");

                _isHighlighted = false;

                _sprite.material.effect = null;
            }
        }

        public override void update()
        {
            Velocity = (0.975f * Velocity + 0.025f * _acceleration);
            var isColliding = _mover.move(Velocity, out CollisionResult result);

            if (Velocity.Length() < 0.001f)
            {
                Velocity = Vector2.Zero;
            }

            if (_isHighlighted)
            {
                _flashEffect.Parameters["gameTime"].SetValue(Time.time);
            }
        }

        public void OnPickup()
        {
            Console.WriteLine($"Picked up {entity.name}");
            _pickupHitbox.setEnabled(false);
            _collisionHitbox.setEnabled(false);
            entity.destroy();
        }
    }
}
