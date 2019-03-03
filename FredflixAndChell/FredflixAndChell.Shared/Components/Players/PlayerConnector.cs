using FredflixAndChell.Shared.Components.HUD;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

namespace FredflixAndChell.Components.Players
{
    public class PlayerConnector : SceneComponent
    {
        private List<Player> _connectedPlayers;
        private GameSystem _gameSystem;
        private HUD _hud;
        private bool _playerSpawnedThisFrame;


        public PlayerConnector()
        {
            _connectedPlayers = new List<Player>();
        }

        public override void onEnabled()
        {
            base.onEnabled();

            _gameSystem = scene.getSceneComponent<GameSystem>();
            _hud = scene.getSceneComponent<HUD>();
        }

        public void ControllerConnected(GamePadData gamepad)
        {
            var index = _connectedPlayers.Count + 1;
            SpawnIdlePlayer(gamepad, index);
        }

        public void SpawnDebugPlayer()
        {
            SpawnPlayer(null, -(_connectedPlayers.Count + 1));
        }

        private Player SpawnPlayer(GamePadData gamepad, int index)
        {

            var playerMeta = ContextHelper.PlayerMetadataByIndex(index);
            if (playerMeta == null)
            {
                playerMeta = new PlayerMetadata
                {
                    PlayerIndex = index
                };
            }

            SpawnLocation spawnLocation = _gameSystem.Map.GetUniqueSpawnLocation(playerMeta?.TeamIndex ?? -1);

            var player = scene.addEntity(new Player(playerMeta.Character, (int)spawnLocation.Position.X, (int)spawnLocation.Position.Y, index));
            player.addComponent(new PlayerController(gamepad));

            _connectedPlayers.Add(player);

            _playerSpawnedThisFrame = true;

            return player;
        }

        private void SpawnIdlePlayer(GamePadData gamepad, int index)
        {
            var player = SpawnPlayer(gamepad, index);
            player.PlayerState = PlayerState.Idle;
        }

        public override void update()
        {
            if (_playerSpawnedThisFrame)
            {
                _playerSpawnedThisFrame = false;
                _hud.BuildHudForAllPlayers(_connectedPlayers);
            }
        }
    }
}