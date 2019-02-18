using FredflixAndChell.Shared.Systems;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Utilities
{
    public static class ContextHelper
    {
        public static List<PlayerMetadata> PlayerMetadata { get; set; }
        public static GameSettings GameSettings { get; set; }
        public static bool IsGameInitialized { get; set; }
    }
}
