using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities.Events;
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
        private GameSystem _gameSystem;
        protected Map Map { get; set; }
        protected string EventKey { get; set; }

        public bool EmitGlobally { get; set; }

        public MapEventEmitter(Map map, string eventKey)
        {
            Map = map;
            EventKey = eventKey;
        }

        public override void onAddedToScene()
        {
            _gameSystem = scene.getSceneComponent<GameSystem>();
        }

        public void EmitMapEvent(object[] parameters = null)
        {
            var mapEvent = new MapEvent { EventKey = EventKey, Parameters = parameters };

            Map.EmitMapEvent(mapEvent, EmitGlobally);
        }
    }
}
