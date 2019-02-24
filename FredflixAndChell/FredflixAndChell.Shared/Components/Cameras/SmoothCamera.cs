using FredflixAndChell.Shared.GameObjects.Players;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Cameras
{
    public class SmoothCamera : SceneComponent
    {
        private bool _winMode = false;
        private float _baseZoom = 10f;

        private Camera _camera;
        private Camera _reflectionCamera;

        private TiledMapComponent _map;
        private List<CameraTracker> _trackers;
        private Vector2 _reflectiveOffset = new Vector2(8, 8);

        public float BaseZoom { get => _baseZoom; set => _baseZoom = value; }
        public float Zoom
        {
            get => _camera.rawZoom;
            set
            {
                _camera.rawZoom = value;
                _reflectionCamera.rawZoom = value;
            }
        }

        public Vector2 Position
        {
            get => _camera.position;
            set
            {
                _camera.position = value;
                _reflectionCamera.position = value + _reflectiveOffset;
            }
        }

        public SmoothCamera(Camera reflectionCamera)
        {
            _reflectionCamera = reflectionCamera;
            _trackers = new List<CameraTracker>();
        }

        public override void onEnabled()
        {
            base.onEnabled();
            _camera = scene.camera;
            _map = scene.findEntity(TiledObjects.TiledMapEntity).getComponent<TiledMapComponent>();
            _camera.position = new Vector2(_map.width / 2, _map.height / 2);
        }

        public override void update()
        {
            base.update();
            CenterCamera();
#if DEBUG
            DebugZoom();
#endif
        }

        private void DebugZoom()
        {
            _baseZoom = Math.Max(Math.Min(10, _baseZoom + Input.mouseWheelDelta / 200.0f), 2.5f);
        }

        public void Register(CameraTracker cameraTracker)
        {
            _trackers.Add(cameraTracker);
            SortByPriority();
        }

        private void SortByPriority()
        {
            _trackers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public void Unregister(CameraTracker cameraTracker)
        {
            _trackers.Remove(cameraTracker);
            SortByPriority();
        }

        private void CenterCamera()
        {
            if (_trackers.Count == 0)
                return;

            float left = 10000,
                right = -10000,
                top = 10000,
                bottom = -10000;
            float paddingX = 64, paddingY = 64;

            bool anyToTrack = false;
            int lastTrackerPriority = -1;
            foreach (var tracker in _trackers)
            {
                if (!tracker.enabled
                    || !tracker.ShouldTrackEntity()
                    || (_winMode && tracker.entity as Player == null)) continue;
                if (tracker.Priority < lastTrackerPriority) break;

                lastTrackerPriority = tracker.Priority;

                left = Math.Min(tracker.Position.X - paddingX / 2, left);
                right = Math.Max(tracker.Position.X + paddingX / 2, right);
                top = Math.Min(tracker.Position.Y - paddingY / 2, top);
                bottom = Math.Max(tracker.Position.Y + paddingY / 2, bottom);
                anyToTrack = true;
            }
            if (!anyToTrack)
            {
                Zoom = Lerps.lerpTowards(_camera.rawZoom, 2.7f, 0.95f, Time.deltaTime * 10f);
                Position = Lerps.lerpTowards(_camera.position, new Vector2(_map.bounds.right / 2, _map.bounds.bottom / 2), 0.95f, Time.deltaTime * 10f);
                return;
            }
            var targetWidth = Math.Max(ScreenWidth, (right - left) * _baseZoom);
            var targetHeight = Math.Max(ScreenHeight, (bottom - top) * _baseZoom);
            var zoom = _baseZoom * Math.Min(ScreenWidth / targetWidth, ScreenHeight / targetHeight);
            var center = new Vector2(left + (right - left) / 2, top + (bottom - top) / 2);

            Zoom = Lerps.lerpTowards(_camera.rawZoom, zoom, 0.75f, Time.deltaTime * 10f);

            Position = Lerps.lerpTowards(_camera.position, center, 0.25f, Time.deltaTime * 10f);

            // Hack to avoid weird camera stopping if players are still
            _camera.position += new Vector2(1f, 0);
            _camera.position += new Vector2(-1f, 0);

            if (_camera.bounds.left < 0) _camera.position = new Vector2(_camera.bounds.width / 2, _camera.position.Y);
            if (_camera.bounds.top < 0) _camera.position = new Vector2(_camera.position.X, _camera.bounds.height / 2);
            if (_camera.bounds.right > _map.bounds.right) _camera.position = new Vector2(_map.bounds.right - _camera.bounds.width / 2, _camera.position.Y);
            if (_camera.bounds.bottom > _map.bounds.bottom) _camera.position = new Vector2(_camera.position.X, _map.bounds.bottom - _camera.bounds.height / 2);
        }

        public void SetWinMode(bool winMode)
        {
            _winMode = winMode;
        }
    }
}