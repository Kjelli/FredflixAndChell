using Nez;

namespace FredflixAndChell.Shared.Scenes
{
    internal class SmoothCamera : Component, IUpdatable
    {
        private Entity playerEntity;
        private Camera camera;

        public SmoothCamera(Entity playerEntity)
        {
            this.playerEntity = playerEntity;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            camera = playerEntity.scene.camera;
        }

        public void update()
        {
            camera.position = playerEntity.position;
        }
    }
}