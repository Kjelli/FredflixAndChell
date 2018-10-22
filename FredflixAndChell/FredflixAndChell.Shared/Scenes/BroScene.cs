using Nez;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Assets;

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
            playerEntity.addComponent(new Player(Screen.width / 2, Screen.height / 2));

            camera.position = playerEntity.position;
            camera.setMinimumZoom(12);
            camera.setZoom(16);
            camera.setMaximumZoom(16);

            addRenderer(new DefaultRenderer(renderOrder: -1, camera: camera));
        }
    }
}
