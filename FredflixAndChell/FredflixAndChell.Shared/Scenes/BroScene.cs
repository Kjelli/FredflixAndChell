﻿using Nez;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;
using System;
using static FredflixAndChell.Shared.Assets.Constants;
using Nez.Textures;
using Nez.Tiled;
using Nez.DeferredLighting;
using Nez.Shadows;
using FredflixAndChell.Shared.Utilities.Graphics.Cameras;

namespace FredflixAndChell.Shared.Scenes
{
    public class BroScene : Scene
    {
        public override void initialize()
        {
            base.initialize();

            AssetLoader.Load(content);

            setDesignResolution(1280, 720, SceneResolutionPolicy.BestFit);
            Screen.setSize(1280, 720);

            SetupRenderering();
            SetupMap();

            addSceneComponent(new SmoothCamera());
            addSceneComponent(new PlayerConnector());

            //Core.debugRenderEnabled = true;
        }

        private void SetupMap()
        {
            var tiledEntity = createEntity("tiled-map-entity");
            var tiledmap = AssetLoader.GetMap("winter_1");

            var tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap, "Collision"));
            tiledMapComponent.layerIndicesToRender = new int[] { 5, 2, 1, 0 };
            tiledMapComponent.renderLayer = Layers.MapBackground;
            tiledMapComponent.setMaterial(Material.stencilWrite(Stencils.EntityShadowStencil));
            var obj = tiledmap.getObjectGroup("Objects").objectsWithName("collision");
            
            foreach( var o in obj)
            {
                var collidable = createEntity("collidable" + o.id, new Vector2((o.x + o.width / 2), o.y + o.height / 2));
                var hitbox = collidable.addComponent(new BoxCollider(o.width, o.height));
                Flags.setFlagExclusive(ref hitbox.collidesWithLayers, Layers.MapObstacles);

                Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);

            }

            var tiledMapDetailsComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap));
            tiledMapDetailsComponent.layerIndicesToRender = new int[] { 3, 4 };
            tiledMapDetailsComponent.renderLayer = Layers.MapForeground;
            tiledMapDetailsComponent.setMaterial(Material.stencilWrite(Stencils.HiddenEntityStencil));
            //tiledMapDetailsComponent.material.effect = content.loadNezEffect<SpriteAlphaTestEffect>();
            

            //CustomizeTiles(tiledMapComponent);
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

            // Rendering all layers but lights
            var renderLayerExcludeRenderer = addRenderer(new RenderLayerExcludeRenderer(0,
                Layers.Lights, Layers.Lights2));

            // Rendering lights
            var lightRenderer = addRenderer(new RenderLayerRenderer(1,
                Layers.Lights, Layers.Lights2));
            lightRenderer.renderTexture = new RenderTexture();
            lightRenderer.renderTargetClearColor = new Color(150, 150, 150, 255);

            // Postprocessor effects for lighting
            var spriteLightPostProcessor = addPostProcessor(new SpriteLightPostProcessor(2, lightRenderer.renderTexture));

            var bloomPostProcessor = addPostProcessor(new BloomPostProcessor(3));
            bloomPostProcessor.settings = BloomSettings.presetSettings[5];

            var vignette = addPostProcessor(new VignettePostProcessor(4));
        }
    }
}
