using FredflixAndChell.Shared.Systems;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Utilities
{
    public static class ContextHelper
    {
        public static string CurrentMap { get; set; }
        public static int NumPlayers { get; set; } = 2;
        public static List<PlayerScore> PlayerScores { get; set; }
    }
}
