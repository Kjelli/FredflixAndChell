using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Utilities.Graphics.Cameras
{
    internal class SmoothCamera : SceneComponent
    {
        private Camera camera;
        private TiledMapComponent _map;

        public SmoothCamera()
        {
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

        private void CenterCamera()
        {
            var entitiesToTrack = new List<Entity>();
            var playerEntities = scene.findEntitiesWithTag(Tags.Player);
            var collectibleEntities = scene.findEntitiesWithTag(Tags.Collectible);

            entitiesToTrack.AddRange(playerEntities);
            entitiesToTrack.AddRange(collectibleEntities);

            if (entitiesToTrack.Count == 0) return;

            float left = 10000, 
                right = -10000, 
                top = 10000, 
                bottom = -10000;
            float paddingX = 32, paddingY = 32;

            foreach(var entity in entitiesToTrack)
            {
                if (!entity.enabled || float.IsNaN(entity.position.X) || float.IsNaN(entity.position.X)) continue;
                left = Math.Min(entity.position.X - paddingX / 2, left);
                right = Math.Max(entity.position.X + paddingX / 2, right);
                top = Math.Min(entity.position.Y - paddingY / 2, top);
                bottom = Math.Max(entity.position.Y + paddingY / 2, bottom);
            }

            var baseZoom = 10f;
            var targetWidth = Math.Max(ScreenWidth, (right - left) * baseZoom);
            var targetHeight = Math.Max(ScreenHeight, (bottom - top) * baseZoom);
            var zoom = baseZoom * Math.Min(ScreenWidth / targetWidth, ScreenHeight / targetHeight);
            var center = new Vector2(left + (right - left) / 2, top + (bottom - top) / 2);
            Console.WriteLine($"zoom: {zoom}, center: {center}" +
                $"\nbounds: {right - left},{bottom - top}" +
                $"\ntargetWidth: {targetWidth}");

            camera.rawZoom = camera.rawZoom * 0.925f + 0.075f * zoom;
            camera.position = camera.position * 0.925f + 0.075f * center;

            if (camera.bounds.left < 0) camera.position = new Vector2(camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.top < 0) camera.position = new Vector2(camera.position.X, camera.bounds.height / 2);
            if (camera.bounds.right > _map.bounds.right) camera.position = new Vector2(_map.bounds.right - camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.bottom > _map.bounds.bottom) camera.position = new Vector2(camera.position.X, _map.bounds.bottom - camera.bounds.height / 2);
        }
    }
}