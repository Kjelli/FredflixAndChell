using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.Events
{
    public class CollisionEventEmitter : MapEventEmitter
    {
        public CollisionEventEmitter(Map map, string eventKey, RectangleF rectangle, int physicsLayer) : base(map, eventKey)
        {
            name = "CollisionEventEmitter_" + eventKey;
            tag = Tags.EventEmitter;

            var collider = addComponent(new BoxCollider(rectangle));
            collider.isTrigger = true;
            Flags.setFlagExclusive(ref collider.physicsLayer, physicsLayer);
        }
    }
}
