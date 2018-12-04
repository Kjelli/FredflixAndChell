using FredflixAndChell.Shared.Components.HUD;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Components.Players
{
    public class PlayerConnector : SceneComponent
    {
        private List<Player> _connectedPlayers;
        private GameSystem _gameSystem;
        private HUD _hud;

        private int _maxPlayers;
        private bool _playerSpawnedThisFrame;

        public PlayerSpawner _spawnLocations { get; set; }

        public PlayerConnector(int maxPlayers = 4, PlayerSpawner spawnLocations = null)
        {
            Input.maxSupportedGamePads = maxPlayers;
            _maxPlayers = maxPlayers;
            _connectedPlayers = new List<Player>();
            _spawnLocations = spawnLocations;
        }

        public override void onEnabled()
        {
            base.onEnabled();

            _gameSystem = scene.getSceneComponent<GameSystem>();
            _hud = scene.getSceneComponent<HUD>();

            Core.schedule(1, true, CheckForConnectedPlayers);
            CheckForConnectedPlayers();
        }

        private void CheckForConnectedPlayers(ITimer timer = null)
        {
            _playerSpawnedThisFrame = false;

            for (var playerIndex = 0; playerIndex < ContextHelper.NumPlayers - 1; playerIndex++)
            {
                var gamePadState = GamePad.GetState(playerIndex);

                if (gamePadState.IsConnected && !_connectedPlayers.Any(p => p.PlayerIndex == playerIndex))
                {
                    SpawnPlayer(playerIndex);
                    Console.WriteLine($"Connected player {playerIndex}. Players ingame: {_connectedPlayers.Count}");
                }
            }

            if (!_connectedPlayers.Any(p => p.PlayerIndex == -1))
            {
                SpawnPlayer(-1);
            }

            if (!_connectedPlayers.Any(p => p.PlayerIndex == -2))
            {
                SpawnPlayer(-2);
            }


            if (!_connectedPlayers.Any(p => p.PlayerIndex == -3))
            {
                SpawnPlayer(-3);
            }


            if (_playerSpawnedThisFrame)
            {
                _hud.AddPlayers(_connectedPlayers);
            }

            timer?.reset();
        }

        private void SpawnPlayer(int playerIndex)
        {
            Vector2 spawnLocation = _spawnLocations.DistributeSpawn();
            var spawnX = (int)spawnLocation.X;
            var spawnY = (int)spawnLocation.Y;
            var player = scene.addEntity(new Player(Characters.All().randomItem(), spawnX, spawnY, playerIndex));

            _connectedPlayers.Add(player);

            _playerSpawnedThisFrame = true;
            Console.WriteLine($"Spawned {player.name}");
        }

        public override void update()
        {
        }
    }
}