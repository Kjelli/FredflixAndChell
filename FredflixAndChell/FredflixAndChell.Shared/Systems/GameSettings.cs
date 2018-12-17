
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
            GameMode = GameModes.Deathmatch,
            Map = "winter_1",
            ScoreLimit = 2
        };

        public Teams Team { get; set; }
        public GameModes GameMode { get; set; }
        public string Map { get; set; }
        public int ScoreLimit { get; set; }
    }
}
