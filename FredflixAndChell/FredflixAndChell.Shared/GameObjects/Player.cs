using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities;
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

        private Animation _animation;
        private Animation _animationWalking;
        private Animation _animationStopped;
        private Sprite _renderTexture;
        private Mover _mover;
        private PlayerController _controller;

        public Player() : base(128, 128)
        {
            SetupAnimations();
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
            System.Console.WriteLine(_controller.XAxis);
            Acceleration = new Vector2(_controller.XAxis * _speed, _controller.YAxis * _speed);
            Velocity = new Vector2(Velocity.X * 0.8f + Acceleration.X * 0.2f, Velocity.Y * 0.8f + Acceleration.Y * 0.2f);

            _mover.move(Velocity, out CollisionResult result);

            UpdateAnimation();
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
    }
}
