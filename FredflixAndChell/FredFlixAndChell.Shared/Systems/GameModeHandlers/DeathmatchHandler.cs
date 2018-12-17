using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;
using FredflixAndChell.Shared.Components.Cameras;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class DeathmatchHandler : GameModeHandler
    {
        private bool _weHaveAWinner;

        public DeathmatchHandler(GameSystem gameSystem) : base(gameSystem) { }

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
            Core.schedule(2.5f, false, _ => OnPlayerKilled(parameters));
        }

        private void OnPlayerKilled(GameEventParameters parameters)
        {
            var pkParams = parameters as PlayerKilledEventParameters;
            if (pkParams.Killer != null)
            {
                var playerScore = ContextHelper.PlayerScores?.First(s => s.PlayerIndex == pkParams.Killer.PlayerIndex);
                if (playerScore != null)
                {
                    playerScore.Score++;
                }
            }
            CheckForWinner();
            RespawnPlayer(pkParams.Killed);
        }

        private void RespawnPlayer(Player player)
        {
            if (_weHaveAWinner) return;

            var players = GameSystem.Players;
            var spawnLocations = GameSystem.Map.PlayerSpawner.SpawnLocations;

            var furthestDistance = 0.0f;
            var furthestSpawnPosition = new Vector2();
            foreach (var spawnLocation in spawnLocations)
            {
                var spawnPosition = new Vector2(spawnLocation.X, spawnLocation.Y);
                var distanceToSpawnLocation = 0.0f;
                foreach (var otherPlayer in players)
                {
                    if (otherPlayer == player) continue;
                    var distance = Math.Abs((otherPlayer.position - spawnPosition).Length());
                    distanceToSpawnLocation += distance;
                }
                if (distanceToSpawnLocation >= furthestDistance)
                {
                    furthestDistance = distanceToSpawnLocation;
                    furthestSpawnPosition = spawnPosition;
                }
            }
            player.Respawn(furthestSpawnPosition);
        }

        private void CheckForWinner()
        {
            if (_weHaveAWinner) return;

            var maxScoreHolder = ContextHelper.PlayerScores.Max();
            if (maxScoreHolder.Score < Settings.ScoreLimit) return;

            _weHaveAWinner = true;
            var winningPlayer = GameSystem.Players.First(p => p.PlayerIndex == maxScoreHolder.PlayerIndex);
            winningPlayer.getComponent<CameraTracker>().setEnabled(true);
            var otherPlayers = GameSystem.Players.Where(p => p.PlayerIndex != maxScoreHolder.PlayerIndex);
            foreach (var otherPlayer in otherPlayers)
            {
                otherPlayer.getComponent<CameraTracker>().setEnabled(false);
            }
            GameSystem.EndRound();
        }

        public override bool WeHaveAWinner()
        {
            return _weHaveAWinner;
        }
    }
}
