using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Players
{
    public class CollectibleCollisionHandler : Component, ITriggerListener
    {
        private Collectible _collectible;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            _collectible = entity as Collectible;
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            if (other == null || other.entity == null) return;
            if (_collectible.CollectibleState == CollectibleState.Appearing) return;

            if (other.entity.tag == Tags.Pit)
            {
                _collectible.FallIntoPit(other.entity);
            }
        }

        public void onTriggerExit(Collider other, Collider local)
        {
        }

    }
}
