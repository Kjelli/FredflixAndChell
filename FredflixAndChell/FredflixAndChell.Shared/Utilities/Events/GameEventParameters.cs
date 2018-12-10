using FredflixAndChell.Shared.GameObjects.Players;
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
}
