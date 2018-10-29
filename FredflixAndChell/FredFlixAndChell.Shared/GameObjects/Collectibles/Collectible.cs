using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public class Collectible : GameObject
    {
        public CollectibleParameters _preset;

        private Sprite Sprite;

        private float _scaleValue = 0.0f;
        private float _maxScaleValue = 0.5f;

        public Vector2 Acceleration;
        private Mover _mover;

        private Vector2 _origin;

        private bool _resetOrigin;
        private bool _dropped;

        public Collectible(int x, int y, string objectName, bool dropped) : base(x, y)
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

            _origin = new Vector2(x, y);
            _resetOrigin = false;

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

            transform.setScale(_dropped ? _maxScaleValue : 0.0f);
            _mover = entity.addComponent(new Mover());

        }


        public void PushDirection(float power, float direction)
        {
            _resetOrigin = true;
            Acceleration = new Vector2((float)Math.Cos(direction) * power, (float)Math.Sin(direction) * power);
        }

        public override void update()
        {

            if(_scaleValue < _maxScaleValue)
            {
                //Spawn scaling
                _scaleValue += 0.025f;
                transform.setScale(_scaleValue);
            }
            else
            {
                if(Velocity == Vector2.Zero)
                {
                    if (_resetOrigin)
                    {
                        _origin = new Vector2(transform.position.X, transform.position.Y);
                        _resetOrigin = false;
                    }

                    //Bouncy boii
                    transform.position = new Vector2(_origin.X, _origin.Y + (float)Math.Sin(Time.time * 5.0f)); 
                }
            }

            Acceleration *= Time.deltaTime;
            Velocity = (0.95f * Velocity + 0.05f * Acceleration);
            var isColliding = _mover.move(Velocity, out CollisionResult result);
            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;

        }
    }
}
