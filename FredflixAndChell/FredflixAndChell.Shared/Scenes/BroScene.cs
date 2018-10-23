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

            var tiledEntity = createEntity("tiled-map-entity");
            var tiledmap = AssetLoader.GetMap("firstlevel");
            var tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap, "Obstacles"));
            tiledMapComponent.layerIndicesToRender = new int[] { 2,1,0 };
            tiledMapComponent.renderLayer = 10;

            var tiledMapDetailsComp = tiledEntity.addComponent(new TiledMapComponent(tiledmap));
            tiledMapDetailsComp.layerIndicesToRender = new int[] { 3 };
            tiledMapDetailsComp.renderLayer = -10;

            var playerEntity = createEntity("player");
            playerEntity.addComponent(new Player((int) tiledMapComponent.width / 2, (int)tiledMapComponent.height / 2));

            playerEntity.addComponent(new FollowCamera(playerEntity));

            camera.position = playerEntity.position;
            camera.setMinimumZoom(2);
            camera.setMaximumZoom(6);
            camera.setZoom(6);

            addRenderer(new DefaultRenderer(renderOrder: -1, camera: camera));
        }
    }
}
