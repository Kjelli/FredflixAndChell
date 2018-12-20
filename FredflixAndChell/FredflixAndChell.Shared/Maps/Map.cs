using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Effects.Weather;
using FredflixAndChell.Shared.Components.Interactables;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps
{
    public class Map : Entity
    {
        private PlayerSpawner _playerSpawner;
        private TiledMap _tiledMap;
        private GameSystem _gameSystem;

        public PlayerSpawner PlayerSpawner => _playerSpawner;
        public List<MapEventListener> MapEventListeners { get; set; }

        public Map() : base(TiledObjects.TiledMapEntity)
        {
            MapEventListeners = new List<MapEventListener>();
        }

        public override void onAddedToScene()
        {
            _gameSystem = scene.getSceneComponent<GameSystem>();
        }

        public void EmitMapEvent(MapEvent mapEvent, bool emitGlobally = false)
        {
            MapEventListeners
                .Where(listener => listener.EventKey == mapEvent.EventKey)
                .ToList()
                .ForEach(listener => listener.EventTriggered(mapEvent));

            if (emitGlobally)
            {
                _gameSystem.Publish(GameEvents.GlobalMapEvent,
                    new GlobalMapEventParameters { MapEvent = mapEvent });
            }
        }

        public void Setup(string mapName)
        {
            _tiledMap = AssetLoader.GetMap(mapName);

            var tiledMapComponent = addComponent(new TiledMapComponent(_tiledMap));
            tiledMapComponent.layerIndicesToRender = new int[] { 5, 2, 1, 0 };
            tiledMapComponent.renderLayer = Layers.MapBackground;
            tiledMapComponent.setMaterial(Material.stencilWrite(Stencils.EntityShadowStencil));

            SetupMapObjects();

            var tiledMapDetailsComponent = addComponent(new TiledMapComponent(_tiledMap));
            tiledMapDetailsComponent.layerIndicesToRender = new int[] { 3, 4 };
            tiledMapDetailsComponent.renderLayer = Layers.MapForeground;
            tiledMapDetailsComponent.setMaterial(Material.stencilWrite(Stencils.HiddenEntityStencil));
            //tiledMapDetailsComponent.material.effect = content.loadNezEffect<SpriteAlphaTestEffect>();

            //CustomizeTiles(tiledMapComponent);

            ApplyWeather(_tiledMap);
        }

        private void SetupMapObjects()
        {
            var mapObjects = _tiledMap.getObjectGroup(TiledObjects.ObjectGroup);

            SetupCollisions(mapObjects);
            SetupPits(mapObjects);
            SetupItemSpawners(mapObjects);
            SetupPlayerSpawns(mapObjects);
            SetupMapEvents(mapObjects);
            SetupScreens(mapObjects);

            foreach (var lightSource in mapObjects.objectsWithName("light_source"))
            {
                var entity = scene.createEntity("world-light", lightSource.position + new Vector2(8, 8));
                entity.setScale(0.5f);
                var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
                sprite.material = Material.blendLinearDodge();
                sprite.color = ColorExt.hexToColor("#" + lightSource.properties["color"].Substring(2));
                sprite.renderLayer = Layers.Lights;

                var props = lightSource.properties;
                if (props.ContainsKey("toggle_key") && !string.IsNullOrWhiteSpace(props["toggle_key"]))
                {
                    var listener = entity.addComponent(new MapEventListener(props["toggle_key"])
                    {
                        EventTriggered = _ =>
                        {
                            entity.setEnabled(!entity.enabled);
                        }
                    });
                    MapEventListeners.Add(listener);
                }
            }
        }

        private void SetupScreens(TiledObjectGroup mapObjects)
        {
            foreach (var screen in mapObjects.objectsWithName("screen"))
            {
                var props = screen.properties;
                var text = props.ContainsKey("text") ? props["text"] : "";
                var entity = scene.createEntity("screen");
                var textComponent = new Text(new NezSpriteFont(Assets.AssetLoader.GetFont("monitor")),
                    text, new Vector2(screen.position.X + screen.width / 2, screen.position.Y), Color.White);
                textComponent.horizontalOrigin = HorizontalAlign.Center;
                entity.addComponent(textComponent);

                if (props.ContainsKey("set_key") && !string.IsNullOrWhiteSpace(props["set_key"]))
                {
                    var listener = entity.addComponent(new MapEventListener(props["set_key"])
                    {
                        EventTriggered = mapEvent =>
                        {
                            textComponent.text = (string)mapEvent.Parameters[0];
                        }
                    });
                    MapEventListeners.Add(listener);
                }
            }

        }

        private void SetupMapEvents(TiledObjectGroup mapObjects)
        {
            foreach (var eventEmitter in mapObjects.objectsWithName(TiledObjects.EventEmitter))
            {
                var bounds = new RectangleF(eventEmitter.position, new Vector2(eventEmitter.width, eventEmitter.height));
                var props = eventEmitter.properties;
                var type = props["type"];
                var key = props["key"];
                var global = props.ContainsKey("global") ? bool.Parse(props["global"]) : false;

                MapEventEmitter emitter = null;
                switch (type)
                {
                    case "timed":
                        float.TryParse(props["interval_min"], out float intervalMin);
                        float.TryParse(props["interval_max"], out float intervalMax);
                        int.TryParse(props["repeat"], out int repeat);

                        emitter = scene.addEntity(new TimedEventEmitter(this, key, intervalMin, intervalMax, repeat));
                        break;
                    case "collision":
                        int.TryParse(props["physics_layer"], out int physicsLayer);
                        emitter = scene.addEntity(new CollisionEventEmitter(this, key, bounds, physicsLayer));
                        break;
                    case "interact":
                        emitter = scene.addEntity(new InteractEventEmitter(this, key, bounds));
                        break;
                    default:
                        Console.Error.WriteLine($"MapEventEmitter of type {type} not recognized!");
                        break;
                }

                if (global)
                {
                    emitter.EmitGlobally = true;
                }
            }
        }

        private void SetupPlayerSpawns(TiledObjectGroup mapObjects)
        {
            _playerSpawner = new PlayerSpawner();
            foreach (var spawnObject in mapObjects.objectsWithName(TiledObjects.PlayerSpawn))
            {
                _playerSpawner.AddLocation("player_spawner" + spawnObject.id, spawnObject.x + spawnObject.height / 2, spawnObject.y + spawnObject.height / 2);
            }
        }

        private void SetupItemSpawners(TiledObjectGroup objectGroup)
        {
            foreach (var spawnObject in objectGroup.objectsWithName(TiledObjects.ItemSpawn))
            {
                var spawner = scene.addEntity(new Spawner(spawnObject.x + spawnObject.height / 2, spawnObject.y + spawnObject.height / 2));

                // Example event listener for spawning weapons usage
                //var props = spawnObject.properties;
                //if (props.ContainsKey("key") && !string.IsNullOrWhiteSpace(props["key"]))
                //{
                //    var listener = spawner.addComponent(new MapEventListener(props["key"])
                //    {
                //        EventTriggered = _ => spawner.SpawnItem()
                //    });
                //    MapEventListeners.Add(listener);
                //}
                // Example event listener usage
            }
        }

        private void SetupPits(TiledObjectGroup objectGroup)
        {
            foreach (var pit in objectGroup.objectsWithName(TiledObjects.Pit))
            {
                var pitEntity = scene.createEntity("pit" + pit.id, new Vector2((pit.x + pit.width / 2), pit.y + pit.height / 2));
                pitEntity.setTag(Tags.Pit);
                var hitbox = pitEntity.addComponent(new BoxCollider(pit.width, pit.height));
                hitbox.isTrigger = true;
                Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);
            }
        }

        private void SetupCollisions(TiledObjectGroup objectGroup)
        {
            foreach (var collisionObject in objectGroup.objectsWithName(TiledObjects.Collision))
            {
                var collidable = scene.createEntity("collidable" + collisionObject.id, new Vector2((collisionObject.x + collisionObject.width / 2), collisionObject.y + collisionObject.height / 2));
                collidable.tag = Tags.Obstacle;
                var hitbox = collidable.addComponent(new BoxCollider(collisionObject.width, collisionObject.height));
                Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);
            }
        }

        private void CustomizeTiles(TiledMapComponent mapComponent)
        {
            var tileSize = new Vector2(mapComponent.tiledMap.tileWidth, mapComponent.tiledMap.tileHeight);
            for (float x = 0; x < mapComponent.width; x += tileSize.X)
            {
                for (float y = 0; y < mapComponent.height; y += tileSize.X)
                {
                    var tilePos = new Vector2(x, y);
                    var tile = mapComponent.getTileAtWorldPosition(tilePos);
                    CustomizeTile(tile, tilePos, tileSize);
                }
            }
        }

        private void CustomizeTile(TiledTile tile, Vector2 pos, Vector2 size)
        {
            if (tile == null)
            {
                return;
            }

            var properties = tile?.tilesetTile?.properties;
            if (properties == null) return;
            if (properties.ContainsKey(TiledProperties.EmitsLight))
            {
                var entity = scene.createEntity("world-light", pos + size);
                entity.setScale(0.55f);

                var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask")));
                sprite.material = Material.blendLighten();
                sprite.color = new Color(Color.White, 0.4f);
                sprite.renderLayer = Layers.Lights;
            }
        }

        public SceneComponent GetWeatherEffect(string name)
        {
            switch (name)
            {
                case "snowstorm":
                    return new Snowstorm();
                case "dungeongloom":
                    return new DungeonGloom();
                default:
                    Console.WriteLine("Weather effect '" + name + "' not found");
                    return null;
            }
        }

        private void ApplyWeather(TiledMap tiledmap)
        {
            try
            {
                var weatherAttribute = tiledmap.properties["weather"];
                if (weatherAttribute != null && weatherAttribute != "")
                {
                    scene.addSceneComponent(GetWeatherEffect(weatherAttribute));
                }
            }
            catch (KeyNotFoundException) { }
        }
    }
}
