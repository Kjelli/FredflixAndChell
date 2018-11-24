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
    class PlayerCollisionHandler : Component, ITriggerListener
    {
        private Collider _playerHitbox;
        private Collider _proximityHitbox;
        private List<Entity> _entitiesInProximity;
        private Player _player;

        public PlayerCollisionHandler(Collider hitboxCollider, Collider proximityCollider)
        {
            _entitiesInProximity = new List<Entity>();
            _playerHitbox = hitboxCollider;
            _proximityHitbox = proximityCollider;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            _player = entity as Player;
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            if (local == _playerHitbox)
            {
                HitboxTriggerEnter(other, local);
            }
            else if (local == _proximityHitbox)
            {
                ProximityTriggerEnter(other, local);
            }
        }

        private void ProximityTriggerEnter(Collider other, Collider local)
        {
            if (other == null || other.entity == null) return;
            if (_entitiesInProximity.Contains(other.entity)) return;

            Console.WriteLine($"Entered proximity: ${other.entity}");

            _entitiesInProximity.Add(other.entity);

            // TODO change tag to include other interactables if relevant
            if (Flags.isFlagSet(other.entity.tag, Tags.Collectible))
            {
                var collectible = other.entity as Collectible;
                if (collectible.CanBeCollected())
                {
                    collectible?.Highlight();
                }
            }
        }

        private void HitboxTriggerEnter(Collider other, Collider local)
        {
            if (other.entity.tag == Tags.Pit)
            {
                (entity as Player).FallIntoPit(other.entity);
            }
        }

        public void onTriggerExit(Collider other, Collider local)
        {
            if (local == _playerHitbox)
            {
                HitboxTriggerExit(other, local);
            }
            else if (local == _proximityHitbox)
            {
                ProximityTriggerExit(other, local);
            }
        }

        private void HitboxTriggerExit(Collider other, Collider local)
        {
        }

        private void ProximityTriggerExit(Collider other, Collider local)
        {
            if (other == null || other.entity == null) return;
            if (!_entitiesInProximity.Contains(other.entity)) return;

            Console.WriteLine($"Left proximity: ${other.entity}");
            _entitiesInProximity.Remove(other.entity);
            if (other.entity != null && Flags.isFlagSet(other.entity.tag, Tags.Collectible))
            {
                var collectible = other.entity as Collectible;
                collectible?.Unhighlight();
            }
        }

        public void InteractWithNearestEntity()
        {
            if (_entitiesInProximity.Count == 0) return;

            // Find closest entity based on distance between player and collectible
            var closestEntity = _entitiesInProximity.Aggregate((curMin, x) =>
            ((x.position - entity.position).Length() < (curMin.position - entity.position).Length() ? x : curMin));

            if (closestEntity is Collectible collectible)
            {
                if (collectible == null || !collectible.CanBeCollected()) return;

                if (collectible.Preset.Type == CollectibleType.Weapon)
                {
                    _player.EquipGun(collectible.Preset.Gun.Name);
                    collectible.OnPickup();
                    _entitiesInProximity.Remove(closestEntity);
                }
            }
        }
    }
}
