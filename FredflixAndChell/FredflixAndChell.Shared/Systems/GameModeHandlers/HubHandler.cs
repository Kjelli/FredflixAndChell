using System;
using FredflixAndChell.Shared.Utilities.Events;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Assets;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using System.Linq;
using Nez;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class HubHandler : GameModeHandler
    {
        private GameSettings _nextGameSettings;

        private int readyPlayers = 0;
        private bool starting;

        public HubHandler(GameSystem gameSystem) : base(gameSystem)
        {
            SetGameSettings(ContextHelper.GameSettings ?? GameSettings.Default);
        }

        public override void Setup(GameSettings settings)
        {
            base.Setup(settings);
            GameSystem.Subscribe(GameEvents.GlobalMapEvent, HandleMapEvent);
        }

        public void SetGameSettings(GameSettings gameSettings)
        {
            _nextGameSettings = gameSettings;

            ToggleGameMode(gameSettings.GameMode);
            ToggleMap(gameSettings.Map);
            ToggleTeamMode(gameSettings.TeamMode);
        }

        private void HandleMapEvent(GameEventParameters parameters)
        {
            GlobalMapEventParameters mapEventParameters = parameters as GlobalMapEventParameters;
            if (mapEventParameters == null) return;

            switch (mapEventParameters.MapEvent.EventKey)
            {
                case Strings.EventReady:
                    var state = mapEventParameters.MapEvent.Parameters[0];
                    var entered = (string)state == Strings.CollisionMapEventEnter;
                    HandleReadiness(entered);
                    break;
                case Strings.TiledMapGameModeKey:
                    ToggleGameMode(_nextGameSettings.GameMode.Next());
                    break;
                case Strings.TiledMapTeamsKey:
                    ToggleTeamMode(_nextGameSettings.TeamMode.Next());
                    break;
                case Strings.TiledMapCharacterSelectKey:
                    var player = mapEventParameters.MapEvent.Parameters[0] as Player;
                    SelectCharacter(player);
                    break;
                case Strings.TiledMapMapKey:
                    ToggleMap();
                    break;
            }
        }

      
        private void SelectCharacter(Player player)
        {
            var currentCharacter = player.Parameters.CharacterName;
            var nextCharacter = Characters.GetNextAfter(currentCharacter);
            player.SetParameters(nextCharacter);
        }

        private void NotifyGameSettingsChange(string key, string value)
        {
            GameSystem.Map.EmitMapEvent(new MapEvent
            {
                EventKey = key,
                Parameters = new string[] { $"{value}" }
            });
        }

        private void ToggleGameMode(GameMode gameMode)
        {
            // Cannot select hub
            if (gameMode == GameMode.Hub) {
                gameMode = gameMode.Next();
            }
            _nextGameSettings.GameMode = gameMode;
            NotifyGameSettingsChange(Strings.TiledMapGameModeDisplayKey, gameMode.ToString());
        }
        private void ToggleMap(string nextMap = null)
        {
            if (nextMap == null)
            {
                var allMaps = AssetLoader.GetMaps();
                var currentMapIndex = allMaps.IndexOf(_nextGameSettings.Map);
                nextMap = allMaps[(currentMapIndex + 1) % allMaps.Count];
            }
            _nextGameSettings.Map = nextMap;

            NotifyGameSettingsChange(Strings.TiledMapMapDisplayKey, nextMap);
        }

        private void ToggleTeamMode(TeamMode teamMode)
        {
            var nextTeamMode = teamMode;
            _nextGameSettings.TeamMode = nextTeamMode;
            NotifyGameSettingsChange(Strings.TiledMapTeamsDisplayKey, nextTeamMode.ToString());
        }

        private void HandleReadiness(bool playerBecameReady)
        {
            readyPlayers += playerBecameReady ? 1 : -1;
            if (readyPlayers == GameSystem.Players.Count() && !starting)
            {
                starting = true;
                Countdown(3);
            }
        }

        private void Countdown(int seconds)
        {
            Console.WriteLine($"Starting in {seconds} seconds");
            if (readyPlayers == GameSystem.Players.Count())
            {
                if (seconds == 0)
                {
                    StartMatch();
                }
                else
                {
                    Core.schedule(1, _ => Countdown(seconds - 1));
                }
            }
            else
            {
                starting = false;
            }
        }

        private void StartMatch()
        {
            ContextHelper.GameSettings = _nextGameSettings;
            GameSystem.StartNextRound();
        }


        public override bool WeHaveAWinner()
        {
            return false;
        }
    }
}
