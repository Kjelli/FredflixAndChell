
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public static class Guns
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, GunParameters> _guns = new Dictionary<string, GunParameters>();

        public static List<GunParameters> All()
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _guns.Values.ToList();
        }

        public static GunParameters Get(string name)
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _guns[name];
        }

        public static void LoadFromData()
        {
            Console.WriteLine("=== LOADING FML FILES FOR GUNS ===");
            var gunFilenames = Directory.EnumerateFiles($"{Constants.Assets.DataDirectory}/guns", "*.fml");
            foreach (var gunFilename in gunFilenames)
            {
                var gun = YamlSerializer.Deserialize(gunFilename);
                _guns[gun.Name] = gun;
            }
            _isInitialized = true;
        }

        public static GunParameters GetNextAfter(string name)
        {
            var list = _guns.Values.ToList();
            var next = list[(list.IndexOf(_guns[name]) + 1) % _guns.Count];
            return next;
        }
    }
}
