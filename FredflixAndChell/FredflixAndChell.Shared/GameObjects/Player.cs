using System;
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
using Nez;
using Nez.Sprites;
using Nez.Tiled;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Player : GameObject
    {
        private Vector2 Acceleration;

        private float _speed = 0.13f;

        public float FacingAngle { get; set; }

        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }


        private Animation _animation;
        private Animation _animationWalking;
        private Animation _animationStopped;
        private Sprite _renderTexture;
        private Mover _mover;
        private PlayerController _controller;



        public Gun gun;



        public Player() : base(128, 128)
        {
            SetupAnimations();

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
            _animation = _animationStopped;

        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {

            _controller = entity.addComponent(new PlayerController(0));
            _mover = entity.addComponent(new Mover());

            var sprite = entity.addComponent(new Sprite(_animation.CurrentFrame));
        }

        public override void update()
        {
            _animationWalking.Settings.FrameDurationMillis = 0.2f - ((float)Acceleration.Length()); ;
            System.Console.WriteLine(_controller.XAxis);
            Acceleration = new Vector2(_controller.XAxis * _speed, _controller.YAxis * _speed);
            Velocity = new Vector2(Velocity.X * 0.8f + Acceleration.X * 0.2f, Velocity.Y * 0.8f + Acceleration.Y * 0.2f);

            _mover.move(Velocity, out CollisionResult result);

            UpdateAnimation(gameTime);
            SetFacing();
        }

        private void UpdateAnimation()
        {
            _animation.Update();

            if (Acceleration.Length() > 0)
            {
                _animation = _animationWalking;
            }
            else
            {
                _animation = _animationStopped;
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
