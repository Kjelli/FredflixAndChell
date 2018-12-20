using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Interactables;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Maps.Events;
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

            var interactable = other.entity.getComponent<InteractableComponent>();
            if (interactable != null)
            {
                _entitiesInProximity.Add(other.entity);
                if (other.entity is Collectible collectible && collectible.CanBeCollected())
                {
                    collectible?.Highlight();
                }
            }
        }

        private void HitboxTriggerEnter(Collider other, Collider local)
        {
            if (other.entity.tag == Tags.Pit)
            {
                var player = entity as Player;
                if (player.PlayerMobilityState != PlayerMobilityState.Rolling)
                {
                    player.FallIntoPit(other.entity);
                }
            }
            else if (other.entity.tag == Tags.EventEmitter)
            {
                (other.entity as CollisionEventEmitter).EmitMapEvent(
                    new string[] { Strings.CollisionMapEventEnter });
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
            if (other.entity.tag == Tags.EventEmitter)
            {
                (other.entity as CollisionEventEmitter).EmitMapEvent(
                    new string[] { Strings.CollisionMapEventExit });
            }
        }

        private void ProximityTriggerExit(Collider other, Collider local)
        {
            if (other == null || other.entity == null) return;
            if (!_entitiesInProximity.Contains(other.entity)) return;

            _entitiesInProximity.Remove(other.entity);

            if (other.entity is Collectible collectible)
            {
                collectible?.Unhighlight();
            }
        }

        public void InteractWithNearestEntity()
        {
            if (_entitiesInProximity.Count == 0) return;
            // Find closest entity based on distance between player and collectible
            var closestEntity = _entitiesInProximity.Aggregate((other1, other2) =>
            ((other2.position - entity.position).Length() < (other1.position - entity.position).Length() ? other2 : other1));

            var interactable = closestEntity.getComponent<InteractableComponent>();
            if (interactable == null) return;

            interactable.OnInteract(_player);

            if (closestEntity is Collectible collectible)
            {
                _entitiesInProximity.Remove(closestEntity);
            }
        }
    }
}
