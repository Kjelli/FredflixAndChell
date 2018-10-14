#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Scenes;
using System.Diagnostics;

#endregion

namespace FredflixAndChell.Shared
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        EndOfGame,
    }

    public class BroGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FrameCounter _frameCounter;

        private SpriteFont debugFont;

        private GameState _gameState;
        private IScene _scene;

        public BroGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 8);
            _scene = new Scene();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            var random = new Random((int)DateTime.Now.Ticks);
            _scene.Spawn(new Player(random.Next(128, 256), random.Next(128, 256)));
            _frameCounter = new FrameCounter();
            debugFont = AssetLoader.GetFont("debug");
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here 
            AssetLoader.Load(Content);
        }
        private DateTime oldTimestamp = DateTime.Now;
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
#endif
            KeyboardInputUtility.Poll();
            _scene.GameObjects.ForEach(g => g.Update(gameTime));

            // FPS Logic
            var now = DateTime.Now;
            _frameCounter.Update((float)(now - oldTimestamp).TotalSeconds + 0.00001f);
            oldTimestamp = now;
            // TODO: Add your update logic here			
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            _scene.GameObjects.ForEach(g => g.Draw(_spriteBatch, gameTime));
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.DrawString(debugFont, $"FPS: {_frameCounter.AverageFramesPerSecond}", new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(debugFont, $"Time elapsed: {gameTime.TotalGameTime.TotalSeconds}", new Vector2(0, 20), Color.White);
            _spriteBatch.DrawString(debugFont, "Memory: " + GC.GetTotalMemory(false) / 1024, new Vector2(0, 40), Color.White);
            _spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
