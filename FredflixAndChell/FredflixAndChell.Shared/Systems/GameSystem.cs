using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Systems.GameModeHandlers;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Nez;
using Nez.Systems;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using System.Linq;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;

namespace FredflixAndChell.Shared.Systems
{
    public enum GameState
    {
        Starting, Started, Ending, Ended
    }

    public class GameSystem : SceneComponent
    {
        private const int MinNumberOfPlayersToPlay = 2;
        private const int MaxNumberOfPlayersForRestart = 1;
        private const float TransitionDelay = 4f;
        private const float LetterBoxSize = 140f;

        private GameState _gameState;

        private List<Player> _registeredPlayers;
        private Dictionary<int, Player> _playerStandings;
        private BroScene _broScene;
        private SmoothCamera _camera;
        private Emitter<GameEvents, GameEventParameters> _emitter;

        private GameSettings _gameSettings;
        private IGameModeHandler _gameHandler;

        private int _playersAlive;
        private Cooldown _transitionDelay;

        public Dictionary<int, Player> PlayerStandings => _playerStandings;
        public GameState GameState => _gameState;

        public List<DebugLine> DebugLines = new List<DebugLine>();

        public GameSystem(GameSettings gameSettings)
        {
            _registeredPlayers = new List<Player>();
            _playerStandings = new Dictionary<int, Player>();
            _transitionDelay = new Cooldown(TransitionDelay, true);
            _gameSettings = gameSettings;
            _emitter = new Emitter<GameEvents, GameEventParameters>();
        }
        public override void onEnabled()
        {
            _broScene = scene as BroScene;
            _camera = scene.getSceneComponent<SmoothCamera>();

            SetupGameModeHandler();
        }

        private void SetupGameModeHandler()
        {
            switch (_gameSettings.GameMode)
            {
                case GameModeHandlers.GameModes.Rounds:
                    _gameHandler = new RoundsHandler(this);
                    break;
                case GameModeHandlers.GameModes.Deathmatch:
                    break;
                case GameModeHandlers.GameModes.CaptureTheFlag:
                    break;
            }
            _gameHandler.Setup(_gameSettings);
        }

        public void RegisterPlayer(Player player)
        {
            _registeredPlayers.Add(player);
            var playerScore = ContextHelper.PlayerScores.FirstOrDefault(x => x.PlayerIndex == player.PlayerIndex);
            if (playerScore == null)
            {
                ContextHelper.PlayerScores.Add(new PlayerScore { PlayerIndex = player.PlayerIndex, Score = 0 });
            }
        }

        public override void update()
        {
            _playerStandings.Clear();
            _playersAlive = 0;

            foreach (var playerEntity in _registeredPlayers)
            {
                var player = playerEntity as Player;
                _playerStandings[player.PlayerIndex] = player;
                if (player.PlayerState != PlayerState.Dead)
                {
                    _playersAlive++;
                }
            }

            switch (_gameState)
            {
                case GameState.Starting:
                    if (_playersAlive >= MinNumberOfPlayersToPlay)
                    {
                        StartRound();
                    }
                    break;
                case GameState.Started:
                    
                    break;
                case GameState.Ending:
                    Time.timeScale *= 0.9975f;
                    _camera.BaseZoom *= 1.0025f;
                    _broScene.LetterBox.letterboxSize = 0.99f * _broScene.LetterBox.letterboxSize + 0.01f * LetterBoxSize;
                    _transitionDelay.Update();
                    if (_transitionDelay.IsReady())
                    {
                        PrepareForNewRound();
                        if (WeHaveAWinner())
                        {
                            EndGame();
                        }
                        else
                        {
                            StartNextRound();
                        }
                    }
                    break;
                case GameState.Ended:
                    break;
            }
        }

        private void EndGame()
        {
            ContextHelper.PlayerScores = null;
            Core.scene = new LobbyScene();
        }

        private bool WeHaveAWinner()
        {
            // TODO: 3 is just a random number at this point. Set this somewhere else
            // where we would define the game mode (stock lives, best of X, first to Y etc.)
            return ContextHelper.PlayerScores.Any(x => x.Score == 3);
        }

        private void DetermineWinner()
        {
            foreach (var playerEntity in _registeredPlayers)
            {
                var player = playerEntity as Player;
                _playerStandings[player.PlayerIndex] = player;
                if (player.PlayerState == PlayerState.Normal)
                {
                    var playerScore = ContextHelper.PlayerScores.FirstOrDefault(x => x.PlayerIndex == player.PlayerIndex);
                    playerScore.Score++;
                }
            }
        }

        private void StartRound()
        {
            _gameState = GameState.Started;
        }

        private void PrepareForNewRound()
        {
            _gameState = GameState.Ended;
            Time.timeScale = 1.0f;
            //TODO: This might be a source for bugs later on ...
            TweenManager.stopAllTweens();
        }

        private void StartNextRound()
        {
            Core.startSceneTransition(new CrossFadeTransition(() => new BroScene(_gameSettings)));
        }

        private void EndRound()
        {
            Time.timeScale = 0.8f;
            _gameState = GameState.Ending;
            _transitionDelay.Start();
            var sepia = _broScene.addPostProcessor(new VignettePostProcessor(5));
            sepia.effect = new SepiaEffect();
            _camera.SetWinMode(true);
        }

        public void Subscribe(GameEvents gameEvent, Action<GameEventParameters> handler){
            _emitter.addObserver(gameEvent, handler);
        }

        public void Publish(GameEvents gameEvent, GameEventParameters parameters)
        {
            _emitter.emit(gameEvent, parameters);
        }
    }

    public class PlayerScore
    {
        public int Score { get; set; }
        public int PlayerIndex { get; set; }
    }
}
