using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Nez.Sprites;
using System;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects
{
    public enum SpawnerState
    {
        Closed, Opening, Closing
    }
    public class Spawner : GameObject
    {
        private SpawnerState _spawnerState;
        public Gun CurrentItem;
        private Cooldown _spawnTimer { get; set; }
        private Cooldown _stayOpenTimer { get; set; }

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

            _spawnTimer = new Cooldown(10f);
            _stayOpenTimer = new Cooldown(3.5f);
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

            entity.addComponent(new CameraTracker(() => _spawnerState != SpawnerState.Closed));

            _animation.play(Animations.Idle);
            _spawnTimer.Start();
        }

        public void SpawnItem()
        {
            var entz = entity.scene.createEntity("collectible");
            var col = entz.addComponent(new Collectible((int)entity.position.X, (int)entity.position.Y, "M4",false));
            col.transform.setScale(0.3f);
        }

        public override void update()
        {
            _spawnTimer.Update();
            _stayOpenTimer.Update();
        
            if (_spawnTimer.IsReady() && _spawnerState == SpawnerState.Closed)
            {
                _spawnerState = SpawnerState.Opening;
                _animation.play(Animations.Open);
                _stayOpenTimer.Start();

                SpawnItem();
            }

            if(_stayOpenTimer.IsReady() && _spawnerState == SpawnerState.Opening)
            {
                _spawnerState = SpawnerState.Closing;
                _animation.play(Animations.Close);
            }

            if(_animation.isAnimationPlaying(Animations.Close) && !_animation.isPlaying)
            {
                // TODO Don't start unless current spawned item is picked up
                _animation.play(Animations.Idle);
                _spawnerState = SpawnerState.Closed;
                _spawnTimer.Start();
            }
        }
    }
}
