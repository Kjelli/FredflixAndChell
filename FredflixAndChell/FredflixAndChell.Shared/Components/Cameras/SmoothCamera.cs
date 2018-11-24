using FredflixAndChell.Shared.GameObjects.Players;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using System.Linq;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Cameras
{
    public class SmoothCamera : SceneComponent
    {
        private bool _winMode = false;
        private float _baseZoom = 10f;

        private Camera camera;
        private TiledMapComponent _map;
        private List<CameraTracker> _trackers;

        public float BaseZoom { get => _baseZoom; set => _baseZoom = value; }

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
            float paddingX = 64, paddingY = 64;

            bool anyToTrack = false;
            foreach(var tracker in _trackers)
            {
                if (!tracker.enabled 
                    || !tracker.ShouldTrackEntity()
                    || (_winMode && tracker.entity as Player == null)) continue;
                left = Math.Min(tracker.Position.X - paddingX / 2, left);
                right = Math.Max(tracker.Position.X + paddingX / 2, right);
                top = Math.Min(tracker.Position.Y - paddingY / 2, top);
                bottom = Math.Max(tracker.Position.Y + paddingY / 2, bottom);
                anyToTrack = true;
            }
            if (!anyToTrack) return;

            var targetWidth = Math.Max(ScreenWidth, (right - left) * _baseZoom);
            var targetHeight = Math.Max(ScreenHeight, (bottom - top) * _baseZoom);
            var zoom = _baseZoom * Math.Min(ScreenWidth / targetWidth, ScreenHeight / targetHeight);
            var center = new Vector2(left + (right - left) / 2, top + (bottom - top) / 2);

            camera.rawZoom = Lerps.lerpTowards(camera.rawZoom, zoom, 0.75f, Time.deltaTime * 10f);
            camera.position = Lerps.lerpTowards(camera.position, center, 0.25f, Time.deltaTime * 10f);

            if (camera.bounds.left < 0) camera.position = new Vector2(camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.top < 0) camera.position = new Vector2(camera.position.X, camera.bounds.height / 2);
            if (camera.bounds.right > _map.bounds.right) camera.position = new Vector2(_map.bounds.right - camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.bottom > _map.bounds.bottom) camera.position = new Vector2(camera.position.X, _map.bounds.bottom - camera.bounds.height / 2);
        }

        public void SetWinMode(bool winMode)
        {
            _winMode = winMode;
        }
    }
}