using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
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
            EquipWeapon(metadata.Weapon ?? CollectibleDict.Get(Constants.Strings.DefaultStartWeapon).Weapon);
        }

        public void EquipWeapon(WeaponParameters weapon, CollectibleMetadata metadata = null)
        {
            if (Weapon != null)
            {
                DropWeapon();
            }

            Weapon newWeapon = null;
            if (weapon.Type == WeaponType.Gun)
            {
                newWeapon = new Gun(_player, weapon as GunParameters, metadata as GunMetadata);
            }
            else if (weapon.Type == WeaponType.Melee)
            {
                newWeapon = new Melee(_player, weapon as MeleeParameters, metadata as MeleeMetadata);
            }

            Weapon = entity.scene.addEntity(newWeapon);

            var meta = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
            if (meta != null)
            {
                meta.Weapon = weapon;
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

            var next = CollectibleDict.GetNextWeaponAfter(Weapon.Name);
            EquipWeapon(next);
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
