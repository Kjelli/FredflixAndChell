using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities;
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
    public class Blood : GameObject, ITriggerListener
    {
   
        private Vector2 _acceleration;
        private Mover _mover;
        private Sprite _sprite;

     
        

        public Blood(float x, float y) : base(x, y)
        {
            _acceleration = new Vector2();
          
        }

        public override void OnDespawn(){}

        public override void OnSpawn()
        {

            _sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("particles/blood")));
            _sprite.renderLayer = Layers.PlayerFrontest;
            _sprite.material = new Material();

            entity.scale = new Vector2(0.5f, 0.5f);

            _mover = entity.addComponent(new Mover());

            Core.schedule(0.5f, _ => UpdateRenderLayerDepth());

        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            throw new NotImplementedException();
        }

        public void onTriggerExit(Collider other, Collider local)
        {
            throw new NotImplementedException();
        }

        public override void update()
        {
            Move();
        }

        private void Move()
        {
            if (Velocity.Length() == 0) return;
            Velocity = (0.975f * Velocity + 0.54f * _acceleration);
            var isColliding = _mover.move(Velocity, out CollisionResult result);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
        }


        private void UpdateRenderLayerDepth()
        {
           _sprite.renderLayer = Layers.MapObstacles;
        }
    }
}
