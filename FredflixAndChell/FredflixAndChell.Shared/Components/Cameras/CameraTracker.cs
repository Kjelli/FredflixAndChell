using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Components.Cameras
{
    public class CameraTracker : Component
    {
        private Func<bool> _shouldTrackEntity;
        private int _priority;

        public Func<bool> ShouldTrackEntity =>
            _shouldTrackEntity ?? (_shouldTrackEntity = Always);
        public int Priority { get; set; }

        public CameraTracker() { }

        public CameraTracker(Func<bool> shouldTrackEntityFunction, int priority = 0)
        {
            _shouldTrackEntity = shouldTrackEntityFunction;
            _priority = priority;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            var cam = entity.scene.getSceneComponent<SmoothCamera>();
            cam.Register(this);
        }

        public override void onRemovedFromEntity()
        {
            base.onRemovedFromEntity();
            var cam = entity.scene.getSceneComponent<SmoothCamera>();
            cam.Unregister(this);
        }

        private bool Always()
        {
            return true;
        }

        public Vector2 Position => entity?.position ?? Vector2.Zero;
    }
}
