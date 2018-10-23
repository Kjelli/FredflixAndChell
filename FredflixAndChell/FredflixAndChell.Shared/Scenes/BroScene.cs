using Nez;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;
using System;

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

            var playerEntity = createEntity("player", new Vector2(Screen.width / 2, Screen.height / 2));

            //TODO Remove this: testing only for controllaz
            for (var i = 0; i < 4; i++)
            {
                if (GamePad.GetCapabilities(GamePadUtility.ConvertToIndex(i + 1)).IsConnected)
                {
                    Console.WriteLine($"Gamepad {i + 1} Detected - Generating player");
                    playerEntity.addComponent(new Player(Screen.width / 2+5, Screen.height / 2 + 5));
                }
            }

            playerEntity.addComponent(new Player(Screen.width / 2, Screen.height / 2));

            camera.position = playerEntity.position;
            camera.setMinimumZoom(12);
            camera.setZoom(16);
            camera.setMaximumZoom(16);

            addRenderer(new DefaultRenderer(renderOrder: -1, camera: camera));
        }
    }
}
