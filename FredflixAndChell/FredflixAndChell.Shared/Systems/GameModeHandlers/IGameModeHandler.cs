using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public interface IGameModeHandler
    {
        void Setup(GameSettings settings);
        bool WeHaveAWinner();
    }
}
