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
    public class BloodEngine : Component
    {
        public class BloodParticle : GameObject
        {
            private Mover _mover;
            private Sprite _sprite;


            public BloodParticle(float x, float y) : base(x, y) {}

            public override void OnDespawn() { }

            public override void OnSpawn()
            {
                System.Random rng = new System.Random();

                _sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("particles/blood")));
                _sprite.renderLayer = Layers.PlayerFrontest;
                _sprite.material = new Material();
                _mover = entity.addComponent(new Mover());

                //Scale
                float random_scale = ((float)rng.Next(-20, 20) / 100);
                entity.scale = new Vector2(0.5f + random_scale, 0.5f + random_scale);

                //Rotation
                _sprite.transform.rotation = random_scale;

                Core.schedule(0.5f, _ => UpdateRenderLayerDepth());
            }

            public override void update()
            {
                if (Velocity.Length() == 0) return;
                Velocity = (0.878f * Velocity);
                var isColliding = _mover.move(Velocity, out CollisionResult result);

                if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;

            }

            private void UpdateRenderLayerDepth()
            {
                _sprite.renderLayer = Layers.MapObstacles;
            }
        }

        private int _bloodId = 0;
        private System.Random _rng;

        public BloodEngine()
        {
            _rng = new System.Random();
        }

        public void Sprinkle(int damage, Vector2 direction)
        {
            var particlesCount = Math.Floor((float)(damage));

            for (int i = 0; i < damage; i++)
            {
                var particle = entity.scene.createEntity($"Blood-Particle{++_bloodId}").addComponent(new BloodParticle(transform.position.X, transform.position.Y));

                float x = ((float)_rng.Next(-50, 50)) / 100f;
                float y = ((float)_rng.Next(-50, 50)) / 100f;
                var trueDirection = new Vector2(direction.X + (float)(direction.X * x), direction.Y + (float)(direction.Y * y));
                var speedConstant = 0.015f;

                particle.Velocity = new Vector2(
                    trueDirection.X * speedConstant,
                    trueDirection.Y * speedConstant);
            }
        }

    }
}
