using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.Components.Bullets;

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
            _mover = entity.addComponent(new ProjectileMover());
            _map = entity.scene.findEntity("tiled-map-entity").getComponent<TiledMapComponent>();

            if (_params.RotateWithGun)
            {
                entity.rotation = _direction;
            }

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, 0);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.MapObstacles);

            // Hack to avoid moving without collider and causing nullreference
            _collider.onAddedToEntity();
            _mover.onAddedToEntity();
        }

        public override void OnDespawn()
        {

        }

        public override void update()
        {
            Move();
        }

        private void Move()
        {
            Velocity = new Vector2(_params.Speed * (float)Math.Cos(_direction), _params.Speed * (float)Math.Sin(_direction));
            Velocity *= Time.deltaTime;
            var isColliding = _mover.move(new Vector2(Velocity.X, Velocity.Y));

            if (isColliding && _collider.collidesWithAny(out CollisionResult collision))
            {
                var pos = entity.position + new Vector2(-4) * collision.normal;

                var tile = _map.getTileAtWorldPosition(pos);
                var bulletsPassable = tile?
                    .tilesetTile?
                    .properties?
                    .ContainsKey(TileProperties.BulletPassable) ?? false;

                if (!bulletsPassable)
                {
                    entity.setEnabled(false);
                    entity.destroy();
                }
            }
        }
    }
}
