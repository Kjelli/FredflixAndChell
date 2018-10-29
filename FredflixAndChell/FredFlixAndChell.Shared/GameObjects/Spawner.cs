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
        public Cooldown timer { get; set; }

        bool deleteme;


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

            timer = new Cooldown(10f);

            deleteme = false;
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
                FPS = 1,
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
            timer.Start();

        }

        public void SpawnItem()
        {
            var entz = entity.scene.createEntity("collectible");
            var col = entz.addComponent(new Collectible((int)entity.position.X, (int)entity.position.Y, CollectiblePreset.M4));
            col.transform.setScale(0.3f);
        }

        public override void update()
        {
            timer.Update();

            if (timer.IsReady() && deleteme != true)
            {
                _animation.play(Animations.Open);   
                deleteme = true ;
                SpawnItem();
            }
        }
    }
}
