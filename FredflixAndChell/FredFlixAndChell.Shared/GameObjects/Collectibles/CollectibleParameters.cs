using FredflixAndChell.Shared.GameObjects.Weapons;
using static FredflixAndChell.Shared.GameObjects.Collectibles.Collectibles;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public enum CollectibleType
    {
        Weapon = 1
    }
    public class CollectibleParameters
    {
        internal CollectibleParameters() { }
        public CollectibleType Type { get; set; }
        public GunParameters Gun { get; set; }
        
    }
}
