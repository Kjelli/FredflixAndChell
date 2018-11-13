using System;
using Microsoft.Xna.Framework;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.Components.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{

    public class Bullet : GameObject
    {
        private BulletParameters _params;
        private BulletRenderer _renderer;
        private ProjectileMover _mover;
        private CircleCollider _collider;
        private TiledMapComponent _map;
        private Player _owner;

        private float _direction;

        public Bullet(Player owner, float x, float y, float direction, BulletParameters bulletParameters) : base((int)x, (int)y)
        {
            _owner = owner;
            _params = bulletParameters;
            _direction = direction;
        }

        public override void OnSpawn()
        {
            _renderer = entity.addComponent(new BulletRenderer(_params.Sprite));
            _collider = entity.addComponent<CircleCollider>();
            _collider.radius = 2f;

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Bullet);

            _mover = entity.addComponent(new ProjectileMover());
            _map = entity.scene.findEntity("tiled-map-entity").getComponent<TiledMapComponent>();

            Velocity = new Vector2(_params.Speed * (float)Math.Cos(_direction), _params.Speed * (float)Math.Sin(_direction));

            if(_params.LifeSpanSeconds > 0)
            {
                Core.schedule(_params.LifeSpanSeconds, _ => entity?.destroy());
            }

            if (_params.RotateWithGun)
            {
                entity.rotation = _direction;
            }

            // Hack to avoid moving without collider and causing nullreference
            _collider.onAddedToEntity();
            _mover.onAddedToEntity();

            entity.setScale(_params.Scale);
        }

        public override void OnDespawn()
        {
            _collider.unregisterColliderWithPhysicsSystem();
            entity.setEnabled(false);
            entity.destroy();
        }

        public override void update()
        {
            Move();
        }

        private void Move()
        {
            var isColliding = _mover.move(Velocity * Time.deltaTime);

            if (Velocity.Length() > 0) _renderer.UpdateRenderLayerDepth();

            if (isColliding && _collider.collidesWithAny(out CollisionResult collision))
            {
                var collidedWithEntity = collision.collider.entity;
                if (Flags.isFlagSet(collidedWithEntity.tag, Tags.Player))
                {
                    var player = collidedWithEntity.getComponent<Player>();
                    if (player != null && player != _owner)
                    {
                        player.Damage((int)_params.Damage);
                        player.Velocity += Velocity * _params.Knockback * Time.deltaTime;
                        entity.destroy();
                    }
                    else if (player == _owner)
                    {
                        return;
                    }
                }
                else if (_params.Bounce)
                {
                    var distance = (collision.point - _collider.absolutePosition);
                    _mover.move(-collision.minimumTranslationVector);

                    var tangentVector = new Vector2(-distance.Y, distance.X);
                    Vector2.Normalize(ref tangentVector, out tangentVector);
                    var length = Vector2.Dot(Velocity, tangentVector);
                    Vector2 velocityComponentOnTangent;
                    Vector2.Multiply(ref tangentVector, length, out velocityComponentOnTangent);
                    Vector2 velocityComponentPerpendicularToTangent = Velocity - velocityComponentOnTangent;
                    Velocity = new Vector2(Velocity.X - 2 * velocityComponentPerpendicularToTangent.X,
                        Velocity.Y - 2 * velocityComponentPerpendicularToTangent.Y);
                }
                else
                {
                    entity.destroy();
                }
            }
        }
    }
}
