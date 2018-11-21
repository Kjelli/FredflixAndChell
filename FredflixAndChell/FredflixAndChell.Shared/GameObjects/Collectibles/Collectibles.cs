using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Utilities.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    
    public static class Collectibles
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
            var list = All();
            foreach(var item in list)
            {
                if (item.Rarity != rarity)
                    list.Remove(item);
            }
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
            foreach (var gun in Guns.All())
            {
                _collectibles.Add(gun.Name, new CollectibleParameters
                {
                    Name = gun.Name,
                    Gun = gun,
                    Type = CollectibleType.Weapon,
                    DropChance = gun.DropChance,
                    Rarity = gun.Rarity
                });
            }

            _isInitialized = true;
        }
    }
}
