using FredflixAndChell.Shared.GameObjects.Props;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class ZoneBuilder
    {
        public static void BuildZones(this Map map, TiledObjectGroup objectGroup)
        {
            foreach (var zoneObject in objectGroup.objectsWithName(TiledObjects.Zone))
            {
                var color = new Color();
                var glow = false;
                var size = new Vector2(zoneObject.width, zoneObject.height);

                var props = zoneObject.properties;
                if(props.ContainsKey("color") && props["color"] != null)
                {
                    color = ColorExt.hexToColor("#" + props["color"].Substring(3));
                }

                if (props.ContainsKey("glow") && props["glow"] != null)
                {
                    bool.TryParse(props["glow"], out glow);
                }

                var zone = new Zone(size, color, glow);
                zone.position = zoneObject.position;

                map.scene.addEntity(zone);
            }
        }
    }
}
