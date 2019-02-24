using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class CaptureTheFlagHandler : GameModeHandler
    {
        public CaptureTheFlagHandler(GameSystem gameSystem) : base(gameSystem)
        {
        }

        public override bool WeHaveAWinner()
        {
            return false;
        }
    }
}
