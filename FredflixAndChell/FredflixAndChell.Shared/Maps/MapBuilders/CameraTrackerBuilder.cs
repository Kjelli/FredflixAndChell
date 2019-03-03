using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Maps.Events;
using Nez;
using Nez.Tiled;
using System;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class CameraTrackerBuilder
    {
        public static void BuildCameraTrackers(this Map map, TiledObjectGroup objectGroup)
        {
            foreach (var cameraTracker in objectGroup.objectsWithName(TiledObjects.CameraTracker))
            {
                var entity = map.scene.createEntity("camera_tracker");
                var camTracker = entity.addComponent(new CameraTracker(() => true));
                entity.setPosition(cameraTracker.x + cameraTracker.width / 2, cameraTracker.y + cameraTracker.height / 2);
                var props = cameraTracker.properties;

                if (props.ContainsKey("initial_state") && !string.IsNullOrWhiteSpace(props["initial_state"]))
                {
                    if (bool.TryParse(props["initial_state"], out bool enabled))
                    {
                        camTracker.enabled = enabled;
                    }
                }

                if (props.ContainsKey("priority") && !string.IsNullOrWhiteSpace(props["priority"]))
                {
                    if (int.TryParse(props["priority"], out int priority))
                    {
                        camTracker.Priority = priority;
                    }
                }

                if (props.ContainsKey("toggle_key") && !string.IsNullOrWhiteSpace(props["toggle_key"]))
                {
                    var listener = entity.addComponent(new MapEventListener(props["toggle_key"])
                    {
                        EventTriggered = e =>
                        {
                            if (e.Parameters?.Length > 0 && e.Parameters[0] is bool enabled)
                            {
                                camTracker.setEnabled(enabled);
                            }
                            else
                            {
                                camTracker.setEnabled(!entity.enabled);
                            }
                            Console.WriteLine("CameraTracker " + (camTracker.enabled ? "enabled" : "disabled"));
                        }
                    });
                    map.MapEventListeners.Add(listener);
                }
            }
        }

    }
}
