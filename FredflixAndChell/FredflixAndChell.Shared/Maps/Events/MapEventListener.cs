using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Maps.Events
{
    public class MapEventListener : Component
    {
        public string EventKey { get; set; }
        public MapEventListener(string eventKey)
        {
            EventKey = eventKey;
        }

        public Action<MapEvent> EventTriggered { get; set; }
    }
}
