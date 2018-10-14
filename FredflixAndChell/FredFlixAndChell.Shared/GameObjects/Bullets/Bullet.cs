using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{

    public class Bullet : GameObject, IBullet
    {
        public float Damage { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }

        private Texture2D _sprite;

        public Bullet(Player origin, float x, float y, int width, int height, float direction, float speed, float damage) : base(origin.Scene, (int)x,(int)y,width,height)
        {
            Damage = damage;
            Direction = direction;
            Speed = speed;

            _sprite = AssetLoader.GetTexture("standard_bullet");
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_sprite, destinationRectangle : Bounds, color : Color.Red);
        }

        public override void OnDespawn()
        {
     
        }

        public override void OnSpawn()
        {
        }

        public override void Update(GameTime gameTime)
        {
            Velocity = new Vector2((float)(Math.Cos(Direction * Speed)), (float)(Math.Sin(Direction * Speed)));
            Position = new Vector2(Position.X + Velocity.X * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

        }
    }
}
