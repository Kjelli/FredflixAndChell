using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public static class CollectibleDict
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, CollectibleParameters> _collectibles =
            new Dictionary<string, CollectibleParameters>();

        public static List<CollectibleParameters> All()
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _collectibles.Values.ToList();
        }

        public static List<CollectibleParameters> All(Rarity rarity)
        {
            var list = All().Where(w => w.Rarity == rarity).ToList();
            return list;
        }

        public static CollectibleParameters Get(string name)
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _collectibles[name];
        }

        public static void LoadFromData()
        {
            if (_isInitialized) return;

            Guns.LoadFromData();
            foreach (var gun in Guns.All())
            {
                _collectibles.Add(gun.Name, new CollectibleParameters
                {
                    Name = gun.Name,
                    Weapon = gun,
                    Type = CollectibleType.Weapon,
                    DropChance = gun.DropChance,
                    Rarity = gun.Rarity
                });
            }

            Melees.LoadFromData();
            foreach (var melee in Melees.All())
            {
                _collectibles.Add(melee.Name, new CollectibleParameters
                {
                    Name = melee.Name,
                    Weapon = melee,
                    Type = CollectibleType.Weapon,
                    DropChance = melee.DropChance,
                    Rarity = melee.Rarity
                });
            }

            _isInitialized = true;
        }

        public static WeaponParameters GetNextWeaponAfter(string name)
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }

            var list = _collectibles.Values
                .Where(c => c.Type == CollectibleType.Weapon)
                .ToList();
            var next = list[(list.IndexOf(_collectibles[name]) + 1) % _collectibles.Count];
            return next.Weapon;
        }
    }
}
