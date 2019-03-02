using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Maps.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Utilities.Events
{
    public class GameEventParameters{
        internal GameEventParameters()
        {
            // Do not instantiate this
        }
    }
    public class PlayerKilledEventParameters : GameEventParameters
    {
        public Player Killed { get; set; }
        public Player Killer { get; set; }
    }
    public class GlobalMapEventParameters : GameEventParameters
    {
        public MapEvent MapEvent { get; set; }
    }

    public class FlagPickedUpEventParameters : GameEventParameters
    {
        public Player CapturingPlayer { get; set; }
    }

    public class FlagDroppedEventParameters : GameEventParameters
    {
        public Player DroppingPlayer { get; set; }
    }
}
