using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FredflixAndChell.Shared.GameObjects
{
    public interface IGameObject
    {
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }

        Rectangle Bounds { get; }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        void OnSpawn();
        void OnDespawn();
    }
}
