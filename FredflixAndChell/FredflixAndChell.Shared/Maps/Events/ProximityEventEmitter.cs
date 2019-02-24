using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.Events
{
    public class ProximityEventEmitter : MapEventEmitter
    {
        public ProximityEventEmitter(Map map, string eventKey, Vector2 center, float radius, int physicsLayer) : base(map, eventKey)
        {
            name = "ProximityEventEmitter_" + eventKey;
            tag = Tags.EventEmitter;

            var collider = addComponent(new CircleCollider(radius));
            collider.isTrigger = true;

            position = new Vector2(center.X + radius, center.Y + radius);

            Flags.setFlagExclusive(ref collider.physicsLayer, physicsLayer);
        }
    }
}
