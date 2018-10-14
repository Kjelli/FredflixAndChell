using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Animations;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Player : GameObject
    {
        private const short LEFT = 1;
        private const short UP = 2;
        private const short DOWN = 4;
        private const short RIGHT = 8;

        private Vector2 Acceleration;
        private Vector2 Velocity;
        private int _direction;
        private float _speed = 0.1f;

        private Animation _currentAnimation;
        private Animation _animationWalking;
        private Animation _animationStopped;
        private Texture2D _rainbow;
        private Effect _rainbowEffect;
        private bool hasFlashEffect = false;

        public Player(int x, int y) : base(x, y, 64, 64)
        {
            SetupAnimations();

            KeyboardInputUtility.While(Keys.A, () => _direction |= LEFT, () => _direction &= (_direction - LEFT));
            KeyboardInputUtility.While(Keys.W, () => _direction |= UP, () => _direction &= (_direction - UP));
            KeyboardInputUtility.While(Keys.S, () => _direction |= DOWN, () => _direction &= (_direction - DOWN));
            KeyboardInputUtility.While(Keys.D, () => _direction |= RIGHT, () => _direction &= (_direction - RIGHT));
            KeyboardInputUtility.While(Keys.Space, () => hasFlashEffect = !hasFlashEffect);
        }

        private void SetupAnimations()
        {
            _animationWalking = new Animation(new Texture2D[] {
                AssetLoader.GetTexture("pig[0][0]"),
                AssetLoader.GetTexture("pig[1][0]"),
                AssetLoader.GetTexture("pig[2][0]"),
            }, new AnimationSettings
            {
                FrameDurationMillis = 80,
                Loop = true,
                Autoplay = true
            });

            _animationStopped = new Animation(new Texture2D[] {
                AssetLoader.GetTexture("pig[0][0]"),
            }, new AnimationSettings
            {
                FrameDurationMillis = 1000,
                Loop = false,
                Autoplay = true
            });

            _rainbow = AssetLoader.GetTexture("rainbow");
            _rainbowEffect = AssetLoader.GetEffect("flash");
            _rainbowEffect.Parameters["flash_texture"].SetValue(_rainbow);
            _rainbowEffect.Parameters["scrollSpeed"].SetValue(new Vector2(0.2f, 0.2f));
            _rainbowEffect.Parameters["flashRate"].SetValue(0f);
            _rainbowEffect.Parameters["flashOffset"].SetValue(.5f);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (hasFlashEffect)
            {
                _rainbowEffect.Parameters["gameTime"]?.SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
                _rainbowEffect.Techniques[0].Passes[0].Apply();
            }
#pragma warning disable CS0618 // Type or member is obsolete
            spriteBatch.Draw(_currentAnimation.CurrentFrame, destinationRectangle: Bounds, effects: (Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {
            _currentAnimation = _animationWalking;
        }

        public override void Update(GameTime gameTime)
        {
            Acceleration = new Vector2((_direction & LEFT) > 0 ? -_speed : (_direction & RIGHT) > 0 ? _speed : 0, (_direction & DOWN) > 0 ? _speed : (_direction & UP) > 0 ? -_speed : 0);
            Velocity = new Vector2(Velocity.X * 0.8f + Acceleration.X * 0.2f, Velocity.Y * 0.8f + Acceleration.Y * 0.2f);
            Position = new Vector2(Position.X + Velocity.X * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

            UpdateAnimation(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            _currentAnimation.Update(gameTime);

            if (Acceleration.Length() > 0)
            {
                _currentAnimation = _animationWalking;
            }
            else
            {
                _currentAnimation = _animationStopped;
            }
        }
    }
}
