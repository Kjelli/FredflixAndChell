using System.Collections.Generic;
using Nez;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Scenes;
using System;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez.Tweens;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Components.Cameras;

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

        private int _playersAlive;
        private Cooldown _transitionDelay;

        public Dictionary<int, Player> PlayerStandings => _playerStandings;
        public GameState GameState => _gameState;


        public GameSystem()
        {
            _registeredPlayers = new List<Player>();
            _playerStandings = new Dictionary<int, Player>();
            _transitionDelay = new Cooldown(TransitionDelay, true);
        }
        public override void onEnabled()
        {
            _broScene = scene as BroScene;
            _camera = scene.getSceneComponent<SmoothCamera>();
        }

        public void RegisterPlayer(Player player)
        {
            _registeredPlayers.Add(player);
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
                        StartGame();
                    }
                    break;
                case GameState.Started:
                    if (_playersAlive <= MaxNumberOfPlayersForRestart)
                    {
                        EndGame();
                    }
                    break;
                case GameState.Ending:
                    Time.timeScale *= 0.9975f;
                    _camera.BaseZoom *= 1.0025f;
                    _broScene.LetterBox.letterboxSize = 0.99f * _broScene.LetterBox.letterboxSize + 0.01f * LetterBoxSize;
                    _transitionDelay.Update();
                    if (_transitionDelay.IsReady())
                    {
                        StartNextGame();
                    }
                    break;
                case GameState.Ended:
                    break;
            }

        }

        private void StartGame()
        {
            _gameState = GameState.Started;
        }

        private void StartNextGame()
        {
            _gameState = GameState.Ended;
            Time.timeScale = 1.0f;
            TweenManager.stopAllTweens();
            Core.startSceneTransition(new CrossFadeTransition(() => new BroScene()));
        }

        private void EndGame()
        {
            Time.timeScale = 0.8f;
            _gameState = GameState.Ending;
            _transitionDelay.Start();
            var sepia = _broScene.addPostProcessor(new VignettePostProcessor(5));
            sepia.effect = new SepiaEffect();
            _camera.SetWinMode(true);
        }
    }
}
