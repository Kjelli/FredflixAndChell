using FredflixAndChell.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FredflixAndChell.Shared.Scenes
{
    public interface IScene
    {

        List<GameObject> GameObjects { get; }

        void Spawn(GameObject gameObject);
        void Despawn(GameObject gameObject);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        void DrawDebug(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
