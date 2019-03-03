using FredflixAndChell.Shared.Components.HUD;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Components.Players
{
    public class PlayerConnector : SceneComponent
    {
        private List<Player> _connectedPlayers;
        private GameSystem _gameSystem;
        private HUD _hud;
        private bool _playerSpawnedThisFrame;

        public PlayerSpawner _spawnLocations { get; set; }

        public PlayerConnector(PlayerSpawner spawnLocations = null)
        {
            _connectedPlayers = new List<Player>();
            _spawnLocations = spawnLocations;
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

        private void SpawnPlayer(GamePadData gamepad, int index)
        {
            Vector2 spawnLocation = _spawnLocations.DistributeSpawn();
            var spawnX = (int)spawnLocation.X;
            var spawnY = (int)spawnLocation.Y;
            var playerMeta = ContextHelper.PlayerMetadataByIndex(index);
            if (playerMeta == null)
            {
                playerMeta = new PlayerMetadata
                {
                    PlayerIndex = index
                };
            }
            var player = scene.addEntity(new Player(playerMeta.Character, spawnX, spawnY, index));
            player.addComponent(new PlayerController(gamepad));

            _connectedPlayers.Add(player);

            _playerSpawnedThisFrame = true;
        }

        private void SpawnIdlePlayer(GamePadData gamepad, int index)
        {
            Vector2 spawnLocation = _spawnLocations.DistributeSpawn();
            var spawnX = (int)spawnLocation.X;
            var spawnY = (int)spawnLocation.Y;
            var playerMeta = ContextHelper.PlayerMetadataByIndex(index);
            if (playerMeta == null)
            {
                playerMeta = new PlayerMetadata
                {
                    PlayerIndex = index
                };
            }
            var player = scene.addEntity(new Player(playerMeta.Character, spawnX, spawnY, index) { PlayerState = PlayerState.Idle });
            player.addComponent(new PlayerController(gamepad));

            _connectedPlayers.Add(player);

            _playerSpawnedThisFrame = true;
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