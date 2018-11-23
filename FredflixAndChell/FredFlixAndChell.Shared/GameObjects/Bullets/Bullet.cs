using System;
using Microsoft.Xna.Framework;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.Components.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Components.Bullets.Behaviours;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{

    public class Bullet : GameObject
    {
        private const string NamespacePrefix = "FredflixAndChell.Shared.Components.Bullets.Behaviours.";

        private BulletParameters _params;
        private BulletRenderer _renderer;
        private BulletBehaviour _behaviour;
        private Player _owner;

        private float _direction;

        public Player Owner => _owner;
        public BulletParameters Parameters => _params;

        private Bullet(Player owner, float x, float y, float direction, BulletParameters bulletParameters) : base((int)x, (int)y)
        {
            _owner = owner;
            _params = bulletParameters;
            _direction = direction;

            var behaviourType = Type.GetType(NamespacePrefix + _params.BulletBehaviour);
            _behaviour = (BulletBehaviour)Activator.CreateInstance(behaviourType, this);
        }

        public override void OnSpawn()
        {
            SetupComponents();

            Velocity = new Vector2(_params.Speed * (float)Math.Cos(_direction), _params.Speed * (float)Math.Sin(_direction));

            if (_params.LifeSpanSeconds > 0)
            {
                Core.schedule(_params.LifeSpanSeconds, _ => entity?.destroy());
            }

            if (_params.RotateWithGun)
            {
                entity.rotation = _direction;
            }

            entity.setScale(_params.Scale);
        }

        private void SetupComponents()
        {
            entity.addComponent(_behaviour);
            _behaviour.OnFired();
            _renderer = entity.addComponent(new BulletRenderer(_params.Sprite));
        }

        public override void OnDespawn()
        {
            entity.setEnabled(false);
            entity.destroy();
        }

        public override void update()
        {
            _renderer.UpdateRenderLayerDepth();
        }

        public static void Create(Player owner, float x, float y, float direction, BulletParameters parameters)
        {
            var bulletEntity = Core.scene.createEntity("bullet");
            bulletEntity.addComponent(new Bullet(owner, x, y, direction, parameters));
        }
    }
}
