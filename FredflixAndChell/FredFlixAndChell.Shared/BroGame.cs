#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#endregion

namespace FredFlixAndChell.Shared
{
    public class BroGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D rainbow;
        Texture2D pixels;
        Vector2 Position;
        Effect effect;
        Effect lightingEffect;

        Texture2D lightMask;
        Texture2D surge;

        RenderTarget2D lightsTarget;
        RenderTarget2D mainTarget;

        public BroGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 16);
            Position = new Vector2(0, 0);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here 
            pixels = Content.Load<Texture2D>("dick");
            rainbow = Content.Load<Texture2D>("rainbow");
            effect = Content.Load<Effect>("shader");
            lightingEffect = Content.Load<Effect>("light");

            lightMask = Content.Load<Texture2D>("lightmask");

            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(
                GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(
                GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
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
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.D))
            {
                Position.X += 3;
            }
            if (state.IsKeyDown(Keys.A))
            {
                Position.X -= 3;
            }
            if (state.IsKeyDown(Keys.W))
            {
                Position.Y -= 3;
            }
            if (state.IsKeyDown(Keys.S))
            {
                Position.Y += 3;
            }
            // TODO: Add your update logic here			
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //TODO: Add your drawing code here

            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            float n = 10;
            for (float i = 0; i < n; i++)
            {
                var x = Position.X + pixels.Bounds.Width / 2 - lightMask.Bounds.Width / 2 + (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds / (100 * i) + i / (10 * (n - i))) * 170;
                var y = Position.Y + pixels.Bounds.Height / 2 - lightMask.Bounds.Height / 2 + (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / (100 * i) + i / (10 * (n - i))) * 40;
                spriteBatch.Draw(lightMask,
                                new Vector2(x,y),
                                new Color(
                                    0.45f * i / n + (n - i / n) * 0.6f * (float)Math.Sin(x / 100),
                                    0.75f * i / n + (n - i / n) * 0.25f * (float)Math.Cos((x + 250) / 100),
                                    0.15f * i / n + (n - i / n) * 0.85f * (float)Math.Sin((y + 500) / 100)));
            }

            spriteBatch.End();

            // Draw the main scene to the Render Target
            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["rainbow"].SetValue(rainbow);
            effect.Parameters["gameTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            spriteBatch.Draw(pixels, Position, Color.White);
            spriteBatch.End();

            // Draw the main scene with a pixel
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            lightingEffect.Parameters["lightMask"].SetValue(lightsTarget);
            lightingEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
