using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Interactables;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Effects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Maps.Events;
using FredflixAndChell.Shared.Utilities;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (other.entity.tag == Tags.Explosion)
            {
                HandleCollisionWithExplosion(other.entity as Explosion);
                return;
            }

            if (other.entity is ProximityEventEmitter pee) // wee
            {
                pee.EmitMapEvent(new object[] { true });
                return;
            }

            if (other.entity is Collectible collectible && collectible.CanBeCollected())
            {
                collectible?.Highlight();
            }

            var interactable = other.entity.getComponent<InteractableComponent>();
            if (interactable == null) return;

            if (_entitiesInProximity.Contains(other.entity)) return;
            _entitiesInProximity.Add(other.entity);
        }

        private void HandleCollisionWithExplosion(Explosion explosion)
        {
            var player = entity as Player;
            var distanceToCenter = (explosion.position - player.position).Length();
            var damagePercentage = Math.Max((Values.ExplosionRadius - distanceToCenter) / Values.ExplosionRadius, 0);
            var damage = damagePercentage * Constants.Values.ExplosionDamage;

            var directionalDamage = new DirectionalDamage
            {
                Damage = damage,
                Direction = -(explosion.position - player.position),
                Knockback = damagePercentage * Values.ExplosionKnockback,
                SourceOfDamage = explosion.ExplosionPlayerSource
            };
            player.Damage(directionalDamage);
        }

        private void HitboxTriggerEnter(Collider other, Collider local)
        {
            var player = entity as Player;
            switch (other.entity.tag)
            {
                case Tags.Pit:
                    player.OnPitHitboxEnter(other.entity);
                    break;

                case Tags.EventEmitter:
                    (other.entity as CollisionEventEmitter).EmitMapEvent(
                    new string[] { Strings.CollisionMapEventEnter });
                    break;
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
            if (other.entity?.tag == Tags.EventEmitter)
            {
                (other.entity as CollisionEventEmitter).EmitMapEvent(
                    new string[] { Strings.CollisionMapEventExit });
            }

            if(other.entity?.tag == Tags.Pit)
            {
                (entity as Player).OnPitHitboxExit(other.entity);
            }
        }

        private void ProximityTriggerExit(Collider other, Collider local)
        {
            if (other == null || other.entity == null) return;

            if (other.entity is Collectible collectible)
            {
                collectible?.Unhighlight();
            }

            if (other.entity is ProximityEventEmitter pee) // wee
            {
                pee.EmitMapEvent(new object[] { false });
                Console.WriteLine("Emitting proximity exit event " + pee.name);
            }

            if (!_entitiesInProximity.Contains(other.entity)) return;
            _entitiesInProximity.Remove(other.entity);

        }

        public void InteractWithNearestEntity()
        {
            if (_entitiesInProximity.Count == 0) return;
            // Find closest entity based on distance between player and collectible
            var closestEntity = _entitiesInProximity.Aggregate((other1, other2) =>
            ((other2.position - entity.position).Length() < (other1.position - entity.position).Length() ? other2 : other1));

            Console.WriteLine("Attempting to interact with " + closestEntity);

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
