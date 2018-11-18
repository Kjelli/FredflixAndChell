using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Pipeline.Content;
using Nez.Sprites;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;
using rng = Nez.Random;

namespace FredflixAndChell.Shared.Components.Effects
{
    public class Snowstorm : SceneComponent, IUpdatable
    {
        private int _xSpawn;
        private int _yMaxSpawn;
        private int _flakeId = 0;


        public Snowstorm(int width, int height)
        {
            _xSpawn = width;
            _yMaxSpawn = height;
        }

        public override void onEnabled()
        {
            base.onEnabled();
            Core.schedule(0.05f, true, onTime : SpawnFlake);
        }

        public void SpawnFlake(ITimer timer = null)
        {
            var y = rng.nextInt(_yMaxSpawn);
            scene.createEntity($"Snowyboii{++_flakeId}").addComponent(new Snowflake(_xSpawn, y));
        }

        public class Snowflake : GameObject{

            private Texture2D _pixel;
            private Sprite _sprite;

            private float _speed;
            private float _yOffset;
            private float _size;

            public Snowflake(float x, float y) : base(x, y)
            {
            }

            public override void OnDespawn(){}

            public override void OnSpawn(){
                _pixel = new Texture2D(Core.graphicsDevice, 1, 1);

                _pixel.SetData(new[] { GetRandomSnowyColor() });

                _sprite = entity.addComponent(new Sprite(_pixel));
                _sprite.renderLayer = Layers.Weather;

                _size = rng.range(5, 15) * 0.1f;
                entity.scale = new Vector2(_size, _size);

                _speed = rng.range(20, 40) * 0.1f; ;
                _yOffset = (float)(rng.range(1, 5) * 0.01);
            }

            public override void update()
            {
                float floatyboii_Y  = (float)(Math.Sin(entity.localPosition.X * _yOffset)) + entity.localPosition.Y;
                
                entity.localPosition = new Vector2(entity.localPosition.X - _speed , floatyboii_Y);
             
                if (entity.position.X < 0)
                    entity.destroy();
            }

            public Color GetRandomSnowyColor()
            {
                switch (rng.nextInt(3))
                {
                    case 2:
                        return Color.LightBlue;
                    case 1:
                        return Color.White;
                    default:
                        return Color.Snow;
                }
                 
            }

           

        }
    }
}
