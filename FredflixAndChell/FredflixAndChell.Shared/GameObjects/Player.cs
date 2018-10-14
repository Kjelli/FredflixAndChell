using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;

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
        private float _speed = 0.2f;

        private Texture2D _texture;
        private Texture2D _rainbow;
        private Effect _rainbowEffect;

        public Player(int x, int y) : base(x, y, 128, 128)
        {
            _texture = AssetLoader.GetTexture("pig[0][0]");
            _rainbow = AssetLoader.GetTexture("rainbow");
            _rainbowEffect = AssetLoader.GetEffect("shader");
            _rainbowEffect.Parameters["rainbow"].SetValue(_rainbow);

            InputUtility.While(Keys.A, () => _direction |= LEFT, () => _direction &= (_direction - LEFT));
            InputUtility.While(Keys.W, () => _direction |= UP, () => _direction &= (_direction - UP));
            InputUtility.While(Keys.S, () => _direction |= DOWN, () => _direction &= (_direction - DOWN));
            InputUtility.While(Keys.D, () => _direction |= RIGHT, () => _direction &= (_direction - RIGHT));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
           // _rainbowEffect.Techniques[0].Passes[0].Apply();
            spriteBatch.Draw(_texture, Bounds, Color.White);
        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {

        }

        public override void Update(GameTime gameTime)
        {
            Acceleration = new Vector2((_direction & LEFT) > 0 ? - _speed : (_direction & RIGHT) > 0 ? _speed : 0, (_direction & DOWN) > 0 ? _speed : (_direction & UP) > 0 ? -_speed : 0);
            Velocity = new Vector2(Velocity.X * 0.8f + Acceleration.X * 0.2f, Velocity.Y * 0.8f + Acceleration.Y * 0.2f);
            Position = new Vector2(Position.X + Velocity.X * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Velocity.Y * gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}
