using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;
using System.Collections.Generic;

namespace FredflixAndChell.Components.Players
{
    public class PlayerConnector : SceneComponent
    {
        private int _maxPlayers;
        private List<int> _connectedPlayers;
        private GameSystem _gameSystem;

        public PlayerSpawner _spawnLocations { get; set; }

        public PlayerConnector(int maxPlayers = 4, PlayerSpawner spawnLocations = null)
        {
            Input.maxSupportedGamePads = maxPlayers;
            _maxPlayers = maxPlayers;
            _connectedPlayers = new List<int>();
            _spawnLocations = spawnLocations;
        }

        public override void onEnabled()
        {
            base.onEnabled();

            _gameSystem = scene.getEntityProcessor<GameSystem>();

            Core.schedule(1, true, CheckForConnectedPlayers);
            CheckForConnectedPlayers();
        }

        private void CheckForConnectedPlayers(ITimer timer = null)
        {
            for (var playerIndex = 0; playerIndex < ContextHelper.NumPlayers - 1; playerIndex++)
            {
                var gamePadState = GamePad.GetState(playerIndex);

                if (gamePadState.IsConnected && !_connectedPlayers.Contains(playerIndex))
                {
                    SpawnPlayer(playerIndex);
                }
            }

            if (!_connectedPlayers.Contains(-1))
            {
                SpawnPlayer(-1);
            }

            if (!_connectedPlayers.Contains(-2))
            {
                SpawnPlayer(-2);
            }

            timer?.reset();
            Console.WriteLine($"Players ingame: {_connectedPlayers.Count}");
        }

        private void SpawnPlayer(int playerIndex)
        {
            Vector2 spawnLocation = _spawnLocations.DistributeSpawn();
            var spawnX = (int)spawnLocation.X;
            var spawnY = (int)spawnLocation.Y;
            var player = scene.createEntity($"player_{playerIndex}");
            player.addComponent(new Player(Characters.All().randomItem(), spawnX, spawnY, playerIndex));
            _connectedPlayers.Add(playerIndex);

            Console.WriteLine($"Spawned {player.name}");
        }

        public override void update()
        {
        }
    }
}