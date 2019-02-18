using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Nez;
using Nez.Sprites;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Props
{
    public enum SpawnerState
    {
        Closed, Opening, Closing
    }
    public class Spawner : GameObject, ITriggerListener
    {
        private SpawnerState _spawnerState;
        public Collectible CurrentItem;
        private int _registerOffset { get; } = 20;
        private float _spawnRate { get; set; }

        private Cooldown _spawnTimer { get; set; }
        private Cooldown _stayOpenTimer { get; set; }

        private Cooldown _timeChecker { get; set; }

        private bool _unoccupied { get; set; } = true;

        private Sprite<Animations> _animation;

        private System.Random rng = new System.Random();
        

        public enum Animations
        {
            Idle,
            Open,
            Close
        }
        public Spawner(int x, int y, float spawnRate = 0.3f,string onlySpawn = null) : base(x, y)
        {
            _animation = SetupAnimations();

            //Time from "announced spawn" to actual spawning (color, show rarity etc)
            _spawnTimer = new Cooldown(4f);
            //Animation
            _stayOpenTimer = new Cooldown(3.5f);

            _spawnRate = spawnRate;

            //How often the spawner might spawn
            //Spawn rate is percentage of that happending
            //5 in spawnrate and 3 in timechecker, gives 5% chance every 3 second to spawn 
            _timeChecker = new Cooldown(3f);

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
            var sprite = addComponent(_animation);
            sprite.renderLayer = Layers.MapObstacles;

            addComponent(new CameraTracker(() => _spawnerState != SpawnerState.Closed));

            _animation.play(Animations.Idle);
            _spawnTimer.Start();

            var hitbox = addComponent(new CircleCollider(4f));

            Flags.setFlagExclusive(ref hitbox.collidesWithLayers, Layers.Interactables);
            Flags.setFlagExclusive(ref hitbox.physicsLayer, 0);
            hitbox.isTrigger = true;

        }

        public void onTriggerExit(Collider other, Collider local)
        {
            if (other == null && local == null) return;

            if(other.entity == CurrentItem) {
                CurrentItem = null;
            }
        }



        public void SpawnItem()
        {
            CurrentItem = new Collectible((int)position.X, (int)position.Y, GetRandomItem(), false);
            var col = scene.addEntity(CurrentItem);
            col.transform.setScale(0.3f);
            _unoccupied = false;
        }

        public Rarity DrawRarity()
        {
            return Rarity.Common;
            //TODO: DONT RETURN COMMON - return picked one brah

            var roll = rng.Next(0,11);
            if (roll == 11)
                return Rarity.Legendary;
            else if (roll >= 9)
                return Rarity.Epic;
            else if (roll >= 5)
                return Rarity.Rare;
        }

        public string GetRandomItem()
        {
            var items = Collectibles.Collectibles.All(DrawRarity());
            ShuffleCollectibleList(items);
            double rand = rng.NextDouble();
            foreach(var item in items)
            {
                if(item.DropChance > rand)
                {
                    return item.Name;
                }
            }
            return items[0].Gun.Name;
        }

        public void ShuffleCollectibleList(List<CollectibleParameters> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }




        public bool ReadyToSpawn()
        {
            if (_spawnTimer.IsReady() && _spawnerState == SpawnerState.Closed)
            {
                if (CurrentItem == null)
                    return true;
                else
                    return CurrentItem == null ? true : false;
            }
            return false;
        }


        public override void Update()
        {
            _spawnTimer.Update();
            _stayOpenTimer.Update();
            _timeChecker.Update();

            if (_timeChecker.IsReady() && ReadyToSpawn())
            {
                //Try to spawn
                var roll = rng.NextDouble();
                if(roll < _spawnRate)
                {
                    _spawnerState = SpawnerState.Opening;
                    _animation.play(Animations.Open);
                    _stayOpenTimer.Start();

                    SpawnItem();
                }
                else
                {
                    _timeChecker.Start();
                }
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

        public void onTriggerEnter(Collider other, Collider local)
        {
        }
    }
}
