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
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using FredflixAndChell.Shared.Utilities.Graphics;

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
        private Camera2D _camera;

        private FrameCounter _frameCounter;
        private SpriteFont _debugFont;

        private DateTime oldTimestamp = DateTime.Now;

        private bool _isDrawingDebug;
        private GameState _gameState;
        private IScene _scene;


        private List<PlayerController> _players = new List<PlayerController>();

        public BroGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 8);
            IsFixedTimeStep = true;

            _scene = new Scene();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _frameCounter = new FrameCounter();

            //Players initialize
            //Keyboard = player 1, 
            _players.Add(new PlayerController(_scene, 0));
            for (var i = 0; i < 4; i++)
            {
                if (GamePad.GetCapabilities(GamePadUtility.ConvertToIndex(i + 1)).IsConnected)
                {
                    Console.WriteLine($"Gamepad {i + 1} Detected - Generating player");
                    _players.Add(new PlayerController(_scene, i + 1));
                }
            }

            _debugFont = AssetLoader.GetFont("debug");

            KeyboardUtility.While(Keys.F2, () => _isDrawingDebug = !_isDrawingDebug);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);
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
            KeyboardUtility.Poll();

            foreach (var player in _players)
            {
                player.Update();
            }

            _scene.Update(gameTime);

            // FPS Logic
            var now = DateTime.Now;
            _frameCounter.Update((float)(now - oldTimestamp).TotalSeconds);
            oldTimestamp = now;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            _scene.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            if (_isDrawingDebug)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
                _scene.DrawDebug(_spriteBatch, gameTime);
                _spriteBatch.DrawString(_debugFont, $"FPS: {_frameCounter.AverageFramesPerSecond}", new Vector2(0, 0), Color.White);
                _spriteBatch.DrawString(_debugFont, $"Time elapsed: {gameTime.TotalGameTime.TotalSeconds}", new Vector2(0, 20), Color.White);
                _spriteBatch.DrawString(_debugFont, "Memory: " + GC.GetTotalMemory(false) / 1024, new Vector2(0, 40), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
