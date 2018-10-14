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

        private List<PlayerController> players = new List<PlayerController>();

        public BroGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
           // IsFixedTimeStep = true;
           // _graphics.SynchronizeWithVerticalRetrace = true;
           // TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 16);
            _scene = new Scene();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            _frameCounter = new FrameCounter();
            debugFont = AssetLoader.GetFont("debugfont");

            //Players initilize
            //Keyboard = player 1, 
            players.Add(new PlayerController(_scene,0));
            for(var i = 0; i < 4; i++)
            { 
                if (GamePad.GetCapabilities(GamePadUtility.ConvertToIndex(i+1)).IsConnected)
                {
                    Console.WriteLine($"Gamepad {i + 1} Detected - Generating player");
                    players.Add(new PlayerController(_scene,i+1));
                } 
            }

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here 
            AssetLoader.Load(Content);
        }

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

            foreach(var player in players)
            {
                player.Update();
            }  
            

            _scene.GameObjects.ForEach(g => g.Update(gameTime));

           // FPS Logic
           var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);

            // TODO: Add your update logic here			
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.DrawString(debugFont, $"FPS: {_frameCounter.AverageFramesPerSecond} ({_frameCounter.CurrentFramesPerSecond})", new Vector2(0,0), Color.White);
            _spriteBatch.DrawString(debugFont, $"Time elapsed: {gameTime.TotalGameTime.TotalSeconds}", new Vector2(0,20), Color.White);
            _spriteBatch.DrawString(debugFont, "Memory: " + GC.GetTotalMemory(false) / 1024, new Vector2(0, 40), Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _scene.GameObjects.ForEach(g => g.Draw(_spriteBatch, gameTime));
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
