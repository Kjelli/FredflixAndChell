using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Props;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Systems.GameModeHandlers;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class ZoneBuilder
    {
        public static void BuildZones(this Map map, TiledObjectGroup objectGroup)
        {
            foreach (var zoneObject in objectGroup.objectsWithType(TiledObjects.Zone))
            {
                var props = zoneObject.properties;

                if (props.ContainsKey("ctf_only") && props["ctf_only"] != null)
                {
                    var ctfOnly = bool.Parse(props["ctf_only"]);
                    var currentGameMode = ContextHelper.GameSettings.GameMode;
                    if (ctfOnly && currentGameMode != GameMode.CTF)
                    {
                        continue;
                    }

                    var position = zoneObject.position;
                    var collectible = new Collectible(position.X, position.Y, "flag", false);

                    // TODO
                    collectible.OnPickupEvent += _ => { };
                }

                var color = new Color();
                var glow = false;
                var size = new Vector2(zoneObject.width, zoneObject.height);

                if (props.ContainsKey("color") && props["color"] != null)
                {
                    color = ColorExt.hexToColor(props["color"].Substring(3));
                }

                if (props.ContainsKey("glow") && props["glow"] != null)
                {
                    bool.TryParse(props["glow"], out glow);
                }

                var zone = new Zone(size, color, glow);
                zone.position = zoneObject.position;

                if (props.ContainsKey("key") && !string.IsNullOrWhiteSpace(props["key"]))
                {
                    var listener = zone.addComponent(new MapEventListener(props["key"])
                    {
                        EventTriggered = mapEvent =>
                        {
                            var targetColor = Color.Purple;
                            var targetColorString = (string)mapEvent.Parameters[0];
                            if (props.ContainsKey(targetColorString) && props[targetColorString] != null)
                            {
                                targetColor = ColorExt.hexToColor(props[targetColorString].Substring(3));
                            }
                            zone.ZoneColor = targetColor;
                        }
                    });
                    map.MapEventListeners.Add(listener);
                }

                map.scene.addEntity(zone);
            }
        }
    }
}
