using FredflixAndChell.Shared.GameObjects.Weapons;
using static FredflixAndChell.Shared.GameObjects.Collectibles.CollectiblePresets;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public class CollectibleParameters
    {
        internal CollectibleParameters() { }
        public CollectibleType Type { get; set; }
        //Leave null if not that type bruh
        public GunParameters Gun { get; set; }
        
    }
}
