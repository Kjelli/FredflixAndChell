using FredflixAndChell.Shared.GameObjects.Weapons;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    
    public static class CollectiblePresets
    {
        public static readonly CollectibleParameters M4 = new CollectibleParameters
        {
            Type = CollectibleType.Weapon,
            Gun = GunPresets.M4
        };

        public static readonly CollectibleParameters Fido = new CollectibleParameters
        {
            Type = CollectibleType.Weapon,
            Gun = GunPresets.Fido
        };

        public static readonly CollectibleParameters PewPew = new CollectibleParameters
        {
            Type = CollectibleType.Weapon,
            Gun = GunPresets.PewPew
        };

        public enum CollectibleType
        {
            Weapon = 1
        }
    }
}
