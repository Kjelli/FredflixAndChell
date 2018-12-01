using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Maps.Events
{
    public abstract class MapEventEmitter : Entity
    {
        protected Map Map { get; set; }
        protected string EventKey { get; set; }

        public MapEventEmitter(Map map, string eventKey)
        {
            Map = map;
            EventKey = eventKey;
        }

        public void EmitMapEvent()
        {
            var mapEvent = new MapEvent { EventKey = EventKey };

            Map.MapEventListeners
                .Where(listener => listener.EventKey == mapEvent.EventKey)
                .ToList()
                .ForEach(listener => listener.EventTriggered(mapEvent));
        }
    }
}
