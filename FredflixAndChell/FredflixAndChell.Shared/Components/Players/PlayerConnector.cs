using System;
using Nez;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.Scenes
{
    public class PlayerConnector : SceneComponent
    {
        private int _maxPlayers;
        private List<int> _connectedPlayers;

        public PlayerConnector(int maxPlayers = 4)
        {
            Input.maxSupportedGamePads = maxPlayers;
            _maxPlayers = maxPlayers;
            _connectedPlayers = new List<int>();
        }

        public override void onEnabled()
        {
            base.onEnabled();

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
            timer?.reset();
        }

        private void SpawnPlayer(int playerIndex)
        {
            var spawnX = 100 + playerIndex * 8;
            var spawnY = 100;
            var player = scene.createEntity($"player_{playerIndex}");
            player.addComponent(new Player(spawnX, spawnY, playerIndex));
            _connectedPlayers.Add(playerIndex);
        }

        public override void update()
        {
        }
    }
}