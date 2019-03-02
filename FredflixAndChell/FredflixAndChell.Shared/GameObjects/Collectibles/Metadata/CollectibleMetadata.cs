using FredflixAndChell.Shared.GameObjects.Players;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.GameObjects.Collectibles.Metadata
{
    public class CollectibleMetadata
    {
        public Color Color { get; set; }
        public Action<Collectible> OnDestroyEvent { get; set; }
        public Action<Collectible, Player> OnDropEvent { get; set; }
        public Action<Collectible, Player> OnPickupEvent { get; set; }
        public List<Func<Player, bool>> CanCollectRules { get; set; }
        protected Dictionary<string, object> Data { get; set; }
        public CollectibleMetadata()
        {
            Color = Color.White;
            Data = new Dictionary<string, object>();
            CanCollectRules = new List<Func<Player, bool>>();
        }
    }
}
