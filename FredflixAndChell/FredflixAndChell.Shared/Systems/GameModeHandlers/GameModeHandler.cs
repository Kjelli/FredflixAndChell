using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
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
        private GameSettings _settings;

        public GameSystem GameSystem => _gameSystem;
        public GameSettings Settings => _settings;

        private bool _weHaveAWinner;

        public GameModeHandler(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public virtual void Setup(GameSettings settings)
        {
            _settings = settings;
            _gameSystem.StartRound();
        }
        public abstract bool WeHaveAWinner();

      
    }
}
