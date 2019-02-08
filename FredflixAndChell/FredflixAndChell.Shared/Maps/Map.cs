using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Effects.Weather;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Maps.MapBuilders;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
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

            ApplyWeather(_tiledMap);
        }

        private void SetupMapObjects()
        {
            var mapObjects = _tiledMap.getObjectGroup(TiledObjects.ObjectGroup);

            this.BuildCollisionZones(mapObjects);
            this.BuildPits(mapObjects);
            this.BuildItemSpawns(mapObjects);
            this.BuildEventEmitters(mapObjects);
            this.BuildMonitors(mapObjects);
            this.BuildLightSources(mapObjects);
            this.BuildCameraTrackers(mapObjects);

            SetupPlayerSpawns(mapObjects);

        }

        private void SetupPlayerSpawns(TiledObjectGroup mapObjects)
        {
            _playerSpawner = new PlayerSpawner();
            foreach (var spawnObject in mapObjects.objectsWithName(TiledObjects.PlayerSpawn))
            {
                _playerSpawner.AddLocation("player_spawner" + spawnObject.id, spawnObject.x + spawnObject.height / 2, spawnObject.y + spawnObject.height / 2);
            }
        }

        /* Unused tile-based lightsources
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
        */

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
