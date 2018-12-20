using FredflixAndChell.Shared.Components.Interactables;
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
    public class InteractEventEmitter : MapEventEmitter
    {
        public InteractEventEmitter(Map map, string eventKey, RectangleF rectangle) : base(map, eventKey)
        {
            tag = Tags.EventEmitter;
            name = "InteractEventEmitter";
            position = new Vector2(rectangle.x + 8, rectangle.y + 8);

            var collider = addComponent(new BoxCollider(rectangle.width, rectangle.height));
            collider.isTrigger = true;
            Flags.setFlagExclusive(ref collider.physicsLayer, Layers.Interactables);

            addComponent(new InteractableComponent()
            {
                OnInteract = player => EmitMapEvent(new object[]{ player })
            });
        }
    }
}
