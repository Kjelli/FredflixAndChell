using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Effects;
using FredflixAndChell.Shared.Components.Interactables;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using System;
using System.Linq;
using static FredflixAndChell.Shared.Assets.Constants;

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
        private CollectibleCollisionHandler _collisionHandler;

        private bool _dropped;
        private bool _isHighlighted;
        private int _numberOfPlayersInProximity;

        public CollectibleState CollectibleState => _collectibleState;
        public CollectibleParameters Preset { get; set; }
        public CollectibleMetadata Metadata { get; set; }

        public Collectible(float x, float y, string name, bool dropped, CollectibleMetadata Metadata = null) : base(x, y)
        {
            Preset = CollectibleDict.Get(name);
            _dropped = dropped;
            _acceleration = new Vector2();
            this.Metadata = Metadata;
        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {
            SetupComponents();
            if(Preset.Name == "Flag")
            {
                addComponent(new LightSource(Metadata.Color, this));
            }
        }

        private void SetupComponents()
        {
            _collisionHandler = addComponent(new CollectibleCollisionHandler());

            WeaponSprite sprite = null;
            if (Preset.Weapon is GunParameters gunParams)
            {
                sprite = gunParams.Sprite;
            }
            else if (Preset.Weapon is MeleeParameters meleeParams)
            {
                sprite = meleeParams.Sprite;
            }

            _sprite = addComponent(new Sprite(sprite.Icon.ToSpriteAnimation(sprite.Source).frames[0]));
            _sprite.renderLayer = Layers.Interactables;
            _sprite.material = new Material();
            _sprite.color = Metadata?.Color ?? Color.White;

            scale = new Vector2(0.5f, 0.5f);
            _hoverTween = this.tweenLocalScaleTo(0.5f, 0.5f)
                .setEaseType(EaseType.ExpoOut)
                .setCompletionHandler(_ => Hover(2f));
            _hoverTween.start();

            _mover = addComponent(new Mover());

            // Delay pickup (debug)
            Core.schedule(0.5f, _ => SetupPickupHitbox());

            //Collision
            _collisionHitbox = addComponent(new CircleCollider(4));
            Flags.setFlagExclusive(ref _collisionHitbox.collidesWithLayers, Layers.MapObstacles);
        }

        private void SetupPickupHitbox()
        {
            setTag(Tags.Collectible);

            if (_collectibleState == CollectibleState.Unavailable) return;

            _collectibleState = CollectibleState.Available;
            _pickupHitbox = addComponent(new BoxCollider());
            _pickupHitbox.isTrigger = true;
            Flags.setFlagExclusive(ref _pickupHitbox.physicsLayer, Layers.Interactables);

            var interactable = addComponent(new InteractableComponent
            {
                OnInteract = player => OnPickup(player)
            });
        }

        public void FallIntoPit(Entity pitEntity)
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

            this.tweenPositionTo(destination, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => this.setEnabled(false))
                .start();
            this.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .start();
            this.tweenScaleTo(targetScale, durationSeconds)
                .setEaseType(easeType)
                .start();
            _sprite.tweenColorTo(targetColor, durationSeconds)
                .setEaseType(easeType)
                .start();

            Metadata?.OnDestroyEvent?.Invoke(this);
        }

        private void Hover(float yOffset)
        {
            if (transform == null
                || !enabled
                || _collectibleState != CollectibleState.Available) return;
            _hoverTween?.stop(true);
            _hoverTween = this.tweenLocalPositionTo(new Vector2(transform.position.X, transform.position.Y + yOffset), 1f)
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
            if (CollectibleState == CollectibleState.Available && _numberOfPlayersInProximity > 0 && !_isHighlighted)
            {
                _isHighlighted = true;

                var flashEffect = Assets.AssetLoader.GetEffect("shader_flash");
                var flashTexture = Assets.AssetLoader.GetTexture("effects/lava2");

                flashEffect.Parameters["flash_texture"].SetValue(flashTexture);
                flashEffect.Parameters["flashRate"].SetValue(0f);
                flashEffect.Parameters["flashOffset"].SetValue(1f);
                flashEffect.Parameters["scrollSpeed"].SetValue(new Vector2(0.45f, 0.45f));
                flashEffect.Parameters["replace_color"].SetValue(Metadata.Color.ToVector4());

                _flashEffect = flashEffect;
                _sprite.material.effect = _flashEffect;
            }
            else if (CollectibleState != CollectibleState.Available || (_numberOfPlayersInProximity == 0 && _isHighlighted))
            {
                _isHighlighted = false;

                _sprite.material.effect = null;
            }
        }

        public override void Update()
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
            _sprite.layerDepth = 1 - (position.Y) * Constants.RenderLayerDepthFactor;
        }

        public bool CanBeCollectedByPlayer(Player p)
        {
            if(Metadata?.CanCollectRules.Any(r => r.Invoke(p) == false) == true)
            {
                return false;
            }
            return _collectibleState == CollectibleState.Available;
        }

        public void OnPickup(Player player)
        {
            if (_collectibleState != CollectibleState.Available) return;
            if (!CanBeCollectedByPlayer(player)) return;

            player.EquipWeapon(Preset.Weapon, Metadata);

            _collectibleState = CollectibleState.Unavailable;
            _pickupHitbox.setEnabled(false);
            _collisionHitbox.setEnabled(false);

            Metadata?.OnPickupEvent?.Invoke(this, player);

            destroy();
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
