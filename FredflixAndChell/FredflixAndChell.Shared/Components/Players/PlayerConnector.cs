using System;
using Nez;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Systems;

namespace FredflixAndChell.Components.Players
{
    public class PlayerConnector : SceneComponent
    {
        private int _maxPlayers;
        private List<int> _connectedPlayers;
        private GameSystem _gameSystem;

        public PlayerConnector(int maxPlayers = 4)
        {
            Input.maxSupportedGamePads = maxPlayers;
            _maxPlayers = maxPlayers;
            _connectedPlayers = new List<int>();
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
            for (var playerIndex = 0; playerIndex < _maxPlayers; playerIndex++)
            {
                var gamePadState = GamePad.GetState(playerIndex);

                if (gamePadState.IsConnected && !_connectedPlayers.Contains(playerIndex))
                {
                    SpawnPlayer(playerIndex);
                }
            }

            if(!_connectedPlayers.Contains(-1))
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
            var spawnX = 100 + playerIndex * 32;
            var spawnY = 100;
            var player = scene.createEntity($"player_{playerIndex}");
            player.addComponent(new Player(spawnX, spawnY, playerIndex));
            _connectedPlayers.Add(playerIndex);

            Console.WriteLine($"Spawned {player.name}");
        }

        public override void update()
        {
        }
    }
}