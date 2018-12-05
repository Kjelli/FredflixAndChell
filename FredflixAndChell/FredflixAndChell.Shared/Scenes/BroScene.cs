using FredflixAndChell.Components.Players;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Effects.Weather;
using FredflixAndChell.Shared.Components.HUD;
using FredflixAndChell.Shared.Components.PlayerComponents;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Maps;
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

        public BroScene()
        {
        }

        public override void initialize()
        {
            Screen.isFullscreen = true;
            AssetLoader.LoadBroScene(content);
            setDesignResolution(ScreenWidth, ScreenHeight, SceneResolutionPolicy.ShowAll);

            InitializePlayerScores();

            var map = addEntity(new Map());
            map.Setup();

            addSceneComponent(new SmoothCamera());
            addSceneComponent(new HUD());
            var connector = addSceneComponent(new PlayerConnector(spawnLocations: map.PlayerSpawner));
            connector.SpawnDebugPlayer();
            addSceneComponent(new GameSystem());
            addSceneComponent(new ControllerSystem());


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
            lightRenderer.renderTargetClearColor = new Color(80, 80, 80, 255);

            // Postprocessor effects for lighting
            var spriteLightPostProcessor = addPostProcessor(new SpriteLightPostProcessor(2, lightRenderer.renderTexture));

            // Render screenspace
            _screenSpaceRenderer = new ScreenSpaceRenderer(100, Layers.HUD);
            _screenSpaceRenderer.shouldDebugRender = false;
            addRenderer(_screenSpaceRenderer);

            // Letterbox effect when a winner is determined
            LetterBox = addPostProcessor(new CinematicLetterboxPostProcessor(3));
        }
    }
}
