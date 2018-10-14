using FredflixAndChell.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FredflixAndChell.Shared.GameObjects
{
    public abstract class GameObject : IGameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Rectangle Bounds { get => new Rectangle(Position.ToPoint(), Size.ToPoint()); }

        public GameObject() : this(0,0)
        {
        }

        public GameObject(int x, int y) : this(x,y,32,32)
        {
        }

        public GameObject(int x, int y, int width, int height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void OnDespawn();
        public abstract void OnSpawn();
        public abstract void Update(GameTime gameTime);
    }
}
