using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Maps.Events;
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
    public static class LightsourceBuilder
    {
        public static void BuildLightSources(this Map map, TiledObjectGroup mapObjects)
        {
            foreach (var lightSource in mapObjects.objectsWithName("light_source"))
            {
                var entity = map.scene.createEntity("world-light", lightSource.position + new Vector2(lightSource.width / 2, lightSource.height / 2));
                entity.setScale(new Vector2(lightSource.width / 16, lightSource.height / 16) * 0.5f);
                var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
                sprite.material = Material.blendLinearDodge();
                sprite.color = ColorExt.hexToColor("#" + lightSource.properties["color"].Substring(2));
                sprite.renderLayer = Layers.Lights;

                var props = lightSource.properties;
                if (props.ContainsKey("toggle_key") && !string.IsNullOrWhiteSpace(props["toggle_key"]))
                {
                    var listener = entity.addComponent(new MapEventListener(props["toggle_key"])
                    {
                        EventTriggered = e =>
                        {
                            if (e.Parameters?.Length > 0 && e.Parameters[0] is bool enabled)
                            {
                                entity.setEnabled(enabled);
                            }
                            else
                            {
                                entity.setEnabled(!entity.enabled);
                            }
                        }
                    });
                    map.MapEventListeners.Add(listener);
                }
            }
        }
    }
}
