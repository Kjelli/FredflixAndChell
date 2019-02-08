using FredflixAndChell.Shared.Maps.Events;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class EventEmitterBuilder
    {
        public static void BuildEventEmitters(this Map map, TiledObjectGroup mapObjects)
        {
            foreach (var eventEmitter in mapObjects.objectsWithName(TiledObjects.EventEmitter))
            {
                var bounds = new RectangleF(eventEmitter.position, new Vector2(eventEmitter.width, eventEmitter.height));
                var props = eventEmitter.properties;
                var type = props["type"];
                var key = props["key"];
                var global = props.ContainsKey("global") ? bool.Parse(props["global"]) : false;

                MapEventEmitter emitter = null;
                switch (type)
                {
                    case "timed":
                        float.TryParse(props["interval_min"], out float intervalMin);
                        float.TryParse(props["interval_max"], out float intervalMax);
                        int.TryParse(props["repeat"], out int repeat);

                        emitter = map.scene.addEntity(new TimedEventEmitter(map, key, intervalMin, intervalMax, repeat));
                        break;
                    case "collision":
                        int.TryParse(props["physics_layer"], out int physicsLayer);
                        emitter = map.scene.addEntity(new CollisionEventEmitter(map, key, bounds, physicsLayer));
                        break;
                    case "interact":
                        emitter = map.scene.addEntity(new InteractEventEmitter(map, key, bounds));
                        break;
                    case "proximity":
                        int.TryParse(props["physics_layer"], out int proximityLayer);
                        emitter = map.scene.addEntity(new ProximityEventEmitter(map, key, eventEmitter.position, eventEmitter.width / 2, proximityLayer));
                        break;
                    default:
                        Console.Error.WriteLine($"MapEventEmitter of type {type} not recognized!");
                        break;
                }

                if (global)
                {
                    emitter.EmitGlobally = true;
                }
            }
        }
    }
}
