using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Linq;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class CaptureTheFlagHandler : GameModeHandler
    {
        private int _winningTeamIndex;

        public CaptureTheFlagHandler(GameSystem gameSystem) : base(gameSystem)
        {
        }

        public override void Setup(GameSettings settings)
        {
            base.Setup(settings);
            GameSystem.Subscribe(GameEvents.GlobalMapEvent, HandleCTFMapEvent);
            GameSystem.Subscribe(GameEvents.PlayerKilled, QueueOnPlayerKilled);
        }
        private void QueueOnPlayerKilled(GameEventParameters parameters)
        {
            Core.schedule(2.5f, false, _ => OnPlayerKilled(parameters));
        }

        private void OnPlayerKilled(GameEventParameters parameters)
        {
            if (_winningTeamIndex > 0) return;
            var pkParams = parameters as PlayerKilledEventParameters;
            RespawnPlayer(pkParams.Killed);
        }

        private void RespawnPlayer(Player player)
        {
            var players = GameSystem.Players;
            var spawnLocations = GameSystem.Map.GetSpawnLocations(player.TeamIndex);

            var furthestDistance = 0.0f;
            var furthestSpawnPosition = new Vector2();
            foreach (var spawnLocation in spawnLocations)
            {
                var spawnPosition = spawnLocation.Position;
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
            var meta = ContextHelper.PlayerMetadataByIndex(player.PlayerIndex);
            var previousWeapon = meta.Weapon;
            if(previousWeapon?.Name == "Flag")
            {
                meta.Weapon = null;
            }
            player.Respawn(furthestSpawnPosition);
            
        }

        private void HandleCTFMapEvent(GameEventParameters ev)
        {
            if (!(ev is GlobalMapEventParameters globalMapEvent)) return;

            var mapEvent = globalMapEvent.MapEvent;
            var key = mapEvent.EventKey;

            if (mapEvent.Parameters.Count() < 2) return;

            var player = mapEvent.Parameters[1] as Player;
            var playerInventory = player.getComponent<PlayerInventory>();

            if ((string)mapEvent.Parameters[0] != Constants.Strings.CollisionMapEventEnter) return;
            if (playerInventory.Weapon?.Name != "Flag") return;

            if (key == Constants.TiledProperties.CaptureTheFlagRedCollisionZone
                && player.TeamIndex == Constants.Values.TeamIndexRed)
            {
                _winningTeamIndex = player.TeamIndex;
            }
            else if (key == Constants.TiledProperties.CaptureTheFlagBlueCollisionZone
                && player.TeamIndex == Constants.Values.TeamIndexBlue)
            {
                _winningTeamIndex = player.TeamIndex;
            }

            if (_winningTeamIndex > 0)
            {
                playerInventory.DestroyWeapon();
                FocusCameraOnWinners();
                GameSystem.EndRound();
            }
        }

        private void FocusCameraOnWinners()
        {
            var losingPlayers = GameSystem.Players.Where(p => p.TeamIndex != _winningTeamIndex);
            var winningPlayers  = GameSystem.Players.Where(p => p.TeamIndex == _winningTeamIndex);
            foreach (var player in losingPlayers)
            {
                player.getComponent<CameraTracker>().setEnabled(false);
                var meta = ContextHelper.PlayerMetadataByIndex(player.PlayerIndex);
                var previousWeapon = meta.Weapon;
                if (previousWeapon?.Name == "Flag")
                {
                    meta.Weapon = null;
                }
            }

            foreach (var player in winningPlayers)
            {
                ContextHelper.PlayerMetadataByIndex(player.PlayerIndex).Score++;
            }
        }

        public override bool WeHaveAWinner()
        {
            var anyWinningPlayer = GameSystem.Players.First(p => p.TeamIndex == _winningTeamIndex);
            var playerMeta = ContextHelper.PlayerMetadataByIndex(anyWinningPlayer.PlayerIndex);
            return playerMeta.Score >= GameSystem.Settings.ScoreLimit;
        }
    }
}
