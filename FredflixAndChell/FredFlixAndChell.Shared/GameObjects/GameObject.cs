using FredflixAndChell.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics;

namespace FredflixAndChell.Shared.GameObjects
{
    public abstract class GameObject : IGameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
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
            Position = new Vector2();
            Size = new Vector2(width, height);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void OnDespawn();
        public abstract void OnSpawn();
        public abstract void Update(GameTime gameTime);

        public void DebugDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Bounding box
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.X + Size.X, Bounds.Y), Color.Green);
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X + Size.X, Bounds.Y), new Vector2(Bounds.X + Size.X, Bounds.Y + Size.Y), Color.Green);
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X + Size.X, Bounds.Y + Size.Y), new Vector2(Bounds.X, Bounds.Y + Size.Y), Color.Green);
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X, Bounds.Y + Size.Y), new Vector2(Bounds.X, Bounds.Y), Color.Green);
                     
            // Centerer of bounds
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X + Size.X / 2 - 5, Bounds.Y + Size.Y / 2 - 5), new Vector2(Bounds.X + Size.X / 2 + 5, Bounds.Y + Size.Y / 2 + 5), Color.Blue);
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X + Size.X / 2 + 5, Bounds.Y + Size.Y / 2 - 5), new Vector2(Bounds.X + Size.X / 2 - 5, Bounds.Y + Size.Y / 2 + 5), Color.Blue);
                     
            // Directerion
            DebugDrawer.DrawLine(spriteBatch, new Vector2(Bounds.X + Size.X / 2, Bounds.Y + Size.Y / 2), new Vector2(Bounds.X + Size.X / 2 + Velocity.X * 300, Bounds.Y + Size.Y / 2 + Velocity.Y * 300), Color.Blue);
        }
    }
}
