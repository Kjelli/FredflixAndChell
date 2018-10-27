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
            var players = scene.findEntitiesWithTag(Tags.Player);
            if (players.Count == 0) return;

            var centerPoint = new Vector2(players.Average(p => p.position.X), players.Average(p => p.position.Y));
            camera.position = camera.position*0.925f + 0.075f* centerPoint;

            if (camera.bounds.left < 0) camera.position = new Vector2(camera.bounds.width/2, camera.position.Y);
            if (camera.bounds.top < 0) camera.position = new Vector2(camera.position.X, camera.bounds.height/2);
            if (camera.bounds.right > _map.bounds.right) camera.position = new Vector2(_map.bounds.right - camera.bounds.width / 2, camera.position.Y);
            if (camera.bounds.bottom > _map.bounds.bottom) camera.position = new Vector2(camera.position.X, _map.bounds.bottom - camera.bounds.height/2);
        }
    }
}