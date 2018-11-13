using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Cameras
{
    public class SmoothCamera : SceneComponent
    {
        private Camera camera;
        private TiledMapComponent _map;
        private List<CameraTracker> _trackers;

        public SmoothCamera()
        {
            _trackers = new List<CameraTracker>();
        }

        public override void onEnabled()
        {
            base.onEnabled();
            camera = scene.camera;
            _map = scene.findEntity("tiled-map-entity").getComponent<TiledMapComponent>();
        }

        public override void update()
        {
            base.update();
            CenterCamera();
        }

        public void Register(CameraTracker cameraTracker)
        {
            _trackers.Add(cameraTracker);
        }

        public void Unregister(CameraTracker cameraTracker)
        {
            _trackers.Remove(cameraTracker);
        }

        private void CenterCamera()
        {
            if (_trackers.Count == 0) return;

            float left = 10000, 
                right = -10000, 
                top = 10000, 
                bottom = -10000;
            float paddingX = 32, paddingY = 32;

            foreach(var tracker in _trackers)
            {
                if (!tracker.enabled || !tracker.ShouldTrackEntity()) continue;
                left = Math.Min(tracker.Position.X - paddingX / 2, left);
                right = Math.Max(tracker.Position.X + paddingX / 2, right);
                top = Math.Min(tracker.Position.Y - paddingY / 2, top);
                bottom = Math.Max(tracker.Position.Y + paddingY / 2, bottom);
            }

            var baseZoom = 10f;
            var targetWidth = Math.Max(ScreenWidth, (right - left) * baseZoom);
            var targetHeight = Math.Max(ScreenHeight, (bottom - top) * baseZoom);
            var zoom = baseZoom * Math.Min(ScreenWidth / targetWidth, ScreenHeight / targetHeight);
            var center = new Vector2(left + (right - left) / 2, top + (bottom - top) / 2);

            camera.rawZoom = camera.rawZoom * 0.975f + 0.025f * zoom;
            camera.position = camera.position * 0.925f + 0.075f * center;

            if (camera.bounds.left < 0) camera.position = new Vector2(camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.top < 0) camera.position = new Vector2(camera.position.X, camera.bounds.height / 2);
            if (camera.bounds.right > _map.bounds.right) camera.position = new Vector2(_map.bounds.right - camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.bottom > _map.bounds.bottom) camera.position = new Vector2(camera.position.X, _map.bounds.bottom - camera.bounds.height / 2);
        }
    }
}