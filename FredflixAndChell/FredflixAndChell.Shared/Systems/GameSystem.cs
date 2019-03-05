using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Maps;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Systems.GameModeHandlers;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
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
        public IGameModeHandler GameModeHandler => _gameHandler;

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
                case GameMode.HUB:
                    _gameHandler = new HubHandler(this);
                    break;
                case GameMode.FRAGS:
                    _gameHandler = new RoundsHandler(this);
                    break;
                case GameMode.DM:
                    _gameHandler = new DeathmatchHandler(this);
                    break;
                case GameMode.CTF:
                    _gameHandler = new CaptureTheFlagHandler(this);
                    break;
                case GameMode.KOTH:
                    _gameHandler = new KingOfTheHillHandler(this);
                    break;
            }
            (scene as BroScene)?.OnGameHandlerAdded(_gameHandler);
            _gameHandler.Setup(_gameSettings);
        }

        public void RegisterPlayer(Player player)
        {
            _players.Add(player);
            var playerMeta = ContextHelper.PlayerMetadataByIndex(player.PlayerIndex);
            if (playerMeta == null)
            {
                playerMeta = new PlayerMetadata
                {
                    PlayerIndex = player.PlayerIndex
                };
                ContextHelper.PlayerMetadata.Add(playerMeta);
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
            ContextHelper.PlayerMetadata.ForEach(p => p.Score = 0);
            Core.startSceneTransition(new CrossFadeTransition(() => new HubScene()));
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

        public void StartNextRound()
        {
            Core.startSceneTransition(new CrossFadeTransition(
                () => new BroScene(ContextHelper.GameSettings)));
        }

        public void EndRound()
        {
            _gameState = GameState.Ending;
            Time.timeScale = 0.8f;
            _transitionDelay.Start();
            var sepia = _broScene.addPostProcessor(new VignettePostProcessor(5));
            sepia.effect = new SepiaEffect();
            _camera.SetWinMode(true);
            if (_gameHandler.WeHaveAWinner())
            {
                _broScene.LetterBox.color = Color.White;
            }
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

    public class PlayerMetadata : IComparable<PlayerMetadata>
    {
        public int TeamIndex { get; set; }
        public int Score { get; set; }
        public int PlayerIndex { get; set; }
        public CharacterParameters Character { get; set; }
        public WeaponParameters Weapon { get; set; }
        public bool IsInitialized { get; set; }

        public PlayerMetadata()
        {
            Score = 0;
            Character = Characters.Get(Constants.Strings.DefaultStartCharacter);
            Weapon = Guns.Get("M4");
        }

        public int CompareTo(PlayerMetadata other)
        {
            return Score.CompareTo(other.Score);
        }
    }
}
