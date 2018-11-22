using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using static FredflixAndChell.Shared.Assets.Constants;
using rng = Nez.Random;

namespace FredflixAndChell.Shared.Components.Effects.Weather
{
    public class Snowstorm : SceneComponent, IAtmosphere
    {
        private bool _firstFrame = true;

        private int _flakeId = 0;

        private Texture2D _snowTexture;
        private Camera _camera;

        public Snowstorm()
        {
            _snowTexture = new Texture2D(Core.graphicsDevice, 1, 1);
            _snowTexture.SetData(new[] { Color.LightBlue });
        }
        private void Initialize()
        {
            _camera = scene.camera;
            SpawnInitialFlakes();
        }

        public void SpawnInitialFlakes()
        {
            for (var i = 0; i < 60; i++)
            {
                var y = _camera.bounds.top + rng.nextFloat(_camera.bounds.height);
                var x = _camera.bounds.left + rng.nextFloat(_camera.bounds.width);
                var snowflake = scene.createEntity($"Snowyboii{++_flakeId}");
                snowflake.addComponent(new Snowflake(_snowTexture, x, y));
            }
        }

        public void SpawnFlake()
        {
            var y = _camera.bounds.bottom - rng.nextFloat(_camera.bounds.height);
            var x = _camera.bounds.right;
            var snowflake = scene.createEntity($"Snowyboii{++_flakeId}");
            snowflake.addComponent(new Snowflake(_snowTexture, x, y));
        }

        public override void update()
        {
            if (_firstFrame)
            {
                _firstFrame = false;
                Initialize();
                return;
            }
            if (Time.frameCount % 4 == 0)
            {
                SpawnFlake();
            }
        }

        public class Snowflake : Component, IUpdatable
        {

            private Vector2 _spawnPosition;
            private Texture2D _snowTexture;
            private Sprite _sprite;

            private float _speed;
            private float _yOffset;
            private float _size;

            public Snowflake(Texture2D snowTexture, float x, float y)
            {
                _spawnPosition = new Vector2(x, y);
                _snowTexture = snowTexture;
            }

            public override void onAddedToEntity()
            {
                entity.position = _spawnPosition;
                _sprite = entity.addComponent(new Sprite(_snowTexture));
                _sprite.renderLayer = Layers.Weather;

                _size = rng.range(5, 15) * 0.1f;
                entity.scale = new Vector2(_size, _size);

                _speed = rng.range(20, 40) * 0.1f; ;
                _yOffset = (float)(rng.range(1, 5) * 0.01);
            }

            public void update()
            {
                float floatyboii_Y = (float)(Math.Sin(entity.localPosition.X * _yOffset)) + entity.localPosition.Y;

                entity.localPosition = new Vector2(entity.localPosition.X - _speed, floatyboii_Y);

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
