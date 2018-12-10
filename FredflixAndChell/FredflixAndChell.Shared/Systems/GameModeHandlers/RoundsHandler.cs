using FredflixAndChell.Shared.Utilities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class RoundsHandler : GameModeHandler
    {
        public RoundsHandler(GameSystem gameSystem) : base(gameSystem) { }

        public override void Setup(GameSettings settings)
        {
            GameSystem.Subscribe(GameEvents.PlayerKilled, OnPlayerKilled);
        }

        private void OnPlayerKilled(GameEventParameters parameters)
        {
            var pkParams = parameters as PlayerKilledEventParameters;
            if (pkParams.Killer != null)
            {
                Console.WriteLine($"{pkParams.Killer.name} smashed {pkParams.Killed.name}");
            }
            else
            {
                Console.WriteLine($"{pkParams.Killed.name} smashed themself");
            }
        }
    }
}
