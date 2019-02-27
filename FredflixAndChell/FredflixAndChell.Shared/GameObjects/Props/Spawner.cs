using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Effects;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Effects;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using static FredflixAndChell.Shared.Assets.Constants;
using Random = Nez.Random;

namespace FredflixAndChell.Shared.GameObjects.Props
{
    public enum SpawnerState
    {
        Initial, Closed, Opening, Closing, Exhausted
    }
    public class Spawner : Entity
    {
        private const float FromOpeningToSpawnDelaySeconds = 4f;
        private const float FromSpawnToClosedDelaySeconds = 2f;

        private Sprite<Animations> _animation;
        private readonly SpawnerParameters _parameters;
        private List<CollectibleParameters> _allowedCollectibles;

        private SpawnerState _spawnerState;
        private Cooldown _timer;

        private int _spawnCount;
        private CollectibleParameters _nextItemToSpawn;
        private bool _blocked;
        private bool _addedToScene;

        private enum Animations
        {
            Idle,
            Open,
            Close
        }

        public Spawner(int x, int y, SpawnerParameters parameters)
        {
            position = new Vector2(x, y);
            _animation = SetupAnimations();
            _animation.renderLayer = Layers.MapObstacles;
            _animation.play(Animations.Idle);
            addComponent(_animation);

            _parameters = parameters;
            _allowedCollectibles = FilterAllowedCollectibles();
        }

        private List<CollectibleParameters> FilterAllowedCollectibles()
        {
            var items = Collectibles.Collectibles.All().AsEnumerable();

            // If set, include whitelisted weapons by rarity
            if (_parameters.RarityWhitelist?.Count() > 0)
                items = items.Where(i => _parameters.RarityWhitelist.contains(i.Rarity.ToString()));

            // If set, exclude blacklisted weapons by rarity
            if (_parameters.RarityBlacklist?.Count() > 0)
                items = items.Where(i => !_parameters.RarityBlacklist.contains(i.Rarity.ToString()));

            // If set, include whitelisted weapons by name
            if (_parameters.WeaponWhitelist?.Count() > 0)
                items = items.Where(i => _parameters.WeaponWhitelist.contains(i.Name));

            // If set, exclude blacklisted weapons by name
            if (_parameters.WeaponBlacklist?.Count() > 0)
                items = items.Where(i => !_parameters.WeaponBlacklist.contains(i.Name));

            return items.ToList();
        }

       
        private Sprite<Animations> SetupAnimations()
        {
            var animations = new Sprite<Animations>();

            var idle = new SpriteAnimationDescriptor
            {
                Frames = new int[] { 0 },
                FPS = 1,
                Loop = true
            }.ToSpriteAnimation("maps/spawner_tile", tileWidth: 16, tileHeight: 16);
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
                Frames = new int[] { 4, 3, 2, 1, 0 },
                FPS = 15,
                Loop = false,


            }.ToSpriteAnimation("maps/spawner_tile", tileWidth: 16, tileHeight: 16);
            animations.addAnimation(Animations.Close, close);
            return animations;
        }

        public override void onAddedToScene()
        {
            addComponent(new CameraTracker(() =>
                _parameters.CameraTracking && _spawnerState != SpawnerState.Closed && _spawnerState != SpawnerState.Initial
            ));

            _addedToScene = true;
        }
        private void StartTimer()
        {
            if (_parameters.MaxSpawns > 0 && _spawnCount >= _parameters.MaxSpawns)
            {
                Console.WriteLine("Reached maximum spawn count");
                _spawnerState = SpawnerState.Exhausted;
                return;
            }

            var timeUntilNextSpawn = Random.range(_parameters.MinIntervalSeconds, _parameters.MaxIntervalSeconds);
            _timer = new Cooldown(timeUntilNextSpawn);
            _timer.Start();
            Console.WriteLine("Between " + _parameters.MinIntervalSeconds + " and " + _parameters.MaxIntervalSeconds);
            Console.WriteLine("  This should wait " + timeUntilNextSpawn + " seconds until next spawn...");
        }

