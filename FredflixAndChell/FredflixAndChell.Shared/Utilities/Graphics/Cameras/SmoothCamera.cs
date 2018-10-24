using Microsoft.Xna.Framework;
using Nez;

namespace FredflixAndChell.Shared.Scenes
{
    internal class SmoothCamera : SceneComponent
    {
        private Entity playerEntity;
        private Camera camera;
        private TiledMapComponent _map;

        public SmoothCamera(Entity playerEntity)
        {
            this.playerEntity = playerEntity;
        }

        public override void onEnabled()
        {
            base.onEnabled();
            camera = playerEntity.scene.camera;
            camera.position = playerEntity.position;
            _map = scene.findEntity("tiled-map-entity").getComponent<TiledMapComponent>();
            
        }

        public override void update()
        {
            base.update();
            camera.position = camera.position*0.9925f + 0.0075f*playerEntity.position;

            if (camera.bounds.left < 0) camera.position = new Vector2(camera.bounds.width/2, camera.position.Y);
            if (camera.bounds.top < 0) camera.position = new Vector2(camera.position.X, camera.bounds.height/2);
            if (camera.bounds.right > _map.bounds.right) camera.position = new Vector2(_map.bounds.right - camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.bottom > _map.bounds.bottom) camera.position = new Vector2(camera.position.X, _map.bounds.bottom - camera.bounds.height/2);
        }
    }
}