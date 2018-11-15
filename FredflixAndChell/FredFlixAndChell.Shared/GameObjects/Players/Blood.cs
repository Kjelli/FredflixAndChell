using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public class Blood : GameObject
    {
        private Vector2 _direction;
        private Vector2 _acceleration;

        private Mover _mover;

        public Blood(float x, float y, Vector2 direction) : base(x, y)
        {
            _direction = direction;

            _acceleration = Vector2.Zero;
        }

        public override void OnDespawn()
        {
           
        }

        public override void OnSpawn()
        {
            var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("particles/blood")));
            sprite.renderLayer = Layers.MapObstacles;
            _mover = entity.addComponent(new Mover());
            Velocity += _direction * Time.deltaTime * 50f;
        }

        public override void update()
        {
            var deltaTime = Time.deltaTime;
            _acceleration *= 50f * deltaTime;
            Velocity = (0.95f * Velocity * _acceleration);
        }
    }
}
