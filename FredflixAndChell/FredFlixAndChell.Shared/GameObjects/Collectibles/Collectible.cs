using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using Nez.Tweens;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public class Collectible : GameObject
    {
        private CollectibleParameters _preset;
        private Mover _mover;
        private Sprite Sprite;
        private Vector2 Acceleration;

        private Collider _collider;

        private bool _dropped;

        public Collectible(float x, float y, string objectName, bool dropped) : base(x, y)
        {
            switch (objectName)
            {
                case "M4":
                    _preset = CollectiblePreset.M4;
                    break;
                case "Fido":
                    _preset = CollectiblePreset.Fido;
                    break;
                default:
                    _preset = null;
                    Console.WriteLine("Object name not found when trying to drop. Check player -> dropgun()");
                    break;
            }

            _dropped = dropped;
            Acceleration = new Vector2();

        }

        public override void OnDespawn()
        {

        }

        public override void OnSpawn()
        {
            Sprite = entity.addComponent(new Sprite(_preset.Gun.Sprite.Icon.ToSpriteAnimation(_preset.Gun.Sprite.Source).frames[0]));
            Sprite.renderLayer = Layers.Items;
            entity.scale = new Vector2(0.025f, 0.025f);

           _collider = entity.addComponent<CircleCollider>();
            // Setter at kun fysikken skjer fra layr mapforeground og beyond
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.MapForeground);
            // Setter an kollisjons listner med player
            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.Player);



            entity.tweenLocalScaleTo(0.5f, 1f)
                .setEaseType(EaseType.Linear)
                .setCompletionHandler(_ => Hover(2f))
                .start();

            //_collider.onAddedToEntity();
            _mover = entity.addComponent(new Mover());
        }

        private void Hover(float yOffset)
        {
            entity.tweenLocalPositionTo(new Vector2(entity.transform.position.X, entity.transform.position.Y + yOffset), 1f)
                .setEaseType(EaseType.SineInOut)
                .setCompletionHandler(_ => Hover(-yOffset))
                .start();
        }

        public void PushDirection(float power, float direction)
        {
            Acceleration = new Vector2((float)Math.Cos(direction) * power, (float)Math.Sin(direction) * power);
        }

        public override void update()
        {
            Acceleration *= Time.deltaTime;
            Velocity = (0.95f * Velocity + 0.05f * Acceleration);
            var isColliding = _mover.move(Velocity, out CollisionResult result);
            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
        }
    }
}
