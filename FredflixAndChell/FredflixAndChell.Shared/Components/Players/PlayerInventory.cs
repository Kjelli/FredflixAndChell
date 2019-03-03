using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;

namespace FredflixAndChell.Shared.Components.Players
{
    public class PlayerInventory : Component
    {
        private const float ThrowSpeed = 0.5f;
        private Player _player;
        public Weapon Weapon { get; private set; }
        public bool IsArmed => Weapon != null;

        public override void onAddedToEntity()
        {
            _player = entity as Player;
            var metadata = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
            EquipWeapon(metadata.Weapon?.Name ?? Constants.Strings.DefaultStartWeapon);
        }

        public void EquipWeapon(string name, CollectibleMetadata metadata = null)
        {
            if (Weapon != null)
            {
                DropWeapon();
            }

            var gunParams = Guns.Get(name);
            if (gunParams != null)
            {
                var gunMetadata = metadata != null ? (GunMetadata)metadata : null;
                Weapon = entity.scene.addEntity(new Gun(_player, gunParams, gunMetadata));

                var meta = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
                if (meta != null)
                {
                    meta.Weapon = gunParams;
                }
            }
            else
            {
                var meleeParams = Melees.Get(name);
                if (meleeParams != null)
                {
                    var meleeMetadata = metadata != null ? (MeleeMetadata)metadata : null;
                    Weapon = entity.scene.addEntity(new Melee(_player, meleeParams, meleeMetadata));

                    var meta = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
                    if (meta != null)
                    {
                        meta.Weapon = meleeParams;
                    }
                }
            }
        }

        public void DestroyWeapon()
        {
            Weapon.destroy();
            Weapon = null;
            var meta = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
            if (meta != null)
            {
                meta.Weapon = null;
            }
        }

        public void SwitchWeapon()
        {
#if DEBUG
            if (!IsArmed) return;

            if (Weapon is Melee melee)
            {
                var nextMelee = Melees.GetNextAfter(melee.Parameters.Name ?? "Stick").Name;
                EquipWeapon(nextMelee);
            }
            else if (Weapon is Gun gun)
            {
                var nextGun = Guns.GetNextAfter(gun.Parameters.Name ?? "M4").Name;
                EquipWeapon(nextGun);
            }
#else
            DropWeapon();
#endif
        }

        public void UnequipWeapon()
        {
            Weapon.destroy();
            Weapon = null;
            var meta = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
            if (meta != null)
            {
                meta.Weapon = null;
            }
        }

        public void Reload()
        {
            if (IsArmed && Weapon is Gun gun)
                gun.ReloadMagazine();
        }

        public void Attack()
        {
            if (!IsArmed) return;

            Weapon.Fire();
        }

        public void DropWeapon()
        {
            if (Weapon == null) return;

            Collectible throwedItem = null;

            if (Weapon is Gun gun)
            {
                throwedItem = entity.scene.addEntity(new Collectible(transform.position.X, transform.position.Y,
                    gun.Parameters.Name, true, gun.Metadata));
            }
            else if (Weapon is Melee melee)
            {
                throwedItem = entity.scene.addEntity(new Collectible(transform.position.X, transform.position.Y,
                    melee.Parameters.Name, true, melee.Metadata));
            }

            throwedItem.Metadata?.OnDropEvent?.Invoke(throwedItem, _player);

            throwedItem.Velocity = new Vector2(
                    _player.FacingAngle.X * ThrowSpeed,
                    _player.FacingAngle.Y * ThrowSpeed)
                    + _player.Velocity * 2f;
            UnequipWeapon();
        }
    }
}
