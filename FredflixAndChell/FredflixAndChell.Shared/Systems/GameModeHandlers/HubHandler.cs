using System;
using FredflixAndChell.Shared.Utilities.Events;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Assets;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Characters;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class HubHandler : GameModeHandler
    {
        private GameSettings _gameSettings;
        public HubHandler(GameSystem gameSystem) : base(gameSystem)
        {
            _gameSettings = new GameSettings();
        }

        public override void Setup(GameSettings settings)
        {
            base.Setup(settings);
            GameSystem.Subscribe(GameEvents.GlobalMapEvent, HandleMapEvent);
        }

        private void HandleMapEvent(GameEventParameters parameters)
        {
            GlobalMapEventParameters mapEventParameters = parameters as GlobalMapEventParameters;
            if (mapEventParameters == null) return;

            switch (mapEventParameters.MapEvent.EventKey)
            {
                case "ready":
                    var state = mapEventParameters.MapEvent.Parameters[0];
                    var entered = (string) state == Strings.CollisionMapEventEnter;
                    HandleReadiness(entered);
                    break;
                case "mode":
                    ToggleGameMode();
                    break;
                case "teams":
                    ToggleTeams();
                    break;
                case "character":
                    var player = mapEventParameters.MapEvent.Parameters[0] as Player;
                    SelectCharacter(player);
                    break;
            }
        }

        private void SelectCharacter(Player player)
        {
            var currentCharacter = player.Parameters.CharacterName;
            var nextCharacter = Characters.GetNextAfter(currentCharacter);
            player.SetParameters(nextCharacter);
        }

        private void ToggleTeams()
        {
            var teamMode = _gameSettings.Team;
            var nextTeamMode = teamMode.Next();
            _gameSettings.Team = nextTeamMode;
            GameSystem.Map.EmitMapEvent(new MapEvent
            {
                EventKey = "teams_display",
                Parameters = new string[] { $"{nextTeamMode}" }
            });
        }

        private void HandleReadiness(bool playerBecameReady)
        {
            Console.WriteLine($"A player is ready? {playerBecameReady}");
        }

        private void ToggleGameMode()
        {
            var gameMode = _gameSettings.GameMode;
            var nextGameMode = gameMode.Next();
            if (nextGameMode == GameModes.Hub) nextGameMode = nextGameMode.Next();
            _gameSettings.GameMode = nextGameMode;
            GameSystem.Map.EmitMapEvent(new MapEvent {
                EventKey = "mode_display",
                Parameters = new string[]{ $"{nextGameMode}" }
            });
        }

        public override bool WeHaveAWinner()
        {
            return false;
        }
    }
}
