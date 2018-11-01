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
        public CollectibleParameters Preset;
        private Mover _mover;
        private Sprite _sprite;
        private Vector2 _acceleration;

        private bool _dropped;

        public Collectible(float x, float y, string objectName, bool dropped) : base(x, y)
        {
            switch (objectName)
            {
                case "M4":
                    Preset = CollectiblePresets.M4;
                    break;
                case "Fido":
                    Preset = CollectiblePresets.Fido;
                    break;
                case "PewPew":
                    Preset = CollectiblePresets.PewPew;
                    break;
                case "PewPew":
                    _preset = CollectiblePresets.PewPew;
                    break;
                default:
                    Preset = null;
                    Console.WriteLine("Object name not found when trying to drop. Check player -> dropgun()");
                    break;
            }

            _dropped = dropped;
            _acceleration = new Vector2();
        }

        public override void OnDespawn()
        {

        }

        public override void OnSpawn()
        {
            _sprite = entity.addComponent(new Sprite(Preset.Gun.Sprite.Icon.ToSpriteAnimation(Preset.Gun.Sprite.Source).frames[0]));
            _sprite.renderLayer = Layers.Items;
            entity.scale = new Vector2(0.025f, 0.025f);
            entity.tweenLocalScaleTo(0.5f, 0.5f)
                .setEaseType(EaseType.ExpoOut)
                .setCompletionHandler(_ => Hover(2f))
                .start();
            _mover = entity.addComponent(new Mover());

            entity.setTag(Tags.Collectible);
            //Collision
            var hitbox = this.addComponent(new BoxCollider());
            hitbox.isTrigger = true;
            Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);



        }

        private void Hover(float yOffset)
        {
            entity.tweenLocalPositionTo(new Vector2(entity.transform.position.X, entity.transform.position.Y + yOffset), 1f)
                .setEaseType(EaseType.SineInOut)
                .setCompletionHandler(_ => Hover(-yOffset))
                .start();
        }

        public override void update()
        {
            Velocity = (0.975f * Velocity + 0.025f * _acceleration);
            var isColliding = _mover.move(Velocity, out CollisionResult result);
            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
        }

        
    }
}
