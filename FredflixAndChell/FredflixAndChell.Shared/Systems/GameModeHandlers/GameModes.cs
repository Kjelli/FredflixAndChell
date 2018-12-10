using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public enum GameModes
    {
        Rounds, // Score by being last player alive
        Deathmatch, // Score by killing another player
        CaptureTheFlag, // Score by retrieving opposite team's flag
    }
}