        private void SpawnItem()
        {
            var collectible = new Collectible(position.X, position.Y, _nextItemToSpawn.Name, false);
            collectible.OnPickupEvent += _ => _blocked = false;

            scene.addEntity(collectible);
            _blocked = true;
        }

        private Rarity DrawRarity()
        {
            var roll = Random.range(0, 12);

            if (roll == 11)
                return Rarity.Legendary;
            else if (roll >= 9)
                return Rarity.Epic;
            else if (roll >= 5)
                return Rarity.Rare;
            else
                return Rarity.Common;
        }

        private CollectibleParameters GetRandomCollectible()
        {
            if (_allowedCollectibles.Count == 0) throw new System.Exception("No allowed collectibles, nothing could spawn!");

            CollectibleParameters item = null;

            do
            {
                var rarity = DrawRarity();
                var potentialItems = _allowedCollectibles
                    .Where(w => w.Rarity == rarity)
                    .ToList();
                if (potentialItems.Count == 0) continue;

                item = potentialItems.randomItem();
            } while (item == null);

            return item;
        }

        public override void update()
        {
            base.update();
            _timer?.Update();

            switch (_spawnerState)
            {
                case SpawnerState.Initial:
                    if (!_addedToScene || _blocked) break;
                    Console.WriteLine("Counting down spawner");

                    _spawnerState = SpawnerState.Closed;
                    StartTimer();
                    break;
                case SpawnerState.Closed:
                    if (_timer.IsReady() && !_blocked)
                    {
                        _nextItemToSpawn = GetRandomCollectible();
                        Console.WriteLine("Spawning " + _nextItemToSpawn.Name + " in " + FromOpeningToSpawnDelaySeconds + " seconds" );

                        // TODO: Light effects? Color indicating rarity?
                        _animation.play(Animations.Open);
                        _spawnerState = SpawnerState.Opening;
                        _timer = new Cooldown(FromOpeningToSpawnDelaySeconds);
                        _timer.Start();
                    }
                    break;
                case SpawnerState.Opening:

                    if (Time.checkEvery(0.5f))
                    {
                        scene.addEntity(new SpawnRing(position, ResolveRarityColor(_nextItemToSpawn)));
                    }

                    if (_timer.IsReady())
                    {
                        Console.WriteLine("Spawned " + _nextItemToSpawn.Name + "!");

                        SpawnItem();
                        _spawnCount++;
                        _spawnerState = SpawnerState.Closing;
                        _animation.play(Animations.Close);
                        _timer = new Cooldown(FromSpawnToClosedDelaySeconds);
                        _timer.Start();
                    }
                    break;
                case SpawnerState.Closing:
                    if (_timer.IsReady())
                    {
                        _spawnerState = SpawnerState.Initial;
                        _animation.play(Animations.Idle);
                    }
                    break;
            }
        }

        private Color ResolveRarityColor(CollectibleParameters collectible)
        {
            switch (collectible.Rarity)
            {
                default:
                case Rarity.Common:
                    return new Color(Color.White, 0.5f);
                case Rarity.Rare:
                    return new Color(Color.Blue, 0.5f);
                case Rarity.Epic:
                    return new Color(Color.Purple, 0.5f);
                case Rarity.Legendary:
                    return new Color(Color.Orange, 0.5f);
            }
        }

        public class SpawnerParameters
        {
            public int MaxSpawns { get; set; } = -1;
            public float MinIntervalSeconds { get; set; } = 5.0f;
            public float MaxIntervalSeconds { get; set; } = 15.0f;
            public string[] WeaponWhitelist { get; set; }
            public string[] WeaponBlacklist { get; set; }
            public string[] RarityWhitelist { get; set; }
            public string[] RarityBlacklist { get; set; }
            public bool CameraTracking { get; set; } = true;
        }
    }
}
