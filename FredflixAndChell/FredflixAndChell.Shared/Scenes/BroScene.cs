using Nez;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;
using System;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Scenes
{
    public class BroScene : Scene
    {
        public override void initialize()
        {
            base.initialize();

            AssetLoader.Load(content);

            // default to 1280x720 with no SceneResolutionPolicy
            setDesignResolution(1280, 720, Scene.SceneResolutionPolicy.BestFit);
            Screen.setSize(1280, 720);

            var tiledEntity = createEntity("tiled-map-entity");
            var tiledmap = AssetLoader.GetMap("firstlevel");
            var tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap, "Obstacles"));
            tiledMapComponent.setMaterial(Material.stencilWrite(Stencils.EntityShadowStencil));
            tiledMapComponent.layerIndicesToRender = new int[] { 2,1,0 };
            tiledMapComponent.renderLayer = Layers.MapBackground;

            var tiledMapDetailsComp = tiledEntity.addComponent(new TiledMapComponent(tiledmap));
            tiledMapDetailsComp.setMaterial(Material.stencilWrite(Stencils.HiddenEntityStencil));
            tiledMapDetailsComp.layerIndicesToRender = new int[] { 3 };
            tiledMapDetailsComp.renderLayer = Layers.MapForeground;
            tiledMapDetailsComp.material.effect = content.loadNezEffect<SpriteAlphaTestEffect>();

            var playerEntity = createEntity("player");
            playerEntity.addComponent(new Player((int) tiledMapComponent.width / 2, (int)tiledMapComponent.height / 2));

            playerEntity.addComponent(new SmoothCamera(playerEntity));
            //var playerEntity = createEntity("player", new Vector2(Screen.width / 2, Screen.height / 2));

            ////TODO Remove this: testing only for controllaz
            //for (var i = 0; i < 4; i++)
            //{
            //    if (GamePad.GetCapabilities(GamePadUtility.ConvertToIndex(i + 1)).IsConnected)
            //    {
            //        Console.WriteLine($"Gamepad {i + 1} Detected - Generating player");
            //        playerEntity.addComponent(new Player(Screen.width / 2+5, Screen.height / 2 + 5));
            //    }
            //}

            //playerEntity.addComponent(new Player(Screen.width / 2, Screen.height / 2));

            camera.setMinimumZoom(2);
            camera.setMaximumZoom(6);
            camera.setZoom(6);

            addRenderer(new DefaultRenderer(renderOrder: -1, camera: camera));
        }
    }
}
