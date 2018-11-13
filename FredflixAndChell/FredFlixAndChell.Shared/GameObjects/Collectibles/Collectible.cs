using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using Nez.Tweens;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Assets;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public enum CollectibleState
    {
        Appearing, Available, Unavailable
    }
    public class Collectible : GameObject, ITriggerListener
    {
        private CollectibleState _collectibleState;

        private ITween<Vector2> _hoverTween;

        private Effect _flashEffect;
        private Mover _mover;
        private Sprite _sprite;
        private Vector2 _acceleration;
        private Collider _pickupHitbox;
        private Collider _collisionHitbox;

        private bool _dropped;
        private bool _isHighlighted;
        private int _numberOfPlayersInProximity;

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
            _hoverTween = entity.tweenLocalScaleTo(0.5f, 0.5f)
                .setEaseType(EaseType.ExpoOut)
                .setCompletionHandler(_ => Hover(2f));
            _hoverTween.start();

            _mover = entity.addComponent(new Mover());

            // Delay pickup (debug)
            Core.schedule(0.5f, _ => SetupPickupHitbox());

            //Collision
            _collisionHitbox = entity.addComponent(new CircleCollider(4));
            Flags.setFlagExclusive(ref _collisionHitbox.collidesWithLayers, Layers.MapObstacles);
        }

        private void SetupPickupHitbox()
        {
            if (_collectibleState == CollectibleState.Unavailable) return;

            _collectibleState = CollectibleState.Available;
            entity.setTag(Tags.Collectible);
            _pickupHitbox = entity.addComponent(new BoxCollider());
            _pickupHitbox.isTrigger = true;
            Flags.setFlagExclusive(ref _pickupHitbox.physicsLayer, Layers.Items);
        }

        private void FallIntoPit(Entity pitEntity)
        {
            Velocity = Vector2.Zero;
            _acceleration = Vector2.Zero;
            _collectibleState = CollectibleState.Unavailable;

            UpdateHighlightRendering();

            _mover.setEnabled(false);
            _collisionHitbox.setEnabled(false);

            if (_pickupHitbox != null)
            {
                _pickupHitbox.setEnabled(false);
                _pickupHitbox.collidesWithLayers = 0;
                _pickupHitbox.physicsLayer = 0;
            }

            var easeType = EaseType.CubicOut;
            var durationSeconds = 1.25f;
            var targetScale = 0.2f;
            var targetRotationDegrees = 180;
            var targetColor = new Color(0, 0, 0, 0.25f);
            var destination = pitEntity.localPosition;

            _hoverTween?.stop(true);

            entity.tweenPositionTo(destination, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => entity.setEnabled(false))
                .start();
            entity.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .start();
            entity.tweenScaleTo(targetScale, durationSeconds)
                .setEaseType(easeType)
                .start();
            _sprite.tweenColorTo(targetColor, durationSeconds)
                .setEaseType(easeType)
                .start();
        }

        private void Hover(float yOffset)
        {
            if (entity == null 
                || entity.transform == null
                || !entity.enabled 
                || _collectibleState != CollectibleState.Available) return;
            _hoverTween?.stop(true);
            _hoverTween = entity.tweenLocalPositionTo(new Vector2(entity.transform.position.X, entity.transform.position.Y + yOffset), 1f)
           .setEaseType(EaseType.SineInOut)
           .setCompletionHandler(_ => Hover(-yOffset));
            _hoverTween.start();
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
            if (CanBeCollected() && _numberOfPlayersInProximity > 0 && !_isHighlighted)
            {
                _isHighlighted = true;

                var flashEffect = Assets.AssetLoader.GetEffect("shader_flash");
                var flashTexture = Assets.AssetLoader.GetTexture("effects/lava2");

                flashEffect.Parameters["flash_texture"].SetValue(flashTexture);
                flashEffect.Parameters["flashRate"].SetValue(0f);
                flashEffect.Parameters["flashOffset"].SetValue(1f);
                flashEffect.Parameters["scrollSpeed"].SetValue(new Vector2(0.45f, 0.45f));

                _flashEffect = flashEffect;
                _sprite.material.effect = _flashEffect;
            }
            else if (!CanBeCollected() || (_numberOfPlayersInProximity == 0 && _isHighlighted))
            {
                Console.WriteLine($"Unhighlighting entity {entity.name}");

                _isHighlighted = false;

                _sprite.material.effect = null;
            }
        }

        public override void update()
        {
            if (_isHighlighted)
            {
                _flashEffect.Parameters["gameTime"].SetValue(Time.time);
            }

            if (_collectibleState == CollectibleState.Unavailable) return;

            Move();
        }

        private void Move()
        {
            if (Velocity.Length() == 0) return;
            Velocity = (0.975f * Velocity + 0.025f * _acceleration);
            var isColliding = _mover.move(Velocity, out CollisionResult result);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
            if (Velocity.Length() > 0) UpdateRenderLayerDepth();
        }

        private void UpdateRenderLayerDepth()
        {
            _sprite.layerDepth = 1 - (entity.position.Y) * Constants.RenderLayerDepthFactor;
        }

        public bool CanBeCollected()
        {
            return _collectibleState == CollectibleState.Available;
        }

        public void OnPickup()
        {
            if (_collectibleState != CollectibleState.Available) return;

            _collectibleState = CollectibleState.Unavailable;
            _pickupHitbox.setEnabled(false);
            _collisionHitbox.setEnabled(false);
            entity.destroy();
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            if (other == null || other.entity == null) return;
            if (_collectibleState == CollectibleState.Appearing) return;

            if (other.entity.tag == Tags.Pit)
            {
                FallIntoPit(other.entity);
            }
        }

        public void onTriggerExit(Collider other, Collider local)
        {
        }
    }
}
