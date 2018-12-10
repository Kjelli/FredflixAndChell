
using FredflixAndChell.Shared.Systems.GameModeHandlers;
using Nez;

namespace FredflixAndChell.Shared.Systems
{
    public enum Teams
    {
        FreeForAll,
        Team
    }

    public class GameSettings
    {
        public static GameSettings Default = new GameSettings
        {
            GameMode = GameModes.Rounds,
            Map = "winter_debug"
        };

        public Teams Team { get; set; }
        public GameModes GameMode { get; set; }
        public string Map { get; set; }
    }
}
