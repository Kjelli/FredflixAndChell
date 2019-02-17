using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class RoundsHandler : GameModeHandler
    {
        private bool _weHaveAWinner;

        public RoundsHandler(GameSystem gameSystem) : base(gameSystem) { }

        public override void Setup(GameSettings settings)
        {
            base.Setup(settings);
            GameSystem.Subscribe(GameEvents.PlayerKilled, QueueOnPlayerKilled);
            GameSystem.DebugLines.Add(new DebugLine
            {
                Text = () => $"Players Alive: {GameSystem.Players.Count(p => p.PlayerState == PlayerState.Normal) }"
            });
        }

        private void QueueOnPlayerKilled(GameEventParameters parameters)
        {
            Core.schedule(1.0f, _ => OnPlayerKilled(parameters));
        }

        private void OnPlayerKilled(GameEventParameters parameters)
        {
            var pkParams = parameters as PlayerKilledEventParameters;
            CheckForWinner();
        }

        private void CheckForWinner()
        {
            var alivePlayersLeft = GameSystem.Players
                .Where(p => p.PlayerState == PlayerState.Normal);

            if (alivePlayersLeft.Count() > 1)
            {
                return;
            }

            var player = alivePlayersLeft.FirstOrDefault();
            if (player != null)
            {
                var playerScore = ContextHelper.PlayerMetadata?.FirstOrDefault(x => x.PlayerIndex == player.PlayerIndex);
                if (playerScore == null) return;

                playerScore.Score++;

                if (playerScore.Score >= Settings.ScoreLimit)
                {
                    _weHaveAWinner = true;
                }
            }

            GameSystem.EndRound();
        }

        public override bool WeHaveAWinner()
        {
            return _weHaveAWinner;
        }
    }
}
