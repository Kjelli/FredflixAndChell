using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class KingOfTheHillHandler : GameModeHandler
    {
        private int _winningTeamIndex;
        private bool _weHaveAWinner;
        private List<Player> _playersInZone;

        public KingOfTheHillHandler(GameSystem gameSystem) : base(gameSystem)
        {
            _playersInZone = new List<Player>();
        }

        public override void Setup(GameSettings settings)
        {
            base.Setup(settings);
            GameSystem.Subscribe(GameEvents.GlobalMapEvent, HandleKOTHMapEvent);
            GameSystem.Subscribe(GameEvents.PlayerKilled, PlayerKilled);
            Core.schedule(0.2f, true, _ => CheckYourselfBeforeYouWreckYourself());
        }
        private void CheckYourselfBeforeYouWreckYourself()
        {
            if(_playersInZone.Count > 0)
            {
                foreach (Player p in _playersInZone)
                {
                    var playerScore = ContextHelper.PlayerMetadataByIndex(p.PlayerIndex);
                    //playerScore.Score++;
                    UpdateZoneColor("ffa_hold");
                    CheckForWinner();

                }
            }
            else
            {
                UpdateZoneColor("idle");
            }
            
        }

        private void PlayerKilled(GameEventParameters parameters)
        {
            var pkParams = parameters as PlayerKilledEventParameters;
            RemoveFromZoneList(pkParams.Killed);
            Core.schedule(3f, false, _ => OnPlayerKilled(parameters));
        }

        private void OnPlayerKilled(GameEventParameters parameters)
        {
            if (_winningTeamIndex > 0) return;
            var pkParams = parameters as PlayerKilledEventParameters;
            RespawnPlayer(pkParams.Killed);
        }

        private void RemoveFromZoneList(Player player)
        {
            if (_playersInZone.Contains(player))
            {
                _playersInZone.Remove(player);
            }
        }
        private void RespawnPlayer(Player player)
        {
            if (_weHaveAWinner) return;

            var players = GameSystem.Players;
            var spawnLocations = GameSystem.Map.SpawnLocations;

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
            player.Respawn(furthestSpawnPosition);
        }


        private void HandleKOTHMapEvent(GameEventParameters ev)
        {
            if (!(ev is GlobalMapEventParameters globalMapEvent)) return;

            var mapEvent = globalMapEvent.MapEvent;
            var key = mapEvent.EventKey;

            if (mapEvent.Parameters.Count() < 2) return;

            if(key == Constants.TiledProperties.KingOfTheHillZone)
            {
                var player = mapEvent.Parameters[1] as Player;
                if ((string)mapEvent.Parameters[0] == "enter")
                {
                    _playersInZone.Add(player);
                }
                else
                {
                    RemoveFromZoneList(player);
                }
            }
        }

        private void UpdateZoneColor(string color)
        {
            GameSystem.Map.EmitMapEvent(new MapEvent
            {
                EventKey = "koth_platform",
                Parameters = new object[] { color }
            });
        }

        private void CheckForWinner()
        {
            if (_weHaveAWinner) return;

            var maxScoreHolder = ContextHelper.PlayerMetadata.Max();
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



        private void FocusCameraOnWinners()
        {
           
        }

        public override bool WeHaveAWinner()
        {
            var anyWinningPlayer = GameSystem.Players.First(p => p.TeamIndex == _winningTeamIndex);
            var playerMeta = ContextHelper.PlayerMetadataByIndex(anyWinningPlayer.PlayerIndex);
            return playerMeta.Score >= GameSystem.Settings.ScoreLimit;
        }
    }
}
