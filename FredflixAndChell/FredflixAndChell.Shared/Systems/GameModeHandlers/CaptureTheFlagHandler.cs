using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using System.Linq;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class CaptureTheFlagHandler : GameModeHandler
    {
        private int _winningTeamIndex;

        public CaptureTheFlagHandler(GameSystem gameSystem) : base(gameSystem)
        {
        }

        public override void Setup(GameSettings settings)
        {
            base.Setup(settings);
            GameSystem.Subscribe(GameEvents.GlobalMapEvent, HandleCTFMapEvent);
        }

        private void HandleCTFMapEvent(GameEventParameters ev)
        {
            if (!(ev is GlobalMapEventParameters globalMapEvent)) return;

            var mapEvent = globalMapEvent.MapEvent;
            var key = mapEvent.EventKey;
            var player = mapEvent.Parameters[1] as Player;
            var playerInventory = player.getComponent<PlayerInventory>();

            if ((string)mapEvent.Parameters[0] != Constants.Strings.CollisionMapEventEnter) return;
            if (playerInventory.Weapon?.Name != "Flag") return;

            if (key == Constants.TiledProperties.CaptureTheFlagRedCollisionZone
                && player.TeamIndex == Constants.Values.TeamIndexRed)
            {
                _winningTeamIndex = player.TeamIndex;
            }
            else if (key == Constants.TiledProperties.CaptureTheFlagBlueCollisionZone
                && player.TeamIndex == Constants.Values.TeamIndexBlue)
            {
                _winningTeamIndex = player.TeamIndex;
            }

            if (_winningTeamIndex > 0)
            {
                playerInventory.DestroyWeapon();
                FocusCameraOnWinners();
                GameSystem.EndRound();
            }
        }

        private void FocusCameraOnWinners()
        {
            var losingPlayers = GameSystem.Players.Where(p => p.TeamIndex != _winningTeamIndex);
            var winningPlayers  = GameSystem.Players.Where(p => p.TeamIndex == _winningTeamIndex);
            foreach (var player in losingPlayers)
            {
                player.getComponent<CameraTracker>().setEnabled(false);
            }

            foreach (var player in winningPlayers)
            {
                ContextHelper.PlayerMetadataByIndex(player.PlayerIndex).Score++;
            }
        }

        public override bool WeHaveAWinner()
        {
            var anyWinningPlayer = GameSystem.Players.First(p => p.TeamIndex == _winningTeamIndex);
            var playerMeta = ContextHelper.PlayerMetadataByIndex(anyWinningPlayer.PlayerIndex);
            return playerMeta.Score >= GameSystem.Settings.ScoreLimit;
        }
    }
}
