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

namespace FredflixAndChell.Shared.GameObjects.Bullets
{

    public class Bullet : GameObject, IBullet
    {
        private Texture2D _texture;
        private ProjectileMover _mover;
        private CircleCollider _collider;

        public float Damage { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }

        public Bullet(Player origin, float x, float y, int width, int height, float direction, float speed, float damage) : base((int)x, (int)y, width, height)
        {
            Damage = damage;
            Direction = direction;
            Speed = speed;

            _texture = AssetLoader.GetTexture("standard_bullet");

        }

        public override void OnDespawn()
        {

        }

        public override void OnSpawn()
        {
            var sprite = entity.addComponent(new Sprite(_texture));
            sprite.renderLayer = Layers.PlayerBehind;

            var shadow = entity.addComponent(new SpriteMime(sprite));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);

            _collider = entity.addComponent<CircleCollider>();
            _mover = entity.addComponent(new ProjectileMover());

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, 0);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.MapObstacles);

            // Hack to avoid moving without collider and causing nullreference
            _collider.onAddedToEntity();
            _mover.onAddedToEntity();

            entity.setScale(0.25f);
        }

        public override void update()
        {
            Move();
        }

        private void Move()
        {
            Velocity = new Vector2(Speed * (float)Math.Cos(Direction), Speed * (float)Math.Sin(Direction));
            Velocity *= Time.deltaTime;
            var isColliding = _mover.move(new Vector2(Velocity.X, Velocity.Y));
            
            if (isColliding)
            {
                entity.setEnabled(false);
                entity.destroy();
            }
        }
    }
}
