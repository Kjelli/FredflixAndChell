using FredflixAndChell.Shared.Systems;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Shared.Utilities
{
    public static class ContextHelper
    {
        public static List<PlayerMetadata> PlayerMetadata { get; set; }
        public static GameSettings GameSettings { get; set; }
        public static bool IsGameInitialized { get; set; }

        public static PlayerMetadata PlayerMetadataByIndex(int index)
        {
            return PlayerMetadata?.FirstOrDefault(p => p.PlayerIndex == index);
        }
    }
}
