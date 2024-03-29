﻿using FredflixAndChell.Shared.Components.Cameras;
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
using FredflixAndChell.Shared.Maps;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;

namespace FredflixAndChell.Shared.Systems
{
    public enum GameState
    {
        Starting, Started, Ending, Ended
    }

    public class GameSystem : SceneComponent
    {
        private const float TransitionDelay = 4f;
        private const float LetterBoxSize = 140f;

        private GameState _gameState;

        private readonly List<Player> _players;
        private BroScene _broScene;
        private SmoothCamera _camera;
        private Emitter<GameEvents, GameEventParameters> _emitter;
        private Map _map;

        private GameSettings _gameSettings;
        private IGameModeHandler _gameHandler;

        private Cooldown _transitionDelay;

        public GameState GameState => _gameState;
        public List<Player> Players => _players;
        public List<DebugLine> DebugLines = new List<DebugLine>();
        public Map Map => _map;
        public GameSettings Settings => _gameSettings;

        public GameSystem(GameSettings gameSettings, Map map)
        {
            _map = map;
            _players = new List<Player>();
            _transitionDelay = new Cooldown(TransitionDelay, true);
            _gameSettings = gameSettings;
            _emitter = new Emitter<GameEvents, GameEventParameters>();
        }
        public override void onEnabled()
        {
            _broScene = scene as BroScene;
            _camera = scene.getSceneComponent<SmoothCamera>();

            DebugLines.Add(new DebugLine
            {
                Text = () => $"Game State: {_gameState}"
            });
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
                    _gameHandler = new DeathmatchHandler(this);
                    break;
                case GameModeHandlers.GameModes.CaptureTheFlag:
                    break;
            }
            _gameHandler.Setup(_gameSettings);
        }

        public void RegisterPlayer(Player player)
        {
            _players.Add(player);
            var playerScore = ContextHelper.PlayerScores.FirstOrDefault(x => x.PlayerIndex == player.PlayerIndex);
            if (playerScore == null)
            {
                ContextHelper.PlayerScores.Add(new PlayerScore { PlayerIndex = player.PlayerIndex, Score = 0 });
            }
        }

        public override void update()
        {
            switch (_gameState)
            {
                case GameState.Starting:
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
                        if (_gameHandler.WeHaveAWinner())
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

        public void StartRound()
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

        public void EndRound()
        {
            _gameState = GameState.Ending;
            Time.timeScale = 0.8f;
            _transitionDelay.Start();
            var sepia = _broScene.addPostProcessor(new VignettePostProcessor(5));
            sepia.effect = new SepiaEffect();
            _camera.SetWinMode(true);
        }

        public void Subscribe(GameEvents gameEvent, Action<GameEventParameters> handler)
        {
            _emitter.addObserver(gameEvent, handler);
        }

        public void Publish(GameEvents gameEvent, GameEventParameters parameters)
        {
            _emitter.emit(gameEvent, parameters);
        }
    }

    public class PlayerScore : IComparable<PlayerScore>
    {
        public int Score { get; set; }
        public int PlayerIndex { get; set; }

        public int CompareTo(PlayerScore other)
        {
            return Score.CompareTo(other.Score);
        }
    }
}
