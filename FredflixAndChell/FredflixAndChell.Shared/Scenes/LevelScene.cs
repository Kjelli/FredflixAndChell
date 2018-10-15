using FredflixAndChell.Shared.Levels;
using FredflixAndChell.Shared.Utilities.Graphics.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FredflixAndChell.Shared.Scenes
{
    public class LevelScene : Scene
    {
        public Level Level { get; set; }
        public LevelScene(SmoothCamera2D camera) : base(camera)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Level.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            Level.Draw(spriteBatch, gameTime);
        }
    }
}
