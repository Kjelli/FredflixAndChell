using FredflixAndChell.Shared.Components.Guns;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.GameObjects.Collectibles.CollectiblePreset;

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
