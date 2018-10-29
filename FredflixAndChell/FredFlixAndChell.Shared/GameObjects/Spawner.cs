using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
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
        private Cooldown _timer { get; set; }
        private Cooldown _closeDelay { get; set; }

        bool open;


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

            _timer = new Cooldown(10f);
            _closeDelay = new Cooldown(1.5f);

            open = false;
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
                Frames = new int[] { 4,3,2,1 ,0},
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
            var sprite = entity.addComponent(_animation);
            sprite.renderLayer = Layers.MapObstacles;

            _animation.play(Animations.Idle);
            _timer.Start();

        }

        public void SpawnItem()
        {
            var entz = entity.scene.createEntity("collectible");
            var col = entz.addComponent(new Collectible((int)entity.position.X, (int)entity.position.Y, "M4",false));
            col.transform.setScale(0.3f);
        }

        public override void update()
        {
            _timer.Update();
            _closeDelay.Update();
        
            if (_timer.IsReady() && !open)
            {
                _animation.play(Animations.Open);   
                open = true ;
                SpawnItem();
                _closeDelay.Start();
            }

            if(open && _closeDelay.IsReady())
            {
                _animation.play(Animations.Close);
                open = false;
                _timer.Start();
            }
        }
    }
}
