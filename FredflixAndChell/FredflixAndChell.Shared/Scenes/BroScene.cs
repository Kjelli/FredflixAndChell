﻿using FredflixAndChell.Components.Players;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Effects.Weather;
using FredflixAndChell.Shared.Components.HUD;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Scenes
{
    public class BroScene : Scene
    {
        private ScreenSpaceRenderer _screenSpaceRenderer;

        public CinematicLetterboxPostProcessor LetterBox { get; private set; }

        public PlayerSpawner PlayerSpawner { get; private set; }

        public BroScene()
        {
        }

        public override void initialize()
        {
            Screen.isFullscreen = true;
            AssetLoader.LoadBroScene(content);
            setDesignResolution(ScreenWidth, ScreenHeight, SceneResolutionPolicy.ShowAll);

            SetupMap();

            InitializePlayerScores();

            addSceneComponent(new SmoothCamera());
            addSceneComponent(new HUD());
            addSceneComponent(new PlayerConnector(spawnLocations: PlayerSpawner));
            addSceneComponent(new GameSystem());

            // TODO turn back on for sweet details. Sweetails.
            addEntity(new DebugHud());
            SetupRenderering();
        }

        private void InitializePlayerScores()
        {
            if (ContextHelper.PlayerScores == null)
            {
                ContextHelper.PlayerScores = new List<PlayerScore>();
            }
        }

        public override void unload()
        {
            base.unload();
            //AssetLoader.Dispose();
        }

        private void SetupMap()
        {
            var tiledEntity = createEntity("tiled-map-entity");

            var tiledmap = AssetLoader.GetMap(ContextHelper.CurrentMap);

            var tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap, "Collision"));
            tiledMapComponent.layerIndicesToRender = new int[] { 5, 2, 1, 0 };
            tiledMapComponent.renderLayer = Layers.MapBackground;
            tiledMapComponent.setMaterial(Material.stencilWrite(Stencils.EntityShadowStencil));
            var mapObjects = tiledmap.getObjectGroup("Objects");

            SetupMapObjects(mapObjects);

            var tiledMapDetailsComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap));
            tiledMapDetailsComponent.layerIndicesToRender = new int[] { 3, 4 };
            tiledMapDetailsComponent.renderLayer = Layers.MapForeground;
            tiledMapDetailsComponent.setMaterial(Material.stencilWrite(Stencils.HiddenEntityStencil));
            //tiledMapDetailsComponent.material.effect = content.loadNezEffect<SpriteAlphaTestEffect>();

            //CustomizeTiles(tiledMapComponent);

            //Weather
            ApplyWeather(tiledmap);
        }

        private void ApplyWeather(TiledMap tiledmap)
        {
            try
            {
                var weatherAttribute = tiledmap.properties["weather"];
                if (weatherAttribute != null && weatherAttribute != "")
                {
                    addSceneComponent(GetWeatherEffect(weatherAttribute));
                }
            }
            catch (KeyNotFoundException) { }
        }

        private void SetupMapObjects(TiledObjectGroup objectGroup)
        {
            foreach (var collisionObject in objectGroup.objectsWithName("collision"))
            {
                var collidable = createEntity("collidable" + collisionObject.id, new Vector2((collisionObject.x + collisionObject.width / 2), collisionObject.y + collisionObject.height / 2));
                var hitbox = collidable.addComponent(new BoxCollider(collisionObject.width, collisionObject.height));
                Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);
            }

            foreach (var pit in objectGroup.objectsWithName("pit"))
            {
                var pitEntity = createEntity("pit" + pit.id, new Vector2((pit.x + pit.width / 2), pit.y + pit.height / 2));
                pitEntity.setTag(Tags.Pit);
                var hitbox = pitEntity.addComponent(new BoxCollider(pit.width, pit.height));
                hitbox.isTrigger = true;
                Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);
            }

            foreach (var spawnObject in objectGroup.objectsWithName("item_spawn"))
            {
                addEntity(new Spawner(spawnObject.x + spawnObject.height / 2, spawnObject.y + spawnObject.height / 2));
            }

            PlayerSpawner = new PlayerSpawner();
            foreach (var spawnObject in objectGroup.objectsWithName("player_spawn"))
            {
                PlayerSpawner.AddLocation("player_spawner" + spawnObject.id, spawnObject.x + spawnObject.height / 2, spawnObject.y + spawnObject.height / 2);
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
            if (properties.ContainsKey(TileProperties.EmitsLight))
            {
                var entity = createEntity("world-light", pos + size);
                entity.setScale(0.55f);

                var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask")));
                sprite.material = Material.blendLighten();
                sprite.color = new Color(Color.White, 0.4f);
                sprite.renderLayer = Layers.Lights;
            }
        }

        private void SetupRenderering()
        {
            camera.setMinimumZoom(4);
            camera.setMaximumZoom(6);
            camera.setZoom(4);

            // Rendering all layers but lights and screenspace
            var renderLayerExcludeRenderer = addRenderer(new RenderLayerExcludeRenderer(0,
                Layers.Lights, Layers.Lights2, Layers.HUD));
            renderLayerExcludeRenderer.renderTargetClearColor = new Color(0, 0, 0);

            // Rendering lights
            var lightRenderer = addRenderer(new RenderLayerRenderer(1,
                Layers.Lights, Layers.Lights2));
            lightRenderer.renderTexture = new RenderTexture();
            lightRenderer.renderTargetClearColor = new Color(180, 180, 180, 255);

            // Postprocessor effects for lighting
            var spriteLightPostProcessor = addPostProcessor(new SpriteLightPostProcessor(2, lightRenderer.renderTexture));

            // Render screenspace
            _screenSpaceRenderer = new ScreenSpaceRenderer(100, Layers.HUD);
            _screenSpaceRenderer.shouldDebugRender = false;
            addRenderer(_screenSpaceRenderer);

            // Letterbox effect when a winner is determined
            LetterBox = addPostProcessor(new CinematicLetterboxPostProcessor(4));

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


    }
}
