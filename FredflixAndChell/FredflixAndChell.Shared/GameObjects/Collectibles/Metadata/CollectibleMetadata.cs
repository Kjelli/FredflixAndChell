using System.Collections.Generic;

namespace FredflixAndChell.Shared.GameObjects.Collectibles.Metadata
{
    public class CollectibleMetadata
    {
        protected Dictionary<string, object> Data { get; set; }
        public CollectibleMetadata()
        {
            Data = new Dictionary<string, object>();
        }
    }
}
