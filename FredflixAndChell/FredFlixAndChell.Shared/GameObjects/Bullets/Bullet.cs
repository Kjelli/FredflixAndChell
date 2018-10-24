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
        public float Damage { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }

        private Texture2D _texture;
        private Mover _mover;

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
            sprite.renderLayer = -2;

            var shadow = entity.addComponent(new SpriteMime(sprite));
            shadow.color = new Color(0, 0, 0, 255);
            shadow.material = Material.defaultMaterial;
            shadow.renderLayer = Layers.PlayerBehind;
            shadow.localOffset = new Vector2(1, 2);

            _mover = entity.addComponent(new Mover());
            entity.setScale(0.25f);
        }

        public override void update()
        {
            Velocity = new Vector2(Speed * (float)Math.Cos(Direction), Speed * (float)Math.Sin(Direction));
            _mover.move(new Vector2(Velocity.X * Time.deltaTime, Velocity.Y * Time.deltaTime), out CollisionResult result);
        }
    }
}
