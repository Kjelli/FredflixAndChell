using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Nez.Sprites;
using System;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Spawner : GameObject
    {

        public Gun CurrentItem;
        public Cooldown timer { get; set; }


        private Sprite<Animations> _animation;
        public enum Animations
        {
            Idle,
            Open,
            Close
        }
        public Spawner(int x, int y) : base(x, y)
        {
            _animation = SetupAnimations();

            timer = new Cooldown(7f);

           
        }

        private Sprite<Animations> SetupAnimations()
        {
            var animations = new Sprite<Animations>();
                
            var idle = new SpriteAnimationDescriptor
            {
                Frames = new int[] { 0 },
                FPS = 1,
                Loop = true
            }.ToSpriteAnimation("maps/spawner_tile",  tileWidth : 16, tileHeight : 16);
            animations.addAnimation(Animations.Idle, idle);

            

            var open = new SpriteAnimationDescriptor
            {
                Frames = new int[] 
                {
                    1,2,3,4
                },
                FPS = 15,
                Loop = false
            }.ToSpriteAnimation("maps/spawner_tile", tileWidth: 16, tileHeight: 16);
            animations.addAnimation(Animations.Open, open);
           

            var close = new SpriteAnimationDescriptor
            {
                Frames = new int[] { 4,3,2,1 },
                FPS = 15,
                Loop = false,
                
                
            }.ToSpriteAnimation("maps/spawner_tile", tileWidth: 16, tileHeight: 16);
            animations.addAnimation(Animations.Close, close);
            return animations;
        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {
            var sprite = entity.addComponent(new Sprite(_animation.getAnimation(Animations.Idle).frames[0]));
            sprite.renderLayer = Layers.MapObstacles;

            _animation.play(Animations.Idle);
            timer.Start();

        }

        public override void update()
        {
            timer.Update();

            if (timer.IsReady())
            {
                _animation.play(Animations.Open);
            }
        }
    }
}
