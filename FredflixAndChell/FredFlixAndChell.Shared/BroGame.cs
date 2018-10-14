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
using FredflixAndChell.Shared.Utilities.Graphics.Cameras;

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
        private SmoothCamera2D _camera;

        private SpriteFont _debugFont;

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

        }

        protected override void Initialize()
        {
            base.Initialize();
            
            _debugFont = AssetLoader.GetFont("debug");

            KeyboardUtility.While(Keys.F2, () => _isDrawingDebug = !_isDrawingDebug);
            KeyboardUtility.While(Keys.Up, () => _camera.ZoomIn(0.01f), repeat: true);
            KeyboardUtility.While(Keys.Down, () => _camera.ZoomOut(0.01f), repeat: true);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 640, 480);
            _camera = new SmoothCamera2D(viewportAdapter);
            _camera.SpeedRatio = 0.04f;
            _scene = new Scene(_camera);

            //Players initialize - Keyboard = player 1

            _players.Add(new PlayerController(_scene, 0));

            for (var i = 0; i < 4; i++)
            {
                if (GamePad.GetCapabilities(GamePadUtility.ConvertToIndex(i + 1)).IsConnected)
                {
                    Console.WriteLine($"Gamepad {i + 1} Detected - Generating player");
                    _players.Add(new PlayerController(_scene, i + 1));
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
            KeyboardUtility.Poll();

            foreach (var player in _players)
            {
                player.Update();
            }

            _camera.LookAtSmooth(_players[0].Player.Position + _players[0].Player.Size / 2);
            _scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var transformMatrix = _camera.GetViewMatrix();

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: transformMatrix);
            _scene.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            if (_isDrawingDebug)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: transformMatrix);
                _scene.DrawDebug(_spriteBatch, gameTime);
                _spriteBatch.DrawString(_debugFont, $"FPS:      no.", _camera.Position + new Vector2(0, 0), Color.White);
                _spriteBatch.DrawString(_debugFont, $"Time elapsed: {gameTime.TotalGameTime.TotalSeconds}", _camera.Position + new Vector2(0, 20), Color.White);
                _spriteBatch.DrawString(_debugFont, "Memory: " + GC.GetTotalMemory(false) / 1024, _camera.Position + new Vector2(0, 40), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
