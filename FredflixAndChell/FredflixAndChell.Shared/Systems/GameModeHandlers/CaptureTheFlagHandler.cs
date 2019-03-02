using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities.Events;

namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public class CaptureTheFlagHandler : GameModeHandler
    {
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
            int teamIndex = -1;
            switch (mapEvent.EventKey)
            {
                case "ctf_red":
                    teamIndex = 1;
                    break;
                case "ctf_blue":
                    teamIndex = 2;
                    break;
                default:
                    return;
            }

            if ((string)mapEvent.Parameters[0] != Constants.Strings.CollisionMapEventEnter) return;



        }

        public override bool WeHaveAWinner()
        {
            return false;
        }
    }
}
