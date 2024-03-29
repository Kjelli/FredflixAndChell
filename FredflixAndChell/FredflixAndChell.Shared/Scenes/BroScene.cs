﻿using FredflixAndChell.Components.Players;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.HUD;
using FredflixAndChell.Shared.Maps;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Scenes
{
    public class BroScene : Scene
    {
        private readonly GameSettings _gameSettings;
        private ScreenSpaceRenderer _screenSpaceRenderer;

        public CinematicLetterboxPostProcessor LetterBox { get; private set; }

        public BroScene(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            Setup();
        }

        public override void initialize()
        {
            Screen.isFullscreen = true;
            AssetLoader.LoadBroScene(content);
            setDesignResolution(ScreenWidth, ScreenHeight, SceneResolutionPolicy.ShowAll);

            SetupRenderering();
        }

        public void Setup()
        {
            InitializePlayerScores();

            var map = addEntity(new Map());
            map.Setup(_gameSettings.Map);

            addSceneComponent(new SmoothCamera());
            addSceneComponent(new HUD());
            var connector = addSceneComponent(new PlayerConnector(spawnLocations: map.PlayerSpawner));
#if DEBUG
            connector.SpawnDebugPlayer();
            connector.SpawnDebugPlayer();
#endif
            var gameSystem = new GameSystem(_gameSettings, map);
            addSceneComponent(gameSystem);
            addSceneComponent(new ControllerSystem());

            // TODO turn back on for sweet details. Sweetails.
            addEntity(new DebugHud());
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
