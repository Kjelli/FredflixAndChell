using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public abstract class GameModeHandler : IGameModeHandler
    {
        private GameSystem _gameSystem;

        public GameSystem GameSystem => _gameSystem;

        public GameModeHandler(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public abstract void Setup(GameSettings settings);
    }
}
