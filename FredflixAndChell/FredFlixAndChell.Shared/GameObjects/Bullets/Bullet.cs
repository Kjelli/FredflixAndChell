using FredflixAndChell.Shared.Components.Bullets;
using FredflixAndChell.Shared.Components.Bullets.Behaviours;
using FredflixAndChell.Shared.Components.Effects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Weapons.Parameters;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Reflection;
using static FredflixAndChell.Shared.Utilities.Activator.ObjectActivatorHelper;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{

    public class Bullet : GameObject
    {
        private const string NamespacePrefix = "FredflixAndChell.Shared.Components.Bullets.Behaviours.";

        private static Dictionary<string, ObjectActivator<BulletBehaviour>> _behaviourCache
            = new Dictionary<string, ObjectActivator<BulletBehaviour>>();

        private BulletParameters _params;
        private BulletRenderer _renderer;
        private BulletBehaviour _behaviour;
        private LightSource _lightSource;
        private Player _owner;

        private float _direction;
        private bool _destroyed;

        public Player Owner => _owner;
        public BulletParameters Parameters => _params;
        public float Direction => _direction;
        public BulletRenderer Renderer => _renderer;

        private Bullet(Player owner, float x, float y, float direction, BulletParameters bulletParameters) : base((int)x, (int)y)
        {
            updateOrder = 1;

            _owner = owner;
            _params = bulletParameters;
            _direction = direction;

            _behaviour = addComponent(ConstructBehaviour(_params.BulletBehaviour));
            _renderer = addComponent(new BulletRenderer(this));

            Velocity = new Vector2(_params.Speed * (float)Math.Cos(_direction), _params.Speed * (float)Math.Sin(_direction));
        }

        private BulletBehaviour ConstructBehaviour(string bulletBehaviour)
        {
            if (!_behaviourCache.ContainsKey(bulletBehaviour))
            {
                var behaviourType = Type.GetType(NamespacePrefix + _params.BulletBehaviour);
                ConstructorInfo ctor = behaviourType.GetConstructors()[0];
                ObjectActivator<BulletBehaviour> activator = GetActivator<BulletBehaviour>(ctor);
                _behaviourCache.Add(bulletBehaviour, activator);
            }
            var instance = _behaviourCache[bulletBehaviour](this);
            return instance;
        }

        public override void OnSpawn()
        {
            if (Parameters.BulletType == BulletType.Entity)
            {
                _lightSource = addComponent(new LightSource(_params.BulletColor, this));
            }
            if (_params.LifeSpanSeconds >= 0)
            {
                Core.schedule(_params.LifeSpanSeconds, _ =>
                {
                    if (!_destroyed) destroy();
                });
            }

            if (_params.RotateWithGun)
            {
                rotation = _direction;
            }

            this.setScale(_params.Scale);
        }

        public override void OnDespawn()
        {
            _destroyed = true;
        }

        public override void Update()
        {
            _renderer.UpdateRenderLayerDepth();
        }

        public static Bullet Create(Player owner, float x, float y, float direction, BulletParameters parameters)
        {
            return Core.scene.addEntity(new Bullet(owner, x, y, direction, parameters));
        }

        public DirectionalDamage ToDirectionalDamage()
        {
            var directionalDamage = new DirectionalDamage
            {
                Damage = Parameters.Damage,
                Knockback = Parameters.Knockback,
                Direction = Velocity,
                SourceOfDamage = Owner
            };
            return directionalDamage;
        }
    }
}
