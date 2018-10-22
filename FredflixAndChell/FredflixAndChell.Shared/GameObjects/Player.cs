﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;

using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Player : GameObject
    {
        private const short LEFT = 1;
        private const short UP = 2;
        private const short DOWN = 4;
        private const short RIGHT = 8;

        private Vector2 Acceleration;

        private float _speed = 0.13f;

        public float FacingAngle { get; set; }

        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }


        private Animation _currentAnimation;
        private Animation _animationWalking;
        private Animation _animationStopped;
        private Texture2D _rainbow;
        private Effect _rainbowEffect;

        private bool _hasFlashEffect = false;

        public InputActions Actions { get; set; }

        public Gun gun;



        public Player(IScene scene, int x, int y) : base(scene, x, y, 128, 128)
        {
            SetupAnimations();
            Actions = new InputActions();

            //TODO: Gunz
            gun = new Gun(this, scene, (int)Position.X, (int)Position.Y, 64, 64, cooldown: 0.15f);
        }

        private void SetupAnimations()
        {
            _animationWalking = new Animation(new Texture2D[] {
                AssetLoader.GetTexture("kjelli[1][0]"),
                AssetLoader.GetTexture("kjelli[1][1]"),
                AssetLoader.GetTexture("kjelli[1][2]"),
                AssetLoader.GetTexture("kjelli[1][3]"),
                AssetLoader.GetTexture("kjelli[1][4]"),
                AssetLoader.GetTexture("kjelli[1][5]"),
                AssetLoader.GetTexture("kjelli[1][6]"),
                AssetLoader.GetTexture("kjelli[1][7]"),


            }, new AnimationSettings
            {
                FrameDurationMillis = 80,
                Loop = true,
                Autoplay = true
            });

            _animationStopped = new Animation(new Texture2D[] {
                AssetLoader.GetTexture("kjelli[0][0]"),
                AssetLoader.GetTexture("kjelli[0][1]"),
                AssetLoader.GetTexture("kjelli[0][2]"),
                AssetLoader.GetTexture("kjelli[0][3]"),
            }, new AnimationSettings
            {
                FrameDurationMillis = 200,
                Loop = true,
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
            if (_hasFlashEffect)
            {
                _rainbowEffect.Parameters["gameTime"]?.SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
                _rainbowEffect.Techniques[0].Passes[0].Apply();
            }


            spriteBatch.Draw(_currentAnimation.CurrentFrame, layerDepth: 0.5f, destinationRectangle: Bounds, effects: (HorizontalFacing == (int)FacingCode.LEFT ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
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
            Acceleration = new Vector2(this.Actions.MoveX * _speed, -this.Actions.MoveY * _speed);

            //TODO: Ikke hardcode top speed etc
            _animationWalking.Settings.FrameDurationMillis = 200 - ((float)Acceleration.Length() * 1000); ;

            Velocity = new Vector2(Velocity.X * 0.8f + Acceleration.X * 0.2f, Velocity.Y * 0.8f + Acceleration.Y * 0.2f);
            Position = new Vector2(Position.X + Velocity.X * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Velocity.Y * gameTime.ElapsedGameTime.Milliseconds);

            UpdateAnimation(gameTime);
            SetFacing();
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

        public void Attack()
        {
            gun.Fire();
        }

        private void SetFacing()
        {
            if (FacingAngle < -2.0 && FacingAngle > 1.0)
                VerticalFacing = (int)FacingCode.UP;
            else if (FacingAngle > 1.0 && FacingAngle < 2.0)
                VerticalFacing = (int)FacingCode.DOWN;
            //Prioritizing "horizontal" sprites
            else if (FacingAngle > -1 && FacingAngle < 1)
                HorizontalFacing = (int)FacingCode.UP;
            else
                HorizontalFacing = (int)FacingCode.LEFT;
        }
    }

    public enum FacingCode
    {
        UP = 1,
        RIGHT = 2,
        DOWN = 3,
        LEFT = 4

    }
}
