using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    
    public static class CollectiblePreset
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

        

        public enum CollectibleType
        {
            Weapon = 1
        }
    }
}
