using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;
using rng = Nez.Random;



namespace FredflixAndChell.Shared.GameObjects.Players
{
    public class BloodEngine : Component, IUpdatable
    {
        private System.Random _rng;
        private Cooldown _leakInterval;
        private Texture2D _bloodTexture;

        private int _bloodId = 0;
        private bool _leak = false;
        private int _particlesPrLeakage;

        public BloodEngine()
        {
            _rng = new System.Random();
            _leakInterval = new Cooldown(1f);
            _bloodTexture = new Texture2D(Core.graphicsDevice, 1, 1);
            _bloodTexture.SetData(new[] { Color.DarkRed });
        }

        public void Sprinkle(int damage, Vector2 direction)
        {
            var particlesCount = Math.Floor((float)(damage));

            for (int i = 0; i < damage; i++)
            {
                var particle = entity.scene.addEntity(new BloodParticle(_bloodTexture, transform.position.X, transform.position.Y));

                float x = ((float)_rng.Next(-50, 50)) / 100f;
                float y = ((float)_rng.Next(-50, 50)) / 100f;
                var trueDirection = new Vector2(direction.X + (float)(direction.X * x), direction.Y + (float)(direction.Y * y));
                var speedConstant = 0.015f;

                particle.Velocity = new Vector2(
                    trueDirection.X * speedConstant,
                    trueDirection.Y * speedConstant);
            }
        }


        public void Blast(int particles = 100, float power = 4.0f)
        {
            for(int i = 0; i < particles; i++)
            {
                var particle = entity.scene.addEntity(new BloodParticle(_bloodTexture, transform.position.X, transform.position.Y, false));
                particle.Velocity = new Vector2(rng.minusOneToOne()* power, rng.minusOneToOne()* power);
            }
        }

        public void Leak(int particlesPrLeakage = 20, float duration = 10f)
        {
            _leak = true;
            _particlesPrLeakage = particlesPrLeakage;
            _leakInterval.Start();
            Core.schedule(duration, _ => _leak = false);
        }

        public void update()
        {
            _leakInterval.Update();
            if (_leak && _leakInterval.IsReady())
            {
                Blast(_particlesPrLeakage, 2.5f);
                _leakInterval.Start();
            }
        }

        public class BloodParticle : GameObject
        {
            private Mover _mover;
            private Texture2D _bloodTexture;
            private Sprite _sprite;

            private bool _drawAbovePlayer;

            public BloodParticle(Texture2D bloodTexture, float x, float y, bool drawAbovePlayer = true) : base(x, y)
            {
                _drawAbovePlayer = drawAbovePlayer;
                _bloodTexture = bloodTexture;
            }

            public override void OnDespawn() { }

            public override void OnSpawn()
            {
                //_sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("particles/blood")));
                _sprite = addComponent(new Sprite(_bloodTexture));
                _sprite.renderLayer = _drawAbovePlayer ? Layers.PlayerFrontest : Layers.MapObstacles;
                _sprite.material = new Material();
                _mover = addComponent(new Mover());

                var bloodHitbox = addComponent(new CircleCollider(0.1f));
                Flags.setFlagExclusive(ref bloodHitbox.collidesWithLayers, Layers.MapObstacles);
                bloodHitbox.physicsLayer = 0;
                //Scale

                //float random_scale = ((float)rng.range(-20, 20) / 100);
                float random_scale = ((float)rng.range(-20, 20) / 100);
                scale = new Vector2(1f + random_scale, 1f + random_scale);

                //Rotation
                _sprite.transform.rotation = rng.nextAngle();

                if (_drawAbovePlayer) Core.schedule(0.5f, _ => UpdateRenderLayerDepth());
            }

            public override void Update()
            {
                if (Velocity.Length() == 0) return;
                Velocity = (0.878f * Velocity);
                var isColliding = _mover.move(Velocity, out CollisionResult result);
                if (isColliding && result.collider?.entity?.tag == Tags.Pit) destroy();

                if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
            }

            private void UpdateRenderLayerDepth()
            {
                _sprite.renderLayer = Layers.MapObstacles;
            }
        }

    }
}
