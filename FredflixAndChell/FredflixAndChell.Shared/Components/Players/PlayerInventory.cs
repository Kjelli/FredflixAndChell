using FredflixAndChell.Shared.GameObjects.Collectibles;
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
        private WeaponParameters _weaponParameters;
        
        public Weapon Weapon { get; private set; }
        public bool IsArmed => Weapon != null;

        public PlayerInventory(WeaponParameters weaponParameters)
        {
            _weaponParameters = weaponParameters;
        }
        public override void onAddedToEntity()
        {
            _player = entity as Player;
            EquipWeapon(_weaponParameters.Name);
        }

        public void EquipWeapon(string name)
        {
            if (Weapon != null)
            {
                DropWeapon();
            }

            var gunParams = Guns.Get(name);
            if (gunParams != null)
            {
                Weapon = entity.scene.addEntity(new Gun(_player, gunParams));

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
                    Weapon = entity.scene.addEntity(new Melee(_player, meleeParams));

                    var meta = ContextHelper.PlayerMetadataByIndex(_player.PlayerIndex);
                    if (meta != null)
                    {
                        meta.Weapon = meleeParams;
                    }
                }
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
#endif
        }

        public void UnequipWeapon()
        {
            Weapon.destroy();
            Weapon = null;
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
            if (Weapon != null)
            {
                Collectible throwedItem = null;
                if (Weapon is Gun gun)
                {
                    throwedItem = entity.scene.addEntity(new Collectible(transform.position.X, transform.position.Y, gun.Parameters.Name, true));

                }
                else if (Weapon is Melee melee)
                {
                    throwedItem = entity.scene.addEntity(new Collectible(transform.position.X, transform.position.Y, melee.Parameters.Name, true));
                }

                throwedItem.Velocity = new Vector2(
                        _player.FacingAngle.X * ThrowSpeed,
                        _player.FacingAngle.Y * ThrowSpeed)
                        + _player.Velocity * 2f;
                UnequipWeapon();
            }
        }
    }
}
