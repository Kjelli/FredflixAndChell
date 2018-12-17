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
            Core.schedule(2.0f, _ => OnPlayerKilled(parameters));
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

            RespawnPlayer(pkParams.Killed);
            CheckForWinner();
        }

        private void RespawnPlayer(Player player)
        {
            var playerLocations = GameSystem.Players.Select(p => p.position);
            var spawnLocations = GameSystem.Map.PlayerSpawner.SpawnLocations;
            var furthestSpawn = spawnLocations.Aggregate((spawnA, spawnB) =>
            {
                var enumerable = playerLocations as Vector2[] ?? playerLocations.ToArray();
                return enumerable.Sum(p1 => (new Vector2(spawnA.X, spawnA.Y) - p1).Length()) >
                       enumerable.Sum(p2 => (new Vector2(spawnB.X, spawnB.Y) - p2).Length())
                    ? spawnA
                    : spawnB;
            });
            player.Respawn(new Vector2(furthestSpawn.X, furthestSpawn.Y));
        }

        private void CheckForWinner()
        {
            _weHaveAWinner = false;
            return;
            GameSystem.EndRound();
        }

        public override bool WeHaveAWinner()
        {
            return _weHaveAWinner;
        }
    }
}
