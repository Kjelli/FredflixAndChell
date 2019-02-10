using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Props;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Utilities.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class MonitorBuilder
    {
        public static void BuildMonitors(this Map map, TiledObjectGroup mapObjects)
        {
            foreach (var screen in mapObjects.objectsWithName(TiledObjects.Monitor))
            {
                var entity = CreateMonitorEntity(map, screen);
                map.scene.addEntity(entity);
            }
        }


        private static Entity CreateMonitorEntity(Map map, TiledObject tiledMonitor)
        {
            var props = tiledMonitor.properties;
            var text = props.ContainsKey("text") ? props["text"] : "";
            var monitor = new Monitor(tiledMonitor.position, new Vector2(tiledMonitor.width, tiledMonitor.height));

            if (props.ContainsKey("set_key") && !string.IsNullOrWhiteSpace(props["set_key"]))
            {
                var listener = monitor.addComponent(new MapEventListener(props["set_key"])
                {
                    EventTriggered = mapEvent =>
                    {
                        monitor.Text = (string)mapEvent.Parameters[0];
                    }
                });
                map.MapEventListeners.Add(listener);
            }

            return monitor;
        }

       
    }
}